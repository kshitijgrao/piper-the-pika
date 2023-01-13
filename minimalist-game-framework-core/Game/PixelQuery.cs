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
    public List<Path2> paths;


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
    public static readonly int PATH_CODE = 235;

    public static readonly int SPRING_CODE = 123;
    public static readonly int SPIKE_CODE = 132;

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

                //checking for enemies
                Bounds2 testPath = new Bounds2(new Vector2(locVect.X - 50, 0), new Vector2(locVect.X + 50, 0));
                if (pixels[x, y] == ENEMY_WALKING || pixels[x, y] == ENEMY_FLYING)
                {
                    pixels[x, y] = AIR_CODE;
                }

                //NOTE: these are putting the surfaces up 1 shifted

                //looking for air to ground transitions
                if (y > 0 && pixels[x, y] != pixels[x, y - 1])
                {
                    if ((pixels[x, y - 1] == AIR_CODE ) && (pixels[x,y] == GROUND_CODE || pixels[x,y] == SOLID_CODE || pixels[x,y] == PASS_THROUGH_CODE || pixels[x, y - 1] == SPIKE_CODE))
                        transitionsY[x].Add(y);
                    else if ((pixels[x, y] == AIR_CODE) && (pixels[x, y - 1] == GROUND_CODE || pixels[x, y - 1] == SOLID_CODE || pixels[x, y - 1] == PASS_THROUGH_CODE || pixels[x, y - 1] == SPIKE_CODE))
                        transitionsY[x].Add(y - 1);
                }

                //horizontal 
                if (x > 0 && pixels[x, y] != pixels[x - 1, y])
                {
                    if ((pixels[x, y] == GROUND_CODE || pixels[x, y] == SOLID_CODE || pixels[x,y] == SPIKE_CODE) && (pixels[x - 1, y] == AIR_CODE || pixels[x - 1, y] == PASS_THROUGH_CODE))
                        transitionsX[y].Add(x);
                    else if ((pixels[x - 1, y] == GROUND_CODE || pixels[x - 1, y] == SOLID_CODE || pixels[x-1,y] == SPIKE_CODE) && (pixels[x, y] == AIR_CODE || pixels[x, y] == PASS_THROUGH_CODE))
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
        paths = new List<Path2>();

    }

    public void addCurve(Curve c)
    {
        curves.Add(c);
    }

    public void addPath(Path2 p)
    {
        paths.Add(p);
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

    public bool onSpike(Vector2 coord)
    {
        return getPixelType(coord) == SPIKE_CODE;
    }

    public bool onGround(Vector2 coord)
    {
        return getPixelType(coord) == GROUND_CODE || getPixelType(coord) == SOLID_CODE;
    }

    public bool onPath(Vector2 coord)
    {
        return getPixelType(coord) == PATH_CODE;
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
        int? surfaceY = getSurfaceY(coord);
        if (!surfaceY.HasValue)
        {
            return false;
        }

        return Math.Abs(surfaceY.Value - coord.Y) < CLOSE_THRESHOLD;
    }

    public bool passingSolid(Vector2 initial, Vector2 final)
    {
        if ( (inAir(initial) || throughThrough(initial)) && onSolid(final))
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

        if(inAir(initial) && onSpike(final))
        {
            if (getNormalVector(final).Y == 0)
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
        Vector2 slope = new Vector2(10, 0);

        if(!getSurfaceY(pos + slope / 2).HasValue || !getSurfaceY(pos - slope / 2).HasValue)
        {
            return Vector2.UP;
        }

        slope.Y = getSurfaceY(pos + slope / 2).Value - getSurfaceY(pos - slope / 2).Value;

        return slope.Rotated(270).Normalized();
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

    public Path2 getPath(Vector2 pos)
    {
        foreach (Path2 path in paths)
        {
            if (path.contains(pos))
            {
                return path;
            }
        }
        return paths[0];
    }



    //returns the nearest surface point either moving horizontally or vertically
    //TODO: actually implementing this method, this one is just going straight up to surface
    //tiebreaker goes vertical?
    public Vector2 getNearestSurfacePoint(Vector2 pos)
    {
        int? xSurface = getSurfaceX(pos);
        int? ySurface = getSurfaceY(pos);

        if (!xSurface.HasValue && ySurface.HasValue)
        {
            return new Vector2(pos.X,ySurface.Value);
        }
        else if(xSurface.HasValue && !ySurface.HasValue)
        {
            return new Vector2(xSurface.Value, pos.Y);
        }
        else if(!xSurface.HasValue && !ySurface.HasValue)
        {
            return new Vector2(160, 960);
        }


        

        if (Math.Abs(xSurface.Value - pos.X) < Math.Abs(ySurface.Value - pos.Y))
        {
            return new Vector2(xSurface.Value, pos.Y);
        }
        return new Vector2(pos.X, ySurface.Value);

    }

    public int? getSurfaceAny(Vector2 pos, CoordinateAxis direc)
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
        return null;
    }

    public int? getSurfaceX(Vector2 pos)
    {
        return getSurfaceAny(pos, CoordinateAxis.X);
    }

    public int? getSurfaceY(Vector2 pos)
    {
        return getSurfaceAny(pos, CoordinateAxis.Y);
    }
}


