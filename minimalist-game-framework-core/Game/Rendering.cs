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

    public static readonly Bounds2 defaultWindow = new Bounds2(7 * Game.Resolution.X / 16, Game.Resolution.Y / 3, Game.Resolution.X / 8, Game.Resolution.Y / 3);

    public Rendering(String texture, String bgText, Bounds2 window)
    {
        pos = new Vector2(0, -838);
        map = Engine.LoadTexture(texture);
        bg = Engine.LoadTexture(bgText);
        center = new Vector2(Game.Resolution.X / 2, Game.Resolution.Y / 2);
        this.window = window;
    }

    public Rendering(string texture, string bgText) : this(texture, bgText, defaultWindow)
    {

    }

    public void scrollingMotion(Sonic piper)
    {
        Engine.DrawTexture(bg, (pos / 2) - new Vector2(2382, 420));
        Engine.DrawTexture(map, pos - new Vector2(893, 0));

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