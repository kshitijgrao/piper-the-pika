using System;
using System.Collections.Generic;

class Game
{
    public static readonly string Title = "Minimalist Game Framework";
    public static readonly Vector2 Resolution = new Vector2(320, 224);

    public static Map map;


    Scoreboard sb;

    Font arial = Engine.LoadFont("Arial.ttf", 10);

    public Game()
    {
        //scoreboard
        sb = new Scoreboard();

        map = new Map("C:/Users/evane/source/repos/recreate-a-classic-game-sonic-yeer/minimalist-game-framework-core/Assets/testing_query.bmp");


    }

    public void Update()
    {
        sb.updateScoreboard();


    }
}
