using System;
using System.Collections.Generic;

class Game
{
    public static readonly string Title = "Minimalist Game Framework";
    public static readonly Vector2 Resolution = new Vector2(640, 480);
    Texture map = Engine.LoadTexture("map.jpg"); //1944 x 1172
    public Vector2 window = new Vector2(640, 480);

    public Game()
    {

    }

    public void Update()
    {
        Engine.DrawTexture(map, Vector2.Zero);
    }
}
