using System;
using System.Collections.Generic;

class Game
{
    public static readonly string Title = "Minimalist Game Framework";
    public static readonly Vector2 Resolution = new Vector2(320, 224);
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
