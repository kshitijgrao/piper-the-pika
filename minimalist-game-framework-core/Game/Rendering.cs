using System;
using System.Collections.Generic;
using System.Text;

public class Rendering
{
    Texture map; //1944 x 1172
    Vector2 pos;


    public Rendering(String texture)
    {
        pos = new Vector2(0, 0);
        map = Engine.LoadTexture(texture);
    }

    public void scrollingWindow()
    {
        Engine.DrawTexture(map, pos);

        if (Engine.GetKeyHeld(Key.Right) && pos.X > Game.Resolution.X - map.Width)
        {
            pos.X -= Engine.TimeDelta * 200;
        }

        if (Engine.GetKeyHeld(Key.Left) && pos.X < 0)
        {
            pos.X += Engine.TimeDelta * 200;
        }

        if (Engine.GetKeyHeld(Key.Down) && pos.Y > Game.Resolution.Y - map.Height)
        {
            pos.Y -= Engine.TimeDelta * 200;
        }

        if (Engine.GetKeyHeld(Key.Up) && pos.Y < 0)
        {
            pos.Y += Engine.TimeDelta * 200;

        }

    }

}