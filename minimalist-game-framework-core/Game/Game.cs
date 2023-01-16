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

    public static int gameDifficulty = 0;

    public static readonly int HARD = 2;
    public static readonly int MEDIUM = 1;
    public static readonly int EASY = 0;

    public static bool debugToggle = false;

    public static Level currentLevel;
    public Game()
    {
        //scene control
        currentScene = Scene.start;


        currentLevel = new Level("collision_map_1_11.bmp", "Assets/map_svg_form.txt","displayMap1_14_windows.png", "newBG_windows.png", new Vector2(160, 960), 8000);
    }

    public void Update()
    {

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
            currentLevel.playLevel();


            if (Engine.GetKeyDown(Key.R))
            {
                currentScene = Scene.start;
                currentLevel.reset();
            }
        }
    }
}