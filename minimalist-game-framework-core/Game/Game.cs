using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;


// Scene enumerator
public enum Scene {game, start, instructions, end};


class Game
{
    public static readonly string Title = "Piper the Pika";
    public static readonly Vector2 Resolution = new Vector2(320, 224);
    //public static List<Vector2> enemyCoords = new List<Vector2>();
    public static Map map;
    
    public Scene currentScene;
    public static String message = "PASSED";
    public static List<Flower> flowers = new List<Flower>();
    public static List<Enemy> enemies = new List<Enemy>();

    public static Flower[] flowerArr;
    public static Enemy[] enemyArr;

    public static readonly string RIGHT = "right";
    public static readonly string LEFT = "left";


    readonly Texture piperTexture = Engine.LoadTexture("piper-spritemap.png");
    readonly Texture piperTextureBlink = Engine.LoadTexture("piper-spritemap-blink.png");
    readonly Texture wolfTexture = Engine.LoadTexture("wolf-enemy-spritemap.png");
    readonly Texture hawkTexture = Engine.LoadTexture("hawk-enemy-spritemap.png");
    readonly Music basicMusic = Engine.LoadMusic("emre_turkoglu_piper_basic_music.mp3");

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
        currentScene= Scene.start;

        //scoreboard
        sb = new Scoreboard();

        //create map
        map = new Map("collision_map_1_11.bmp");
        

        // create piper sprite
        piper = new Sonic(new Vector2(160, 960), piperTexture, piperTextureBlink);
        sprites[0] = piper;

        
        render = new Rendering("displayMap1_14_windows.png", "newBG_windows.png", new Bounds2(7 * Game.Resolution.X / 16, Game.Resolution.Y / 3, Game.Resolution.X / 8, Game.Resolution.Y / 3));

        //using svg to get normal vectors
        SVGReader.findElementsAndAdd(map, "Assets/map_svg_form.txt");

        enemyArr = enemies.ToArray();
        flowerArr = flowers.ToArray();

        // start music
        Engine.PlayMusic(basicMusic);

        //piper.currPath = map.paths[map.paths.Count - 1];
    }

    public void Update()
    {
        map.getNormalVector(new Vector2(1302, 932));

        //scene control
        if (currentScene == Scene.instructions)
        {
            currentScene = Scenes.instructionsScene();
        }
        else if (currentScene == Scene.start) { currentScene = Scenes.titleScene(); }
        else if (currentScene==Scene.end) 
        { 
            Engine.StopMusic(5);
            Scenes.endScene(message); 
        }
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

            if (debugToggle)
            {
                if (Engine.GetKeyDown(Key.C))
                {
                    render.pos = Resolution / 2 - piper.loc;
                }
            }
            

            foreach (Enemy enemy in enemiesOnScreen)
            {
                enemy.updateState();
                enemy.setFrameIndex(Animator.animateEnemy(enemy, render.pos + enemy.loc));
            };
            piper.setFrameIndex(Animator.animatePiper(piper, render.pos + piper.loc, currentKey));

            //rings[0].draw(new Bounds2(0, 0, 24, 24), render.pos + rings[0].loc - new Vector2(10,10));

            if (sb.updateScoreboard() == Scene.end)
            {
                currentScene = Scene.end;
            }

            if (piper.loc.X >= 8000)
            {
                currentScene = Scene.end;
            }

            if (Engine.GetKeyDown(Key.F3))
            {
                debugToggle = !debugToggle;
            }
            if (debugToggle)
            {
                Engine.DrawRectSolid(new Bounds2(render.pos + piper.loc - new Vector2(1, 1), new Vector2(3, 3)), Color.Red);
                piper.drawVectors(render.pos + piper.loc);

                Engine.DrawString("Pos: " + piper.loc.Rounded(2).ToString(), new Vector2(Resolution.X - 12, 12), Color.Black, arial, TextAlignment.Right);
                Engine.DrawString("onGround? " + piper.onGround, new Vector2(Resolution.X - 12, 24), Color.Black, arial, TextAlignment.Right);
                Engine.DrawString("onPath? " + piper.onPath + " with fraction: " + Math.Round(piper.fractionOfPath, 3), new Vector2(Resolution.X - 12, 36), Color.Black, arial, TextAlignment.Right);
                Engine.DrawString("current normal: " + map.getNormalVector(piper.loc).Rounded(2).ToString() + " current radius: " + Math.Round(map.getSurfaceRadius(piper.loc),7), new Vector2(Resolution.X - 12, 48), Color.Black, arial, TextAlignment.Right);
                Engine.DrawString("isSpinning? " + piper.isSpinning, new Vector2(Resolution.X - 12, 60), Color.Black, arial, TextAlignment.Right);

                Debug.WriteLine(map.getSurfaceRadius(piper.loc));
            }


            if (Engine.GetKeyDown(Key.R))
            {
                //scene control
                currentScene = Scene.start;

                //scoreboard
                sb = new Scoreboard();

                //create map
                enemiesOnScreen.Clear();
                enemies.Clear();
                flowers.Clear();
                map = new Map("collision_map_1_11.bmp");
                enemyArr = enemies.ToArray();
                flowerArr = flowers.ToArray();
                // create piper sprite
                piper = new Sonic(new Vector2(160, 960), piperTexture, new Vector2(24, 24));
                sprites[0] = piper;

                render = new Rendering("displayMap1_14_windows.png", "newBG_windows.png", new Bounds2(7 * Game.Resolution.X / 16, Game.Resolution.Y / 3, Game.Resolution.X / 8, Game.Resolution.Y / 3));

                //using svg to get normal vectors
                SVGReader.findElementsAndAdd(map, "Assets/map_svg_form.txt");
                enemyArr = enemies.ToArray();
                flowerArr = flowers.ToArray();
            }
        }
    }
}