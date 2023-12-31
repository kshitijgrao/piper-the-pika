﻿using System;
using System.Collections.Generic;
using System.Text;

class Rendering
{
    public Chunking map; //1944 x 1172
    public Chunking bg;
    public Vector2 pos;
    private Vector2 center;
    private Bounds2 window;
    private Vector2 bgOff;
    private Vector2 mapOff;

    public static readonly Bounds2 defaultWindow = new Bounds2(15 * Game.Resolution.X / 32, Game.Resolution.Y / 3, Game.Resolution.X / 16, Game.Resolution.Y / 3);

    public Rendering(Chunking mapSplit, Chunking bgSplit, Bounds2 window)
    {
        map = mapSplit;
        bg = bgSplit;
        center = new Vector2(Game.Resolution.X / 2, Game.Resolution.Y / 2);
        this.window = window;
    }

    public Rendering(Chunking mapSplit, Chunking bgSplit, Vector2 pos, Vector2 bgOff, Vector2 mapOff) : this(mapSplit, bgSplit, defaultWindow)
    {
        // pos = new Vector2(0, -838)
        this.pos = pos;
        this.bgOff = bgOff;
        this.mapOff = mapOff;
    }

    public void scrollingMotion(Sonic piper)
    {
        // bgOff = - new Vector2(2382, 420)
        // mapOff = - new Vector2(893, 0)
        bg.draw((pos / 2) - bgOff);
        map.draw(pos - mapOff);

 /*       for (int i = 0; i < Game.currentLevel.enemies.Length; i++)
        {
            Vector2 enemyPos = Game.currentLevel.enemies[i].loc + pos;

            if (enemyPos.X < Game.Resolution.X && enemyPos.Y < Game.Resolution.Y)
            {
                if (!Game.enemiesOnScreen.Contains(Game.currentLevel.enemies[i]))
                {
                    Game.enemiesOnScreen.Add(Game.currentLevel.enemies[i]);
                }
            } else if (Game.enemiesOnScreen.Contains(Game.currentLevel.enemies[i]))
            {
                Game.enemiesOnScreen.Remove(Game.currentLevel.enemies[i]);
            }
        }*/


        Vector2 onScreenCoord = piper.loc + pos;

        if(!(new Bounds2(Vector2.Zero, Game.Resolution)).Contains(onScreenCoord))
        {
            pos = Game.Resolution / 2 - piper.loc;
        }

        if (onScreenCoord.X > window.Max.X && piper.vel.X > 0)
        {
            pos.X -= Engine.TimeDelta * piper.vel.X;
        }
        
        if (onScreenCoord.X < window.Min.X && piper.vel.X < 0)
        {
            pos.X -= Engine.TimeDelta * piper.vel.X;
        }

        if (onScreenCoord.Y < window.Min.Y && piper.vel.Y < 0)
        {
            pos.Y -= Engine.TimeDelta * piper.vel.Y;
        }

        if (onScreenCoord.Y > window.Max.Y && piper.vel.Y > 0)
        {
            pos.Y -= Engine.TimeDelta * piper.vel.Y;
        }
    }

}