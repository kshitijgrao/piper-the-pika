using System;
using System.Collections.Generic;
using System.Collections;

class Game
{
    public static readonly string Title = "Piper the Pika";
    public static readonly Vector2 Resolution = new Vector2(320, 224);
    Texture piperTexture = Engine.LoadTexture("pika-with-dots");
    Scoreboard sb;
    Sprite piper;
    ArrayList sprites = new ArrayList();

    public Game()
    {
        //scoreboard
        sb = new Scoreboard();

        // create piper sprite
        Vector2[] piperHB = new Vector2[1];
        piperHB[0] = new Vector2(24, 24);
        piper = new Sprite(Resolution / 2, piperTexture, piperHB);
        sprites.Add(piper);
    }

    public void Update()
    {
        // animate each sprite onscreen
        foreach (Sprite s in sprites)
        {
            Animator.draw(s);
        }

        sb.updateScoreboard();
    }
}
