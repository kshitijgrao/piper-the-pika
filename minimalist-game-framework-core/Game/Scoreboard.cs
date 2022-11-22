using System;

	public class Scoreboard
	{
	int score;
	float time;
	int flowers;
    int extraLives;
	Texture pikaPlaceholder = Engine.LoadTexture("pikaPlaceholder.png");

		public Scoreboard()
		{
			score = 0;
			time = 0;
			flowers = 0;
			extraLives = 1;
		}
		
		public void updateScoreboard()
		{
			time += Engine.TimeDelta;

			//topLeft elements
			Engine.DrawString("SCORE", new Vector2(0, 0), Color.Yellow, Engine.LoadFont("Arial.ttf", 10));
			Engine.DrawString("" + score, new Vector2(40, 0), Color.White, Engine.LoadFont("Arial.ttf", 10));

			Engine.DrawString("TIME", new Vector2(0, 15), Color.Yellow, Engine.LoadFont("Arial.ttf", 10));
	        Engine.DrawString(""+(int)time, new Vector2(28, 15), Color.White, Engine.LoadFont("Arial.ttf", 10));

			Engine.DrawString("RINGS", new Vector2(0, 30), Color.Yellow, Engine.LoadFont("Arial.ttf", 10));
			Engine.DrawString("" + flowers, new Vector2(35, 30), Color.White, Engine.LoadFont("Arial.ttf", 10));



			//bottom left elements
			Engine.DrawTexture(pikaPlaceholder, new Vector2(6, 197), size: new Vector2(19, 17));
			Engine.DrawString("PIPER", new Vector2(26, 195), Color.Yellow, Engine.LoadFont("Arial.ttf", 10));
			Engine.DrawString("x      " + extraLives, new Vector2(26, 204), Color.White, Engine.LoadFont("Arial.ttf", 10));
	
		}



		public void addFlowers(int x) 
		{
			flowers += x;
			score += x * 10;
		}

	
}


