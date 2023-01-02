using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SDL2;
using static SDL2.SDL;
using System.Reflection;
using System.Diagnostics;


unsafe class Map
{

    public int[,] pixels;
    private List<int>[] transitionsY;
    private List<int>[] transitionsX;
    private Dictionary<CoordinateAxis, List<int>[]> transitions;

    private List<Curve> curves;


    SDL.SDL_Surface* pixelMap;
    private int w;
    private int h;

    //TODO: come up with color key for what the different types of blocks represent
    public static readonly int AIR_CODE = 255 + 255;
    public static readonly int GROUND_CODE = 113;
    public static readonly int PASS_THROUGH_CODE = 255;
    public static readonly int SOLID_CODE = 255 + 199;
    public static readonly int ENEMY_WALKING = 5;
    public static readonly int ENEMY_FLYING = 25;

    //public static readonly int TUNNEL_CODE = 

    private static readonly int SLOPE_MAX_COUNT = 15;

    public static readonly int CLOSE_THRESHOLD = 10;


    public Map(String loc)
    {
        //getting the memory location of the collision map
        pixelMap = (SDL.SDL_Surface*)SDL.SDL_LoadBMP(Engine.GetAssetPath(loc));

        SDL.SDL_LockSurface((IntPtr)pixelMap);

        int pitch = (*pixelMap).pitch;

        w = (*pixelMap).w;
        h = (*pixelMap).h;

        pixels = new int[w, h];
        transitionsY = new List<int>[w];
        transitionsX = new List<int>[h];
        for (int i = 0; i < w; i++)
        {
            transitionsY[i] = new List<int>();
        }
        for (int i = 0; i < h; i++)
        {
            transitionsX[i] = new List<int>();
        }

        int bpp = 4;
        byte* pixelsImg = (byte*)(*pixelMap).pixels;

        Enemy enemyToAdd;
        //looping through all the pixels
        for (int x = 0; x < pixels.GetLength(0); x++)
        {
            for (int y = 0; y < pixels.GetLength(1); y++)
            {
                pixels[x, y] = *(pixelsImg + pitch * y + bpp * x) + *(pixelsImg + pitch * y + bpp * x + 2);


                //checking for rings
                Vector2 locVect = new Vector2(x, y);
                if (pixels[x, y] == (245 + 255))
                {
                    Game.flowers.Add(new Flower(locVect));
                    pixels[x, y] = AIR_CODE;
                }

                // checking for tunnel
                //if (pixels[x, y] == (TUNNEL_CODE))
                //{

                //}

                //checking for enemies
                Bounds2 testPath = new Bounds2(new Vector2(locVect.X - 50, 0), new Vector2(locVect.X + 50, 0));
                if (pixels[x, y] == ENEMY_WALKING)
                {
                    enemyToAdd = new Enemy(locVect, testPath, false);
                    enemyToAdd.setState(State.Walk);
                    Game.enemies.Add(enemyToAdd);
                    pixels[x, y] = AIR_CODE;
                }

                //checking for flying enemies
                if (pixels[x, y] == ENEMY_FLYING)
                {
                    enemyToAdd = new Enemy(locVect, testPath, true);
                    enemyToAdd.setState(State.Walk);
                    Game.enemies.Add(enemyToAdd);
                    pixels[x, y] = AIR_CODE;
                }

                //NOTE: these are putting the surfaces up 1 shifted

                //looking for air to ground transitions
                if (y > 0 && pixels[x, y] != pixels[x, y - 1])
                {
                    if (pixels[x, y - 1] == AIR_CODE)
                        transitionsY[x].Add(y);
                    else if (pixels[x, y] == AIR_CODE)
                        transitionsY[x].Add(y - 1);
                }

                //horizontal 
                if (x > 0 && pixels[x, y] != pixels[x - 1, y])
                {
                    if ((pixels[x, y] == GROUND_CODE || pixels[x, y] == SOLID_CODE) && (pixels[x - 1, y] == AIR_CODE || pixels[x - 1, y] == PASS_THROUGH_CODE))
                        transitionsX[y].Add(x);
                    else if ((pixels[x - 1, y] == GROUND_CODE || pixels[x - 1, y] == SOLID_CODE) && (pixels[x, y] == AIR_CODE || pixels[x, y] == PASS_THROUGH_CODE))
                        transitionsX[y].Add(x - 1);
                }

            }
        }
        transitions = new Dictionary<CoordinateAxis, List<int>[]>();
        transitions.Add(CoordinateAxis.X, transitionsX);
        transitions.Add(CoordinateAxis.Y, transitionsY);


        SDL.SDL_UnlockSurface((IntPtr)pixelMap);
        SDL.SDL_FreeSurface((IntPtr)pixelMap);


        curves = new List<Curve>();

    }

    public void addCurve(Curve c)
    {
        curves.Add(c);
    }

    //gets the pixel type at the given coordinate
    public int getPixelType(Vector2 coord)
    {
        int x = (int)Math.Round(coord.X);
        int y = (int)Math.Round(coord.Y);
        if (x >= w || x < 0 || y >= h || y < 0)
        {
            return AIR_CODE;
        }
        return pixels[x, y];
    }

