using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SDL2;
using static SDL2.SDL;
using System.Reflection;

unsafe class Map {

    public int[,] pixels;
    SDL.SDL_Surface* pixelMap;
    private int w;
    private int h;

    //TODO: come up with color key for waht the different types of blocks represent
    public static readonly int AIR_CODE = 0;


    private static readonly int SLOPE_MAX_COUNT = 15;


    public Map(String loc)
    {
        pixelMap = (SDL.SDL_Surface*) SDL.SDL_LoadBMP(loc);

        SDL.SDL_FreeSurface((IntPtr) pixelMap);
        SDL.SDL_LockSurface((IntPtr) pixelMap);

        int pitch = (*pixelMap).pitch;

        w = (*pixelMap).w;
        h = (*pixelMap).h;

        pixels = new int[w,h];
        
        int bpp = 3;
        byte* pixelsImg = (byte*)pixelMap->pixels;
        
        for (int x = 0; x < pixels.GetLength(0); x++)
        {
            for(int y = 0; y < pixels.GetLength(1); y++)
            {
                pixels[x, y] = *(pixelsImg + pitch * y + bpp * x);
            }
        }

        SDL.SDL_UnlockSurface((IntPtr) pixelMap);    
    }

    //gets the pixel type at the given coordinate
    public int getPixelType(Vector2 coord)
    {
        return pixels[(int) coord.X,(int) coord.Y];
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

}

