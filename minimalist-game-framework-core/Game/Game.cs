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
    public static Sonic piper;

    public static Scoreboard sb;

    Font arial = Engine.LoadFont("Arial.ttf", 10);

    Sprite[] sprites = new Sprite[1];
    Sprite[] rings = new Flower[1];

    Rendering scroll;
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
        map = new Map("rasterizedMap.bmp");

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
            currentKey = InputHandler.getPlayerInput(piper, scroll.pos + piper.loc - new Vector2(12, 12));
            
            
            //collision detection
            Physics.detectGround(piper);
            Physics.detectUnpenetrable(piper);
            //ground
            Physics.detectCollisions(rings);

            //update acceleration
            piper.setAcceleration(currentKey);

            //update overall physics
            Physics.updatePhysics(sprites);

            // collect input and draw frame
            scroll.scrollingWindow();
            

            piper.setFrameIndex(Animator.animatePiper(piper, scroll.pos + piper.loc - new Vector2(12, 12), currentKey));
            rings[0].draw(new Bounds2(0, 0, 24, 24), scroll.pos + rings[0].loc - new Vector2(10,10));
            sb.updateScoreboard();

        }
    }
}