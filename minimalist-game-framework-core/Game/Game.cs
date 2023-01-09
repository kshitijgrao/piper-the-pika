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


    readonly Texture piperTexture = Engine.LoadTexture("pika-spritemap.png");
    readonly Texture wolfTexture = Engine.LoadTexture("wolf-enemy-spritemap.png");
    readonly Texture hawkTexture = Engine.LoadTexture("hawk-enemy-spritemap.png");

    // sprites
    public static Sonic nextFrame;
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

    bool debugToggle = false;
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
        nextFrame = new Sonic(new Vector2(160, 960), piperTexture, new Vector2(24, 24));
        sprites[0] = nextFrame;

        render = new Rendering("display_map.png", new Bounds2(7 * Game.Resolution.X / 16, Game.Resolution.Y / 3, Game.Resolution.X / 8, Game.Resolution.Y / 3));

        //using svg to get normal vectors
        string[] lines = File.ReadAllLines("Assets/map_svg_form.txt");
        foreach (string line in lines)
        {
            if (line.Length > 16)
            {
                if (line.Substring(0, 5) == "<rect" && (line.Substring(line.Length - 16) == "fill=\"#710000\"/>" || line.Substring(line.Length - 16) == "fill=\"#FF0000\"/>"))
                {
                    if (line.Contains("y"))
                    {
                        string bruh = line.Substring(9, line.Length - 18 - 9).Replace("=", "").Replace("width", "").Replace("height", "").Replace("y", "").Replace("\"\"", "\"").Replace(" ", "");
                        string[] rectVals = line.Substring(9, line.Length - 18 - 9).Replace("=", "").Replace("width", "").Replace("height", "").Replace("y", "").Replace(" ", "").Replace("\"\"", "\"").Split('\"');
                        map.addCurve(new Rect(rectVals, line.Substring(line.Length - 9, 6)));
                    }
                    else if (line.Contains("transform"))
                    {
                        string[] rectVals = line.Substring(13, line.Length - 19 - 13).Replace("transform=\"matrix(", "").Replace("height=", "").Replace("\"", "").Split(' ');
                        map.addCurve(new Rect(rectVals, line.Substring(line.Length - 9, 6)));
                    }
                }
            }
        }

    }

    public void Update()
    {
        Debug.WriteLine(map.getNormalVector(new Vector2(2118,774)).ToString());
        //scene control
        if (startScene == 2)
        {
            startScene = Scenes.instructionsScene();
        }
        else if (startScene == 1) { startScene = Scenes.titleScene(); }
        else if (endScene) { Scenes.endScene(message); }
        else
        {
            currentKey = InputHandler.getPlayerInput(nextFrame, render.pos + nextFrame.loc - new Vector2(12, 12));


            //collision detection
            //ground and walls
            Physics.detectSolid(nextFrame);
            //Physics.detectGround(piper);
            //Physics.detectUnpenetrable(piper);

            //other sprites
            Physics.detectCollisions(nextFrame, flowerArr);
            Physics.detectCollisions(nextFrame, enemyArr);


            //update acceleration
            nextFrame.setAcceleration(currentKey);

            //update overall physics
            Physics.updatePhysics(sprites);

            // collect input and draw frame

            render.scrollingMotion();
            foreach (Enemy enemy in enemiesOnScreen)
            {
                enemy.updateState();
                enemy.setFrameIndex(Animator.animateEnemy(enemy, render.pos + enemy.loc));
            };

            // draws current frame
            //nextFrame.setFrameIndex(Animator.animatePiper(nextFrame, render.pos + nextFrame.loc, currentKey));
            
            //rings[0].draw(new Bounds2(0, 0, 24, 24), render.pos + rings[0].loc - new Vector2(10,10));
            sb.updateScoreboard();
            if (nextFrame.loc.X >= 6125)
            {
                endScene = true;
            }
            if (Engine.GetKeyDown(Key.F3))
            {
                debugToggle = !debugToggle;
            }
            if (debugToggle)
            {
                Engine.DrawRectSolid(new Bounds2(render.pos + nextFrame.loc - new Vector2(1, 1), new Vector2(3, 3)), Color.Red);
                nextFrame.drawVectors(render.pos + nextFrame.loc);

                Engine.DrawString("onGround? " + nextFrame.onGround + " at " + nextFrame.loc.Rounded(2).ToString(), new Vector2(Resolution. X - 12,12), Color.Black, arial,TextAlignment.Right);
                Engine.DrawString("current normal: " + map.getNormalVector(nextFrame.loc).ToString(), new Vector2(Resolution.X - 12, 24), Color.Black, arial,TextAlignment.Right);

                // draws next frame
                nextFrame.setFrameIndex(Animator.animatePiper(nextFrame, render.pos + nextFrame.loc, currentKey));
            }
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
            nextFrame = new Sonic(new Vector2(160, 960), piperTexture, new Vector2(24, 24));
            sprites[0] = nextFrame;

            render = new Rendering("display_map.png", new Bounds2(7 * Game.Resolution.X / 16, Game.Resolution.Y / 3, Game.Resolution.X / 8, Game.Resolution.Y / 3));

            //using svg to get normal vectors
            string[] lines = File.ReadAllLines("Assets/map_svg_form.txt");
            foreach (string line in lines)
            {
                if (line.Length > 16)
                {
                    if (line.Substring(0, 5) == "<rect" && (line.Substring(line.Length - 16) == "fill=\"#710000\"/>" || line.Substring(line.Length - 16) == "fill=\"#FF0000\"/>"))
                    {
                        if (line.Contains("y"))
                        {
                            string bruh = line.Substring(9, line.Length - 18 - 9).Replace("=", "").Replace("width", "").Replace("height", "").Replace("y", "").Replace("\"\"", "\"").Replace(" ", "");
                            string[] rectVals = line.Substring(9, line.Length - 18 - 9).Replace("=", "").Replace("width", "").Replace("height", "").Replace("y", "").Replace(" ", "").Replace("\"\"", "\"").Split('\"');
                            map.addCurve(new Rect(rectVals, line.Substring(line.Length - 9, 6)));
                        }
                        else if (line.Contains("transform"))
                        {
                            string[] rectVals = line.Substring(13, line.Length - 19 - 13).Replace("transform=\"matrix(", "").Replace("height=", "").Replace("\"", "").Split(' ');
                            map.addCurve(new Rect(rectVals, line.Substring(line.Length - 9, 6)));
                        }
                    }
                }
            }
        }
    }
}