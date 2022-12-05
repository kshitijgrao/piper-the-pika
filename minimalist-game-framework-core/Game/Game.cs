using System;
using System.Collections.Generic;
using System.Collections;

class Game
{
    public static readonly string Title = "Piper the Pika";
    public static readonly Vector2 Resolution = new Vector2(320, 224);
    public static Map map;


    readonly Texture piperTexture = Engine.LoadTexture("pika-spritemap-no-dots.png");

    // sprites
    Sprite piper;
    float piperFrameIndex;
    ArrayList sprites = new ArrayList();


    Scoreboard sb;
    Cutscenes scene;
    float speed = 2;


    Font arial = Engine.LoadFont("Arial.ttf", 10);

    public Game()
    {
        //scoreboard
        sb = new Scoreboard();
        scene = new Cutscenes();


        // create piper sprite
        piper = new Sprite(Resolution / 2, piperTexture);
        piperFrameIndex = 0;
        sprites.Add(piper);

    }

    public void Update()
    {
        if (sb.getTime() < 0)
        {
            scene.titleScene();
        }
        
        piperFrameIndex = Animator.animatePiper(piper, speed, piperFrameIndex);
        sb.updateScoreboard();
        
    }
}
