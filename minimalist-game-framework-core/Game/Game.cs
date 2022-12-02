using System;
using System.Collections.Generic;
using System.Collections;

class Game
{
    public static readonly string Title = "Piper the Pika";
    public static readonly Vector2 Resolution = new Vector2(320, 224);
    readonly Texture piperTexture = Engine.LoadTexture("pika-spritemap-no-dots.png");

    // sprites
    Sprite piper;
    float piperFrameIndex;
    ArrayList sprites = new ArrayList();

    Scoreboard sb;
    float speed = 2;



    public Game()
    {
        //scoreboard
        sb = new Scoreboard();

        // create piper sprite
        piper = new Sprite(Resolution / 2, piperTexture);
        piperFrameIndex = 0;
        sprites.Add(piper);
    }

    public void Update()
    {
        piperFrameIndex = Animator.animatePiper(piper, speed, piperFrameIndex);
        sb.updateScoreboard();
    }
}
