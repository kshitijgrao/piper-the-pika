using System;
using System.Collections.Generic;

class Game
{
    public static readonly string Title = "Minimalist Game Framework";
    public static readonly Vector2 Resolution = new Vector2(640, 480);

    //scoreboard elements
    int score;
    int time;
    int rings;

    public Game()
    {

        //scoreboard elements
        score = 0;
        time = 0;
        rings = 0;
    }

    public void Update()
    {

        //scoreboard elements
        //Engine.DrawString("SCORE: " + score, new Vector2(0,0), Color.Yellow, Engine.LoadFont("Raleway.ttf", 40));
        //Engine.DrawString("TIME: " + time, new Vector2(0, 40), Color.Yellow, Engine.LoadFont("Raleway.ttf", 40));
        //Engine.DrawString("RINGS: " + rings, new Vector2(0, 80), Color.Yellow, Engine.LoadFont("Raleway.ttf", 40));
    }
}