    public bool onGround(Vector2 coord)
    {
        return getPixelType(coord) == GROUND_CODE || getPixelType(coord) == SOLID_CODE;
    }

    public bool onSolid(Vector2 coord)
    {
        return getPixelType(coord) == GROUND_CODE || getPixelType(coord) == SOLID_CODE;
    }

    public bool inAir(Vector2 coord)
    {
        return getPixelType(coord) == AIR_CODE;
    }

    public bool throughThrough(Vector2 coord)
    {
        return getPixelType(coord) == PASS_THROUGH_CODE;
    }

    //checking if it's coming in from the top for the light red collisions
    public bool closeToSurface(Vector2 coord)
    {
        return Math.Abs(getSurfaceY(coord) - coord.Y) < CLOSE_THRESHOLD;
    }

    public bool passingSolid(Vector2 initial, Vector2 final)
    {
        if (inAir(initial) && onSolid(final))
        {
            return true;
        }

        if (inAir(initial) && throughThrough(final) && closeToSurface(final))
        {
            if (Vector2.Dot((final - initial), getNormalVector(final)) < 0)
            {
                return true;
            }
        }

        return false;
    }

    //getting normal vectors
    // TODO: remember to account for cases where the ground is above or below and just general edge cases like vert
    // TODO: use svg to get exact normal vectors
    //need to fix for LOTS of edge cases
    // TODO: possibly change to hashmap system to decrease time complexity
    public Vector2 getNormalVector(Vector2 pos)
    {
        Vector2 surfacePoint = getNearestSurfacePoint(pos);

        foreach (Curve c in curves)
        {
            if (c.contains(surfacePoint))
            {
                return c.getNearestNormal(surfacePoint);
            }
        }

        if ((int)pos.X == 4361)
        {
            return new Vector2(-1, 0);
        }
        if (pos.X > 4300 && pos.X < 4361)
        {
            return new Vector2(0, -1);
        }

        Vector2 slope = new Vector2(10, 0);
        slope.Y = getSurfaceY(pos + slope / 2) - getSurfaceY(pos - slope / 2);

        return slope.Rotated(270).Normalized();


        return new Vector2(0, -1);
    }

    //TODO: need to implement -- something along th elines of the commented out code
    //TODO: use svg to get exact curvatures
    public float getSurfaceRadius(Vector2 pos)
    {
        return -1;
        /*Vector2 shift = new Vector2(5, 0);
        Vector2 x1 = new Vector2(pos.X - shift.X, getSurfaceY(pos - shift));
        Vector2 x2 = new Vector2(pos.X + shift.X, getSurfaceY(pos + shift));
        Vector2 norm1 = getNormalVector(x1);
        Vector2 norm2 = getNormalVector(x2);
        float angle = (float) Math.Round(Math.Acos(Vector2.Dot(norm1, norm2)),1);

        if (angle == 0)
            return -1;
        return (x1 - x2).Length() / angle;*/
    }

    //returns the point to put the sprite on when hovering over ground
    public Vector2 getNearestHoveringPoint(Vector2 pos)
    {
        Vector2 surfacePoint = getNearestSurfacePoint(pos);
        Vector2 norm = getNormalVector(surfacePoint);
        return surfacePoint + norm;
    }



    //returns the nearest surface point either moving horizontally or vertically
    //TODO: actually implementing this method, this one is just going straight up to surface
    //tiebreaker goes vertical?
    public Vector2 getNearestSurfacePoint(Vector2 pos)
    {
        int xSurface = getSurfaceX(pos);
        int ySurface = getSurfaceY(pos);

        if (Math.Abs(xSurface - pos.X) < Math.Abs(ySurface - pos.Y))
        {
            return new Vector2(xSurface, pos.Y);
        }
        return new Vector2(pos.X, ySurface);

    }

    public int getSurfaceAny(Vector2 pos, CoordinateAxis direc)
    {
        List<int> currTransitions = transitions[direc][(int)Math.Round(pos.getComp(direc.Flip()))];
        float criticalCoord = pos.getComp(direc);
        for (int i = 0; i < currTransitions.Count; i++)
        {
            //dividing off by halves and returning nearest one
            if (i + 1 >= currTransitions.Count || criticalCoord < (currTransitions[i] + currTransitions[i + 1]) / 2)
            {
                return currTransitions[i];
            }
        }
        return -1;
    }

    public int getSurfaceX(Vector2 pos)
    {
        return getSurfaceAny(pos, CoordinateAxis.X);
    }

    public int getSurfaceY(Vector2 pos)
    {
        return getSurfaceAny(pos, CoordinateAxis.Y);
    }
}
/*
class BezierCurve
{
    private Vector2 start;
    private Vector2 startHandle;
    private Vector2 end;
    private Vector2 endHandle;

    public BezierCurve(Vector2 start, Vector2 startHandle, Vector2 end, Vector2 endHandle)
    {
        this.start = start;
        this.startHandle = startHandle;
        this.end = end;
        this.endHandle = endHandle;
    }


}*/


