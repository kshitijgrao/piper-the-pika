using System;
using System.Collections.Generic;
using System.Collections;

class Game
{
    public static readonly string Title = "Piper the Pika";
    public static readonly Vector2 Resolution = new Vector2(320, 224);
    public static Map map;

    public static readonly string RIGHT = "right";
    public static readonly string LEFT = "left";

    readonly Texture piperTexture = Engine.LoadTexture("pika-spritemap-no-dots.png");

    // sprites
    Sprite piper;
    float piperFrameIndex;
    ArrayList sprites = new ArrayList();


    Scoreboard sb;
    float speed = 2;


    Font arial = Engine.LoadFont("Arial.ttf", 10);

    public Game()
    {
        //scoreboard
        sb = new Scoreboard();
        map = new Map("C:/Users/evane/source/repos/recreate-a-classic-game-sonic-yeer/minimalist-game-framework-core/Assets/testing_query.bmp");


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
