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


    public Map(String loc)
    {
        pixelMap = (SDL.SDL_Surface*) SDL.SDL_LoadBMP(loc);

        SDL.SDL_FreeSurface((IntPtr) pixelMap);
        SDL.SDL_LockSurface((IntPtr) pixelMap);

        int pitch = (*pixelMap).pitch;

        int w = (*pixelMap).w;
        int h = (*pixelMap).h;

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
}

