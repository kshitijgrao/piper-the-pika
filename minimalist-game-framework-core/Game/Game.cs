using System;
using System.Collections.Generic;

class Game
{
    public static readonly string Title = "Minimalist Game Framework";
    public static readonly Vector2 Resolution = new Vector2(320, 224);
    Scoreboard sb;
    Rendering map;


    public Game()
    {
        
        //scoreboard
        sb = new Scoreboard();
        map = new Rendering("sonic map 1.png");
    }

    public void Update()
    {
        sb.updateScoreboard();
        map.scrollingWindow();
    }
}
