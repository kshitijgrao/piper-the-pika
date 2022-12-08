using System;
using System.Collections.Generic;
using System.Collections;

class Game
{
    public static readonly string Title = "Piper the Pika";
    public static readonly Vector2 Resolution = new Vector2(320, 224);
    public static ArrayList flowerCoords = new ArrayList();
    public static Map map;
    public Boolean startScene;
    public Boolean endScene;

    public static readonly string RIGHT = "right";
    public static readonly string LEFT = "left";


    readonly Texture piperTexture = Engine.LoadTexture("pika-spritemap.png");

    // sprites
    //float piperFrameIndex;

    public static Sonic piper;


    public static Scoreboard sb;

    Font arial = Engine.LoadFont("Arial.ttf", 10);

    Sprite[] sprites = new Sprite[1];
    Sprite[] rings = new Flower[1];

    Rendering scroll;
    //Texture bg;
    Vector2 pos;

    public Game()
    {
        //scene control
        startScene = true;
        endScene = false;

        //scoreboard
        sb = new Scoreboard();

        //create map
        map = new Map("TestMap.bmp");

        //bg = Engine.LoadTexture("TestMap.bmp");


        // create piper sprite
        piper = new Sonic(new Vector2(160, 960), piperTexture);
        sprites[0] = piper;
        //sprites.Add(piper);
        for(int i =0; i < rings.Length; i++)
        {
            rings[i] = new Flower(new Vector2(324,962));
        }
        scroll = new Rendering("TestMap.bmp", new Bounds2(3 * Game.Resolution.X / 8, Game.Resolution.Y / 4, Game.Resolution.X / 4, Game.Resolution.Y / 2));

    }

    public void Update()
    {
        //scene control
        if (startScene) { startScene = Scenes.titleScene(); }
        else if (endScene) { Scenes.endScene(); }
        else
        {
            //getting input (need to adjust this to work generally, this is just for testing)
            String currKey = "None";
            if (Engine.GetKeyHeld(Key.Right))
            {
                currKey = Game.RIGHT;
            }
            else if (Engine.GetKeyHeld(Key.Left))
            {
                currKey = Game.LEFT;
            }

            //update velocity (mainly jump velocity)
            if (Engine.GetKeyDown(Key.Up) || Engine.GetKeyDown(Key.W) || Engine.GetKeyDown(Key.Z) || Engine.GetKeyHeld(Key.Up) || Engine.GetKeyHeld(Key.W) || Engine.GetKeyHeld(Key.Z))
            {
                piper.jump();
            } 

            //collision detection
            Physics.detectGround(piper);
            //ground
            Physics.detectCollisions(rings);

            //update acceleration
            piper.setAcceleration(currKey);

            //update overall physics
            Physics.updatePhysics(sprites);


            //Engine.DrawTexture(bg, new Vector2(0, 0));

            scroll.scrollingWindow();

            //replace this with proper drawing with rao/yasemin's rendering/animation system
            //piper.testDraw();
            piper.draw(new Bounds2(0, 0, 24, 24), scroll.pos + piper.loc - new Vector2(12, 12));
            rings[0].draw(new Bounds2(0, 0, 24, 24), scroll.pos + rings[0].loc - new Vector2(10,10));

            //piperFrameIndex = Animator.animatePiper(piper, speed, piperFrameIndex);
            sb.updateScoreboard();

        }
    }
}