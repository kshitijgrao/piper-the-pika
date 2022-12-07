﻿using System;
using System.Collections.Generic;
using System.Collections;

class Game
{
    public static readonly string Title = "Piper the Pika";
    public static readonly Vector2 Resolution = new Vector2(320, 224);
    public static Map map;
    public Boolean startScene;
    public Boolean endScene;

    public static readonly string RIGHT = "right";
    public static readonly string LEFT = "left";

    readonly Texture piperTexture = Engine.LoadTexture("pika-spritemap-no-dots.png");

    // sprites
    //float piperFrameIndex;
    


    Sonic piper;


    Scoreboard sb;
    Scenes scene;
    float speed = 2;
    float speed = 2;



    Font arial = Engine.LoadFont("Arial.ttf", 10);

    Sprite[] sprites = new Sprite[1];

    Texture bg;

    public Game()
    {
        //scene control
        startScene = true;
        endScene = false;

        //scoreboard
        sb = new Scoreboard();
        
        //new scene
        scene = new Scenes();


        //create piper sprite
        piper = new Sprite(Resolution / 2, piperTexture);
        piperFrameIndex = 0;
        sprites.Add(piper);

        //create map
        map = new Map("TestMap.bmp");
        bg = Engine.LoadTexture("TestMap.bmp");


        // create piper sprite
        piper = new Sonic(new Vector2(30,129), piperTexture);
        sprites[0] = piper;
        //sprites.Add(piper);


    }

    public void Update()
    {
        //scene control
        if (startScene){ startScene = scene.titleScene();}
        else if (endScene){scene.endScene();}
        else
        {
            piperFrameIndex = Animator.animatePiper(piper, speed, piperFrameIndex);
            sb.updateScoreboard();
        }
      
        

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
        piper.testDraw();



        //piperFrameIndex = Animator.animatePiper(piper, speed, piperFrameIndex);
        //sb.updateScoreboard();
    }
}
