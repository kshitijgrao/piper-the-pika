using System;
using System.Collections.Generic;
using System.Text;

public class Rendering
{
    Texture map = Engine.LoadTexture("map.jpg"); //1944 x 1172
    Vector2 pos = new Vector2(0, 0);


    public Rendering()
    {

    }

    public void scrollingWindow()
    {
        Engine.DrawTexture(map, pos);

        if (Engine.GetKeyHeld(Key.Right) && pos.X > -1304)
        {
            pos.X -= Engine.TimeDelta * 200;
        }

        if (Engine.GetKeyHeld(Key.Left) && pos.X < 0)
        {
            pos.X += Engine.TimeDelta * 200;
        }

        if (Engine.GetKeyHeld(Key.Down) && pos.Y > -692)
        {
            pos.Y -= Engine.TimeDelta * 200;
        }

        if (Engine.GetKeyHeld(Key.Up) && pos.Y < 0)
        {
            pos.Y += Engine.TimeDelta * 200;

        }

    }

}