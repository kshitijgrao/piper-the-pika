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
        // map rendering
        map = new Rendering("sonic map 1.png", "pikaPlaceholder.png");
        //scoreboard
        sb = new Scoreboard();
    }

    public void Update()
    {
        map.scrollingWindow();
        sb.updateScoreboard();
    }
}
