using System;
using System.Collections.Generic;
using System.Collections;

class Game
{
    public static readonly string Title = "Piper the Pika";
    public static readonly Vector2 Resolution = new Vector2(320, 224);
    public static List<Vector2> flowerCoords = new List<Vector2>();
    public static Map map;
    public Boolean startScene;
    public Boolean endScene;
    public static Sprite[] flowers;

    public static readonly string RIGHT = "right";
    public static readonly string LEFT = "left";


    readonly Texture piperTexture = Engine.LoadTexture("pika-spritemap.png");

    // sprites
    public static Sonic piper;

    public static Scoreboard sb;

    Font arial = Engine.LoadFont("Arial.ttf", 10);

    Sprite[] sprites = new Sprite[1];

    Rendering render;
    Vector2 pos;
    Key currentKey = Key.Q; // defaults to unused key "Q"

    public Game()
    {
        //scene control
        startScene = true;
        endScene = false;

        //scoreboard
        sb = new Scoreboard();

        //create map
        map = new Map("RingEnemyMap.bmp");
        flowers = new Flower[flowerCoords.Count];

        // create piper sprite
        piper = new Sonic(new Vector2(160, 960), piperTexture);
        sprites[0] = piper;
        //sprites.Add(piper);
        for (int i = 0; i < flowerCoords.Count; i++)
        {
            flowers[i] = new Flower(flowerCoords[i]);
        }
        render = new Rendering("TestMapPng.png", new Bounds2(3 * Game.Resolution.X / 8, Game.Resolution.Y / 4, Game.Resolution.X / 4, Game.Resolution.Y / 2));

    }

    public void Update()
    {
        //scene control

        if (startScene) { startScene = Scenes.titleScene(); }
        else if (endScene) { Scenes.endScene(); }
        else
        {
            currentKey = InputHandler.getPlayerInput(piper, render.pos + piper.loc - new Vector2(12, 12));
            
            
            //collision detection
            Physics.detectGround(piper);
            Physics.detectUnpenetrable(piper);
            //ground
            Physics.detectCollisions(flowers);

            //update acceleration
            piper.setAcceleration(currentKey);

            //update overall physics
            Physics.updatePhysics(sprites);

            // collect input and draw frame
            render.scrollingMotion();
            

            piper.setFrameIndex(Animator.animatePiper(piper, render.pos + piper.loc - new Vector2(12, 12), currentKey));
            //flowers[0].draw(new Bounds2(0, 0, 24, 24), scroll.pos + flowers[0].loc - new Vector2(10,10));
            sb.updateScoreboard();
            

        }
    }
}