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
        //testing commits
        //testing branch commit

        //scoreboard elements
        score = 0;
        time = 0;
        rings = 0;
    }

    public void Update()
    {

        //scoreboard elements
        Engine.DrawString("SCORE: ", new Vector2(0,0), Color.Red, Engine.LoadFont("Raleway.ttf", 40));
    }
}
