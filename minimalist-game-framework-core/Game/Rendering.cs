using System;
using System.Collections.Generic;
using System.Text;

class Rendering
{
    public Texture map; //1944 x 1172
    public Vector2 pos;
    private Vector2 center;

    public Rendering(String texture)
    {
        pos = new Vector2(60, -838);
        map = Engine.LoadTexture(texture);
        center = new Vector2(Game.Resolution.X / 2, Game.Resolution.Y / 2);
    }

    public void scrollingWindow()
    {
        Engine.DrawTexture(map, pos);

        Vector2 onScreenCoord = Game.piper.loc + pos;

        if (onScreenCoord.X > Game.Resolution.X * 0.8)
        {
            pos.X -= Engine.TimeDelta * Game.piper.vel.X;
        }
        
        if (onScreenCoord.X < Game.Resolution.X * 0.2)
        {
            pos.X += Engine.TimeDelta * Game.piper.vel.X;
        }

        if (onScreenCoord.Y > Game.Resolution.Y * 0.8)
        {
            pos.Y -= Engine.TimeDelta * Game.piper.vel.Y;
        }

        if (onScreenCoord.Y < Game.Resolution.Y * 0.2)
        {
            pos.Y += Engine.TimeDelta * Game.piper.vel.Y;
        }
    }

}