interface Curve
{
    Vector2 getNearestNormal(Vector2 pos); //for getting normal vector generally
    float getNearestCurvature(Vector2 pos); //for getting curvature
    bool contains(Vector2 pos);
}
/*
class CurveGroup
{
    private BezierCurve[] curves;

    public CurveGroup(BezierCurve[] curves)
    {
        this.curves = curves;
    }

    public virtual Vector2 getNearestNormalVector(Vector2 pos)
    {
        return new Vector2(0, -1);
    }

}*/

/*
class BezierCurve : Curve
{
    private Dictionary<Vector2, Vector2> normalVectors;
    private Dictionary<Vector2, float> curvature;

    private Vector2 start;
    private Vector2 startHandle;
    private Vector2 end;
    private Vector2 endHandle;
    private float halfStroke;

    public BezierCurve(Vector2 start, Vector2 startHandle, Vector2 end, Vector2 endHandle)
    {
        this.start = start;
        this.startHandle = startHandle;
        this.end = end;
        this.endHandle = endHandle;

        //getting normal vectors and curvature at points it crosses
        //NOTE, however, this is an approximation, with the incrementing, but should work for all practical purposes
        float t = 0;
        Vector2 currVel = getVelocity(t);
        Vector2 currLoc = getVelocity(0);
        while (t < 1)
        {
            //have to decide if rounding is correct or not here
            normalVectors.Add(currLoc.Rounded(0), currVel.Normalized().Rotated(270));

            t += Math.Min(timeToNearest(currLoc, currVel, CoordinateAxis.X), timeToNearest(currLoc, currVel, CoordinateAxis.Y));
            currVel = getVelocity(t);
            currLoc = getLoc(t);

        }


    }

    private float timeToNearest(Vector2 loc, Vector2 vel, CoordinateAxis direc)
    {
        float velComp = vel.getComp(direc);
        float locComp = loc.getComp(direc);
     
       if (velComp == 0)
        {
            return float.MaxValue;
        } 

        return (float)(velComp > 0 ? (Math.Ceiling(locComp) - locComp) : (Math.Floor(locComp) - locComp)) / vel.Y;
    }

    //gets location at linearly interpolated time t
    private Vector2 getLoc(float t)
    {
        return (float)Math.Pow(1 - t, 3) * start + 3 * (float)Math.Pow(1 - t, 2) * t * startHandle + 3 * (1 - t) * (float)Math.Pow(t, 2) * endHandle + (float)Math.Pow(t, 3) * end;
    }

    //gets velocity at linearly interpolated time t
    private Vector2 getVelocity(float t)
    {
        return 3 * (float)Math.Pow(1 - t, 2) * (startHandle - start) + 6 * (1 - t) * t * (endHandle - startHandle) + 3 * (float)Math.Pow(t, 2) * (end - endHandle);
    }

    //gets acceleration at linearly interpolated time t
    private Vector2 getAcc(float t)
    {
        return 6 * (1 - t) * (endHandle - 2 * startHandle + start) + 6 * t * (end - 2 * endHandle + startHandle);
    }

    private float getCurvature(float t)
    {
        return Vector2.Cross(getAcc(t), getVelocity(t)) / (float) Math.Pow(getVelocity(t).Length(), 3);
    }


}*/

class Rect : Curve
{
    Bounds2 rect;
    public Rect(Bounds2 rect)
    {
        this.rect = rect;
    }

    public Rect(string[] s)
    {
        rect = new Bounds2(0,0,0,0);
        if(s.Length == 8)
        {
            rect.Position.X = (float)Math.Round(Double.Parse(s[6]));
            rect.Position.Y = (float)Math.Round(Double.Parse(s[7]));
            rect.Size.X = (float)Math.Round(Double.Parse(s[0])) - 1;
            rect.Size.Y = (float)Math.Round(Double.Parse(s[1])) - 1;
        }
        else if(s.Length >= 4)
        {
            rect.Position.X = (float)Math.Round(Double.Parse(s[0]));
            rect.Position.Y = (float)Math.Round(Double.Parse(s[1]));
            rect.Size.X = (float)Math.Round(Double.Parse(s[2])) - 1;
            rect.Size.Y = (float)Math.Round(Double.Parse(s[3])) - 1;
        }
    }

    public Vector2 getNearestNormal(Vector2 pos)
    {
        if (!contains(pos))
        {
            return Vector2.UP;
        }
        if (pos.Y == rect.Min.Y)
        {
            return Vector2.UP;
        }
        else if (pos.Y == rect.Max.Y)
        {
            return Vector2.DOWN;
        }
        else if (pos.X == rect.Max.X)
        {
            return Vector2.RIGHT;
        }
        else if (pos.X == rect.Min.X)
        {
            return Vector2.LEFT;
        }
        return Vector2.UP;

    }

    public float getNearestCurvature(Vector2 pos)
    {
        return -1;
    }

    public bool contains(Vector2 pos)
    {
        return rect.Contains(pos);
    }
}

