using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

class Game
{
    public static readonly string Title = "Piper the Pika";
    public static readonly Vector2 Resolution = new Vector2(320, 224);
    public static List<Vector2> flowerCoords = new List<Vector2>();
    public static Map map;
    public int startScene; //0 = false, 1 = true, 2 = instructions
    public Boolean endScene;
    public static Sprite[] flowers;

    public static readonly string RIGHT = "right";
    public static readonly string LEFT = "left";

    
    readonly Texture piperTexture = Engine.LoadTexture("pika-spritemap.png");
    readonly Texture wolfTexture = Engine.LoadTexture("wolf-enemy-spritemap.png");
    readonly Texture hawkTexture = Engine.LoadTexture("hawk-enemy-spritemap.png");

    // sprites
    public static Sonic piper;
    public static Enemy wolf;
    public static Enemy hawk;
    Sprite[] sprites = new Sprite[1];
    ArrayList enemiesOnScreen = new ArrayList();

    public static Scoreboard sb;

    Font arial = Engine.LoadFont("Arial.ttf", 10);
    Rendering render;
    Vector2 pos;
    Key currentKey = Key.Q; // defaults to unused key "Q"

    public Game()
    {
        //scene control
        startScene = 1;
        endScene = true;



        //scoreboard
        sb = new Scoreboard();

        //create map
        map = new Map("RingEnemyMap.bmp");
        flowers = new Flower[flowerCoords.Count];

        // create piper sprite
        piper = new Sonic(new Vector2(160, 960), piperTexture, new Vector2(24, 24));
        sprites[0] = piper;
        //sprites.Add(piper);

        // TESTING ENEMIES
        Bounds2 testPath = new Bounds2(new Vector2(100, 0), new Vector2(200, 0));
        wolf = new Enemy(new Vector2(150, 960), wolfTexture, new Vector2(40, 34), testPath);
        hawk = new Enemy(new Vector2(160, 900), hawkTexture, new Vector2(54, 37), testPath);
        wolf.setState(1);
        hawk.setState(1);
        enemiesOnScreen.Add(wolf);
        enemiesOnScreen.Add(hawk);

        for (int i = 0; i < flowers.Length; i++)
        {
            flowers[i] = new Flower(flowerCoords[i]);
        }
        render = new Rendering("TestMapPng.png", new Bounds2(3 * Game.Resolution.X / 8, Game.Resolution.Y / 4, Game.Resolution.X / 4, Game.Resolution.Y / 2));
    }

    public void Update()
    {
        //scene control
        if (startScene==2)
        {
            startScene = Scenes.instructionsScene();
        }
        else if (startScene==1) { startScene = Scenes.titleScene(); }
        else if (endScene) {Scenes.endScene(); }
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
            foreach (Enemy enemy in enemiesOnScreen)
            {
                enemy.updateState();
                enemy.setFrameIndex(Animator.animateEnemy(enemy, render.pos + enemy.loc - new Vector2(12, 13)));
            };
            piper.setFrameIndex(Animator.animatePiper(piper, render.pos + piper.loc - new Vector2(12, 12), currentKey));
            //rings[0].draw(new Bounds2(0, 0, 24, 24), render.pos + rings[0].loc - new Vector2(10,10));
            sb.updateScoreboard();
        }
    }
}