using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;


// Scene enumerator
public enum Scene {game, start, instructions, end, levels};
public enum Difficulty {none, easy, medium, hard};
public enum LevelPassed {none, onePassed, twoPassed, allPassed};

class Game
{
    public static readonly string Title = "Piper the Pika";
    public static readonly Vector2 Resolution = new Vector2(320, 224);
    //public static List<Vector2> enemyCoords = new List<Vector2>();
    public static Scene currentScene;
    public static String message = "PASSED";

    public static readonly string RIGHT = "right";
    public static readonly string LEFT = "left";

    
    public static readonly Music basicMusic = Engine.LoadMusic("emre_turkoglu_piper_basic_music.mp3");

    // sprites
    Sprite[] sprites = new Sprite[1];
    public static ArrayList enemiesOnScreen = new ArrayList();

    public static Scoreboard sb;

    public static Font arial = Engine.LoadFont("Arial.ttf", 10);
    Rendering render;
    Vector2 pos;
    Key currentKey = Key.Q; // defaults to unused key "Q"

    public static LevelPassed progress = LevelPassed.none;

    public static readonly int HARD = 2;
    public static readonly int MEDIUM = 1;
    public static readonly int EASY = 0;

    public static bool debugToggle = false;

    //public static Level level1 = new Level("collision_map_1_11.bmp", "Assets/map_svg_form.txt", "displayMap1_14_windows.png", "newBG_windows.png", new Vector2(160, 960), 8000);
    //public static Level level2;
    //public static Level level3;

    public static Level currentLevel;
    public static Level level1;
    public static Level level2;
    public static Level level3;

    public static Chunking level1Chunk;
    public static Chunking level1BG;

    public static Chunking level2Chunk;
    public static Chunking level2BG;

    public static Chunking level3Chunk;
    public static Chunking level3BG;

    public Game()
    {
        level1Chunk = new Chunking("displayMap1_14_windows.png");
        level1BG = new Chunking("newBG_windows.png");

        //level2 needs to be chunked: "displayMap1_14_windows.png"
        //level2 bg needs to be chunked: "cityBG4.png"

        //level3 needs to be chunked: "level3withpaths.png"
        //level3 bg needs to be chunked: "caveBG.png"


        level1 = new Level("collision_map_1_11.bmp", "Assets/map_svg_form.txt", level1Chunk, level1BG, new Vector2(160, 960), new Vector2(0, -838), new Vector2(2382, 420), new Vector2(893, 0), 8000, LevelPassed.twoPassed);
        level2 = new Level("collision_map_1_11.bmp", "Assets/map_svg_form.txt", level2Chunk, level2BG, new Vector2(160, 960), new Vector2(0, -838), new Vector2(2382, 420), new Vector2(893, 0), 8000, LevelPassed.onePassed);
        level3 = new Level("Level3_1_16.bmp", "Assets/level3_svg.txt", level3Chunk, level3BG, new Vector2(125, 800), new Vector2(0, -640), new Vector2(0, 0), new Vector2(0, 0), 10400, LevelPassed.none);

        //scene control
        progress = LevelPassed.none;
        currentScene = Scene.start;
        currentLevel = level1;
        //Engine.PlayMusic(basicMusic);
    }

    public void Update()
    {

        //scene control
        if (currentScene == Scene.instructions)
        {
            currentScene = Scenes.instructionsScene();
        }
        else if (currentScene == Scene.start) { currentScene = Scenes.titleScene(); }
        else if (currentScene == Scene.levels) { currentScene = Scenes.levelSelect(); }
        else if (currentScene == Scene.end) 
        { 
            Engine.StopMusic(5);
            Scenes.endScene(message);
        }
        else
        {

            currentLevel.playLevel();

            if (Engine.GetKeyDown(Key.R))
            {
                currentScene = Scene.start;
                Engine.StopMusic(0);
                currentLevel.reset();
            }
        }
    }
}