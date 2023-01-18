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

    public static Boolean playedMusic = false;
    
    public static readonly Music basicMusic = Engine.LoadMusic("emre_turkoglu_piper_basic_music.mp3");
    // sounds from https://opengameart.org/content/beep-tone-sound-sfx
    public static readonly Sound flowerCollectSound = Engine.LoadSound("beep.wav");
    public static readonly Sound piperDamageSound = Engine.LoadSound("piper-damage-sound.wav");
    public static readonly Sound piperDeathSound = Engine.LoadSound("piper-death-sound.ogg");
    public static readonly Sound piperVictorySound = Engine.LoadSound("victory-sound.wav");
    public static readonly Sound enemyDamageSound = Engine.LoadSound("enemy-damage-sound.ogg");

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
        level1Chunk = new Chunking("Level1Display1_17.png");
        level1BG = new Chunking("newBG_windows.png");

        level2Chunk = new Chunking("level2chunk1.png", "level2chunk2.png", "level2chunk3.png");
        level2BG = new Chunking("level2BGchunk1.png", "level2BGchunk2.png", "level2BGchunk3.png");

        level3Chunk = new Chunking("level3chunk1.png", "level3chunk2.png", "level3chunk3.png");
        level3BG = new Chunking("level3bg1.png", "level3bg2.png", "level3bg3.png");

        level1 = new Level("Level1Collision1_17.bmp", "Assets/level1_svg.txt", level1Chunk, level1BG, new Vector2(160, 960), new Vector2(0, -838), new Vector2(2382, 420), new Vector2(0, 0), 170, LevelPassed.none);
        level2 = new Level("Level2Collision1_17.bmp", "Assets/level2_svg.txt", level2Chunk, level2BG, new Vector2(150, 630), new Vector2(0, -1168), new Vector2(2382, 420), new Vector2(0, 0), 170, LevelPassed.onePassed);
        level3 = new Level("Level3Collision1_17.bmp", "Assets/level3_svg.txt", level3Chunk, level3BG, new Vector2(125, 700), new Vector2(0, -640), new Vector2(0, 0), new Vector2(0, 10), 10400, LevelPassed.twoPassed);

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
            Engine.StopMusic(0);
            if (!playedMusic)
            {
                if (message.Equals("PASSED"))
                {
                    Engine.PlaySound(Game.piperVictorySound);
                }
                else
                {
                    Engine.PlaySound(Game.piperDeathSound);
                }
                playedMusic = true;
            }
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