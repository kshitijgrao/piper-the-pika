using System;
using System.Collections.Generic;
using System.Text;

class Rendering
{
    public Texture map; //1944 x 1172
    public Texture bg;
    public Vector2 pos;
    private Vector2 center;
    private Bounds2 window;

    public Rendering(String texture, String bgText, Bounds2 window)
    {
        pos = new Vector2(0, -838);
        map = Engine.LoadTexture(texture);
        bg = Engine.LoadTexture(bgText);
        center = new Vector2(Game.Resolution.X / 2, Game.Resolution.Y / 2);
        this.window = window;
    }

    public void scrollingMotion()
    {
        Engine.DrawTexture(bg, (pos / 2) - new Vector2(0, 420));
        Engine.DrawTexture(map, pos);

        foreach (Flower flow in Game.flowers)
        {
            flow.draw(new Bounds2(0, 0, 13, 14), flow.loc + pos);
        }

        for (int i = 0; i < Game.enemies.Count; i++)
        {
            Vector2 enemyPos = Game.enemies[i].loc + pos;

            if (enemyPos.X < Game.Resolution.X && enemyPos.Y < Game.Resolution.Y)
            {
                if (!Game.enemiesOnScreen.Contains(Game.enemies[i]))
                {
                    Game.enemiesOnScreen.Add(Game.enemies[i]);
                }
            } else if (Game.enemiesOnScreen.Contains(Game.enemies[i]))
            {
                Game.enemiesOnScreen.Remove(Game.enemies[i]);
            }
        }

        Vector2 onScreenCoord = Game.piper.loc + pos;

        if (onScreenCoord.X > window.Max.X && Game.piper.vel.X > 0)
        {
            pos.X -= Engine.TimeDelta * Game.piper.vel.X;
        }
        
        if (onScreenCoord.X < window.Min.X && Game.piper.vel.X < 0)
        {
            pos.X -= Engine.TimeDelta * Game.piper.vel.X;
        }

        if (onScreenCoord.Y < window.Min.Y && Game.piper.vel.Y < 0)
        {
            pos.Y -= Engine.TimeDelta * Game.piper.vel.Y;
        }

        if (onScreenCoord.Y > window.Max.Y && Game.piper.vel.Y > 0)
        {
            pos.Y -= Engine.TimeDelta * Game.piper.vel.Y;
        }
    }

}