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

    public static readonly string RIGHT = "right";
    public static readonly string LEFT = "left";

    readonly Texture piperTexture = Engine.LoadTexture("pika-spritemap.png");

    // sprites
    public static Sonic piper;
    Sprite[] sprites = new Sprite[1];

    Scoreboard sb;
    Font arial = Engine.LoadFont("Arial.ttf", 10);
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
        map = new Map("TestMap.bmp");

        // create piper sprite
        piper = new Sonic(new Vector2(160, 960), piperTexture);
        sprites[0] = piper;
        scroll = new Rendering("TestMap.bmp", new Bounds2(3 * Game.Resolution.X / 8, Game.Resolution.Y / 4, Game.Resolution.X / 4, Game.Resolution.Y / 2));

    }

    public void Update()
    {
        //scene control
        if (startScene) { startScene = Scenes.titleScene(); }
        else if (endScene) { Scenes.endScene(); }
        else
        {
            //collision detection
            Physics.detectGround(piper);

            //update acceleration
            piper.setAcceleration(currentKey);

            //update overall physics
            Physics.updatePhysics(sprites);

            // collect input and draw frame
            scroll.scrollingWindow();
            currentKey = InputHandler.getPlayerInput(piper, scroll.pos + piper.loc - new Vector2(12, 12));
            sb.updateScoreboard();

        }
    }
}