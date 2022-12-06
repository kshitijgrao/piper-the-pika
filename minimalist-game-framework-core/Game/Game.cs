using System;
using System.Collections.Generic;
using System.Collections;

class Game
{
    public static readonly string Title = "Piper the Pika";
    public static readonly Vector2 Resolution = new Vector2(320, 224);
    public static Map map;
    public Boolean startScene;
    public Boolean endScene;


    readonly Texture piperTexture = Engine.LoadTexture("pika-spritemap-no-dots.png");

    // sprites
    Sprite piper;
    float piperFrameIndex;
    ArrayList sprites = new ArrayList();


    Scoreboard sb;
    Scenes scene;
    float speed = 2;


    Font arial = Engine.LoadFont("Arial.ttf", 10);

    public Game()
    {
        //scene control
        startScene = true;
        endScene = false;

        //scoreboard
        sb = new Scoreboard();
        scene = new Scenes();


        //create piper sprite
        piper = new Sprite(Resolution / 2, piperTexture);
        piperFrameIndex = 0;
        sprites.Add(piper);

    }

    public void Update()
    {
        if (startScene){ startScene = scene.titleScene();}
        else if (endScene){scene.endScene();}
        else
        {
            piperFrameIndex = Animator.animatePiper(piper, speed, piperFrameIndex);
            sb.updateScoreboard();
        }
      
        
        
        
    }
}
