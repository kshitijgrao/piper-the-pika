using System;
using System.Collections.Generic;
using System.Text;

class Rendering
{
    Texture map; //1944 x 1172
    Vector2 pos;
    Texture charTexture;
    Sprite character;
    Vector2 center;

    public Rendering(String texture, String charSprite, Vector2[] hitbox)
    {
        pos = new Vector2(0, 0);
        map = Engine.LoadTexture(texture);
        center = new Vector2(Game.Resolution.X / 2, Game.Resolution.Y / 2);
        charTexture = Engine.LoadTexture(charSprite);
        character = new Sprite(center, charTexture, hitbox);
    }

    public void scrollingWindow()
    {
        Engine.DrawTexture(map, pos);
        character.draw();

        

        if (Engine.GetKeyHeld(Key.Right))
        {
            if (character.loc.X < Game.Resolution.X * 0.8)
            {
                character.loc.X += Engine.TimeDelta * 200;
            } else if (pos.X > Game.Resolution.X - map.Width)
            {
                pos.X -= Engine.TimeDelta * 200;
            }
        }

        if (Engine.GetKeyHeld(Key.Left))
        {
            if (character.loc.X > Game.Resolution.X * 0.2)
            {
                character.loc.X -= Engine.TimeDelta * 200;
            } else if (pos.X < 0)
            {
                pos.X += Engine.TimeDelta * 200;
            }
        }

        if (Engine.GetKeyHeld(Key.Down) && pos.Y > Game.Resolution.Y - map.Height)
        {
            if (character.loc.Y < Game.Resolution.Y * 0.8)
            {
                character.loc.Y += Engine.TimeDelta * 200;
            } else if (pos.Y > Game.Resolution.Y - map.Height)
            {
                pos.Y -= Engine.TimeDelta * 200;
            }
        }

        if (Engine.GetKeyHeld(Key.Up) && pos.Y < 0)
        {
            if (character.loc.Y > Game.Resolution.Y * 0.2)
            {
                character.loc.Y -= Engine.TimeDelta * 200;
            } else if (pos.Y < 0)
            {
                pos.Y += Engine.TimeDelta * 200;
            }
        }
    }

}