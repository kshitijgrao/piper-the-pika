using System;

	public class Scoreboard
	{
	int score;
	int time;
	int flowers;
    int extraLives;

    public Scoreboard()
	{
		score = 0;
		time = 0;
		flowers = 0;
		extraLives = 1;
	}

	public void updateScoreboard()
	{
		Engine.DrawString("SCORE: " + score, new Vector2(0, 0), Color.Yellow, Engine.LoadFont("Arial.ttf", 40));
		Engine.DrawString("TIME: " + time, new Vector2(0, 40), Color.Yellow, Engine.LoadFont("Arial.ttf", 40));
		Engine.DrawString("RINGS: " + flowers, new Vector2(0, 80), Color.Yellow, Engine.LoadFont("Arial.ttf", 40));

	}

	public void updateScore()
	{

	}

	public void updateTime()
	{

	}

	public void updateFlowers()
	{

	}

	
}


