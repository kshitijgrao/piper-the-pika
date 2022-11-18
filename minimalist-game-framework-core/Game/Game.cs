using System;
using System.Collections.Generic;

class Game
{
    public static readonly string Title = "Minimalist Game Framework";
    public static readonly Vector2 Resolution = new Vector2(640, 480);

    //scoreboard elements
    int score;
    int time;
    int flowers;
    int extraLives;

    public Game()
    {

        //scoreboard elements
        score = 0;
        time = 0;
        flowers = 0;
        extraLives = 1;
    }

    public void Update()
    {
        //Scoreboard.updateScoreboard();

        //scoreboard elements
        Engine.DrawString("SCORE: " + score, new Vector2(0,0), Color.Yellow, Engine.LoadFont("Arial.ttf", 40));
        Engine.DrawString("TIME: " + time, new Vector2(0, 40), Color.Yellow, Engine.LoadFont("Arial.ttf", 40));
        Engine.DrawString("RINGS: " + flowers, new Vector2(0, 80), Color.Yellow, Engine.LoadFont("Arial.ttf", 40));

        //if collision with flower, increase
        

        score = flowers*10;
    }
}
