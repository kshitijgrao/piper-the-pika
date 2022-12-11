using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SDL2;
using static SDL2.SDL;
using System.Reflection;
using System.Diagnostics;

unsafe class Map {

    public int[,] pixels;
    private List<int>[] transitions;
    SDL.SDL_Surface* pixelMap;
    private int w;
    private int h;

    //TODO: come up with color key for waht the different types of blocks represent
    public static readonly int AIR_CODE = 255+255;
    public static readonly int GROUND_CODE = 113;
    public static readonly int PASS_THROUGH_CODE = 255;
    public static readonly int SOLID_CODE = 255 + 199;

    private static readonly int SLOPE_MAX_COUNT = 15;

    public static readonly int CLOSE_THRESHOLD = 10;


    public Map(String loc)
    {
        pixelMap = (SDL.SDL_Surface*) SDL.SDL_LoadBMP(Engine.GetAssetPath(loc));

        SDL.SDL_LockSurface((IntPtr) pixelMap);

        int pitch = (*pixelMap).pitch;

        w = (*pixelMap).w;
        h = (*pixelMap).h;

        pixels = new int[w,h];
        transitions = new List<int>[w];
        for(int i = 0; i < w; i++)
        {
            transitions[i] = new List<int>();
        }
        
        int bpp = 4;
        byte* pixelsImg = (byte*)(*pixelMap).pixels;


        for (int x = 0; x < pixels.GetLength(0); x++)
        {
            for (int y = 0; y < pixels.GetLength(1); y++)
            {
                pixels[x, y] = *(pixelsImg + pitch * y + bpp * x) + *(pixelsImg + pitch * y + bpp * x + 2);
                // rings: 255, 0, 245
                //Console.WriteLine(*(pixelsImg + pitch * y + bpp * x + 2));
                Vector2 locVect = new Vector2(x, y);
                if (pixels[x, y] == (245 + 255))
                {
                    Game.flowers.Add(new Flower(locVect));
                    pixels[x, y] = AIR_CODE;
                }

                Bounds2 testPath = new Bounds2(new Vector2(locVect.X - 100, 0), new Vector2(locVect.X + 100, 0));
                if (pixels[x, y] == 5)
                {
                    Game.enemies.Add(new Enemy(locVect, testPath, false));
                    pixels[x, y] = AIR_CODE;
                }

                if (pixels[x, y] == 25)
                {
                    Game.enemies.Add(new Enemy(locVect, testPath, true));
                    pixels[x, y] = AIR_CODE;
                }

                if (y > 0 && pixels[x, y] != pixels[x, y - 1])
                {
                    if (pixels[x, y - 1] == AIR_CODE)
                        transitions[x].Add(y);
                    else
                        transitions[x].Add(y - 1);
                }
            }
        }

        SDL.SDL_UnlockSurface((IntPtr) pixelMap);
        SDL.SDL_FreeSurface((IntPtr) pixelMap);
    }

    //gets the pixel type at the given coordinate
    public int getPixelType(Vector2 coord)
    {
        int x = (int) Math.Round(coord.X);
        int y = (int) Math.Round(coord.Y);
        if(x >= w || x < 0 || y >= h || y < 0)
        {
            return AIR_CODE;
        }
        return pixels[x,y];
    }

    public bool onGround(Vector2 coord)
    {
        return getPixelType(coord) == GROUND_CODE || getPixelType(coord) == SOLID_CODE;
    }

    public bool impenetrable(Vector2 coord)
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

    public bool closeToSurface(Vector2 coord)
    {
        return Math.Abs(getSurfaceY(coord) - coord.Y) < CLOSE_THRESHOLD;
    }

    //getting slope angle
    // TODO: remember to account for cases where the ground is above or below and just general edge cases like vert
    public double getSlopeAngle(Vector2 coord)
    {
        if(getPixelType(coord) == AIR_CODE)
        {
            return 0;
        }

        Vector2 leftShift = new Vector2(-2, 0);
        Vector2 rightShift = new Vector2(2, 0);

        if (coord.X >= 2)
        {
            
            int currType = getPixelType(coord + leftShift);
            for(int i = 0; i < 2 * SLOPE_MAX_COUNT; i++)
            {
                leftShift.Y = (i + 2) / 2 * (1 - 2 * (i % 2));
                if (getPixelType(leftShift + coord) != currType)
                {
                    break;
                }
            }
        }
        else
        {
            leftShift.Y = -1234;
            leftShift.X = 0;
        }

        if(coord.X < w - 2)
        {
            int currType = getPixelType(coord + rightShift);
            for (int i = 0; i < 2 * SLOPE_MAX_COUNT; i++)
            {
                rightShift.Y = (i + 2) / 2 * (1 - 2 * (i % 2));
                if (getPixelType(rightShift + coord) != currType)
                {
                    break;
                }
            }
        }
        else
        {
            rightShift.Y = 1234;
            rightShift.X = 0;
        }

        if(rightShift.Y - leftShift.Y > SLOPE_MAX_COUNT * 2)
        {
            if(getPixelType(coord - new Vector2(1,0)) == AIR_CODE)
            {
                return Math.PI / 2;
            }
            return 3 * Math.PI / 2;
        }

        return Math.Atan((double) (rightShift.Y - leftShift.Y) / (rightShift.X - leftShift.X));


    }


    //need to fix for LOTS of edge cases
    public Vector2 getNormalVector(Vector2 pos)
    {
        Vector2 slope = new Vector2(10,0);
        slope.Y = getSurfaceY(pos + slope / 2) - getSurfaceY(pos - slope / 2);

        return slope.Rotated(270).Normalized();


        return new Vector2(0, -1);
    }

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

    public int getSurfaceY(Vector2 pos)
    {
        List<int> currTransitions = transitions[(int) pos.X];

        for (int i = 0; i < currTransitions.Count; i++)
        {
            if(i % 2 == 0)
            {
                if (i + 1 >= currTransitions.Count || (pos.Y >= currTransitions[i] && pos.Y <( currTransitions[i] + currTransitions[i + 1]) / 2))
                {
                    return currTransitions[i];
                }
                //air case
                if (currTransitions[i] > pos.Y)
                {
                    return currTransitions[i];
                }
            }
            //upside down ground case
            if(i % 2 != 0)
            {
                if(pos.Y  <= currTransitions[i] && pos.Y >= (currTransitions[i] + currTransitions[i-1]) / 2)
                {
                    return currTransitions[i];
                }
            }
        }
        return -1;



        /*
        if(currTransitions.Count == 1)
        {
            return currTransitions[0];
        }

        int i = 1;
        while(i < currTransitions.Count && currTransitions[i] <= pos.Y)
        {
            i += 2;
        }
        if (i >= 2 && i >= currTransitions.Count)
        {
            return currTransitions[i - 2];
        }
        if (currTransitions[i] <= pos.Y)
        {
            return currTransitions[i];
        }
        return currTransitions[Math.Max(0,i - 2)];*/
    }
}