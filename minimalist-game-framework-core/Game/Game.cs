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

    public Game()
    {
        level1 = new Level("collision_map_1_11.bmp", "Assets/map_svg_form.txt", "displayMap1_14_windows.png", "newBG_windows.png", new Vector2(160, 960), 170, LevelPassed.none);
        level2 = new Level("collision_map_1_11.bmp", "Assets/map_svg_form.txt", "displayMap1_14_windows.png", "cityBG4.png", new Vector2(160, 960), 8000, LevelPassed.onePassed);
        level3 = new Level("collision_map_1_11.bmp", "Assets/map_svg_form.txt", "displayMap1_14_windows.png", "caveBG.png", new Vector2(160, 960), 8000, LevelPassed.twoPassed);

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