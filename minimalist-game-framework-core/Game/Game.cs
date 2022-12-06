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
    //float piperFrameIndex;
    


    Sonic piper;


    Scoreboard sb;
    //float speed = 2;


    Font arial = Engine.LoadFont("Arial.ttf", 10);

    Sprite[] sprites = new Sprite[1];

    Texture bg;

    public Game()
    {
        //scoreboard
        sb = new Scoreboard();
        map = new Map("C:/Users/evane/source/repos/recreate-a-classic-game-sonic-yeer/minimalist-game-framework-core/Assets/bruhbruh.bmp");
        bg = Engine.LoadTexture("bruhbruh.bmp");


        // create piper sprite
        piper = new Sonic(new Vector2(30,129), piperTexture);
        sprites[0] = piper;
        //sprites.Add(piper);

    }

    public void Update()
    {
        //getting input (need to adjust this to work generally, this is just for testing)
        String currKey = "None";
        if (Engine.GetKeyHeld(Key.Right))
        {
            currKey = Game.RIGHT;
        }
        else if(Engine.GetKeyHeld(Key.Left))
        {
            currKey = Game.LEFT;
        }

        //update velocity (mainly jump velocity)
        if (Engine.GetKeyDown(Key.Z))
        {
            piper.jump();
        }

        //collision detection
        Physics.detectGround(piper);

        //update acceleration
        piper.setAcceleration(currKey);

        //update overall physics
        Physics.updatePhysics(sprites);


        Engine.DrawTexture(bg, new Vector2(0, 0));
        
        //replace this with proper drawing with rao/yasemin's rendering/animation system
        piper.draw();



        //piperFrameIndex = Animator.animatePiper(piper, speed, piperFrameIndex);
        //sb.updateScoreboard();
    }
}
