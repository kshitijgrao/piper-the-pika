using System;
using System.Collections.Generic;

class Game
{
    public static readonly string Title = "Minimalist Game Framework";
    public static readonly Vector2 Resolution = new Vector2(640, 480);
    Scoreboard sb;


    public Game()
    {
        //scoreboard
        sb = new Scoreboard();
    }

    public void Update()
    {
        sb.updateScoreboard();

    
    }
}
