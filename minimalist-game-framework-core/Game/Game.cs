using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;

class Game
{
    public static readonly string Title = "Piper the Pika";
    public static readonly Vector2 Resolution = new Vector2(320, 224);
    //public static List<Vector2> enemyCoords = new List<Vector2>();
    public static Map map;
    public int startScene; //0 = false, 1 = true, 2 = instructions
    public static Boolean endScene;
    public static String message = "PASSED";
    public static List<Flower> flowers = new List<Flower>();
    public static List<Enemy> enemies = new List<Enemy>();

    public static Flower[] flowerArr;
    public static Enemy[] enemyArr;

    public static readonly string RIGHT = "right";
    public static readonly string LEFT = "left";


    readonly Texture piperTexture = Engine.LoadTexture("piper-spritemap-no-border.png");
    readonly Texture wolfTexture = Engine.LoadTexture("wolf-enemy-spritemap.png");
    readonly Texture hawkTexture = Engine.LoadTexture("hawk-enemy-spritemap.png");

    // sprites
    public static Sonic piper;
    public static Enemy wolf;
    public static Enemy hawk;
    Sprite[] sprites = new Sprite[1];
    public static ArrayList enemiesOnScreen = new ArrayList();

    public static Scoreboard sb;

    Font arial = Engine.LoadFont("Arial.ttf", 10);
    Rendering render;
    Vector2 pos;
    Key currentKey = Key.Q; // defaults to unused key "Q"

    public static int gameDifficulty = 0;

    public static readonly int HARD = 2;
    public static readonly int MEDIUM = 1;
    public static readonly int EASY = 0;

    public static bool debugToggle = false;
    public Game()
    {
        //scene control
        startScene = 1;
        endScene = false;

        //scoreboard
        sb = new Scoreboard();

        //create map
        map = new Map("RingEnemyMapWithStroke.bmp");
        enemyArr = enemies.ToArray();
        flowerArr = flowers.ToArray();

        // create piper sprite
        piper = new Sonic(new Vector2(160, 960), piperTexture, new Vector2(24, 24));
        sprites[0] = piper;

        piper.onPath = false;


        Path2 tunnelTest = new BezierGroup(new Vector2[] { new Vector2(6134, 586), new Vector2(6395.19f, 586), new Vector2(6302.21f, 709.375f), new Vector2(6270.96f, 741), new Vector2(6239.71f, 772.625f), new Vector2(6230.88f, 896), new Vector2(6482, 896) });

        piper.currPath = tunnelTest;
        piper.fractionOfPath = 0;

        render = new Rendering("display_map.png", new Bounds2(7 * Game.Resolution.X / 16, Game.Resolution.Y / 3, Game.Resolution.X / 8, Game.Resolution.Y / 3));

        //using svg to get normal vectors
        SVGReader.findElementsAndAdd(map, "Assets/map_svg_form.txt");

    }

    public void Update()
    {
        //scene control
        if (startScene == 2)
        {
            startScene = Scenes.instructionsScene();
        }
        else if (startScene == 1) { startScene = Scenes.titleScene(); }
        else if (endScene) { Scenes.endScene(message); }
        else
        {
            // collect player input
            currentKey = InputHandler.getPlayerInput(piper, render.pos + piper.loc - new Vector2(12, 12));


            //collision detection
            //ground and walls
            Physics.detectSolid(piper);
            Physics.detectPath(piper);
            //Physics.detectGround(piper);
            //Physics.detectUnpenetrable(piper);

            //other sprites
            Physics.detectCollisions(piper, flowerArr);
            Physics.detectCollisions(piper, enemyArr);


            //update acceleration
            piper.setAcceleration(currentKey);

            //update overall physics
            Physics.updatePhysics(sprites);

            // collect input and draw frame

            render.scrollingMotion();
            foreach (Enemy enemy in enemiesOnScreen)
            {
                enemy.updateState();
                enemy.setFrameIndex(Animator.animateEnemy(enemy, render.pos + enemy.loc));
            };
            piper.setFrameIndex(Animator.animatePiper(piper, render.pos + piper.loc, currentKey));

            //rings[0].draw(new Bounds2(0, 0, 24, 24), render.pos + rings[0].loc - new Vector2(10,10));
            sb.updateScoreboard();
            if (piper.loc.X >= 8000)
            {
                endScene = true;
            }

            if (Engine.GetKeyDown(Key.F3))
            {
                debugToggle = !debugToggle;
            }
            if (debugToggle)
            {
                Engine.DrawRectSolid(new Bounds2(render.pos + piper.loc - new Vector2(1, 1), new Vector2(3, 3)), Color.Red);
                piper.drawVectors(render.pos + piper.loc);

                Engine.DrawString("Pos: " + piper.loc.Rounded(2).ToString(), new Vector2(Resolution.X - 12, 2), Color.Black, arial, TextAlignment.Right);
                Engine.DrawString("onGround? " + piper.onGround, new Vector2(Resolution.X - 12, 14), Color.Black, arial, TextAlignment.Right);
                Engine.DrawString("onPath? " + piper.onPath + " with fraction: " + Math.Round(piper.fractionOfPath, 3), new Vector2(Resolution.X - 12, 26), Color.Black, arial, TextAlignment.Right);
                Engine.DrawString("current normal: " + map.getNormalVector(piper.loc).ToString(), new Vector2(Resolution.X - 12, 38), Color.Black, arial, TextAlignment.Right);
                Engine.DrawString("isSpinning? " + piper.isSpinning, new Vector2(Resolution.X - 12, 50), Color.Black, arial, TextAlignment.Right);
            }

            if (Engine.GetKeyDown(Key.R))
            {
                //scene control
                startScene = 1;
                endScene = false;

                //scoreboard
                sb = new Scoreboard();

                //create map
                map = new Map("RingEnemyMapWithStroke.bmp");
                enemyArr = enemies.ToArray();
                flowerArr = flowers.ToArray();

                // create piper sprite
                piper = new Sonic(new Vector2(160, 960), piperTexture, new Vector2(24, 24));
                sprites[0] = piper;

                render = new Rendering("display_map.png", new Bounds2(7 * Game.Resolution.X / 16, Game.Resolution.Y / 3, Game.Resolution.X / 8, Game.Resolution.Y / 3));

                //using svg to get normal vectors
                SVGReader.findElementsAndAdd(map, "Assets/map_svg_form.txt");
            }
        }
    }
}