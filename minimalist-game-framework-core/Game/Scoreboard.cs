using System;

	public class Scoreboard
	{
	int score;
	float time;
	int flowers;
    int extraLives;
	Texture pikaPlaceholder = Engine.LoadTexture("pikaPlaceholder.png");
	Font arial = Engine.LoadFont("Arial.ttf", 10);

		public Scoreboard()
		{
			score = 0;
			time = 0;
			flowers = 0;
			extraLives = 3;
		}
		
		public void updateScoreboard()
		{
			//check for game over
			if (extraLives == 0)
			{
				gameOver();
			}
			time += Engine.TimeDelta;

			//topLeft scoreboard elements


			Engine.DrawString("SCORE", new Vector2(0, 0), Color.Yellow, arial);
			Engine.DrawString("" + score, new Vector2(40, 0), Color.White, arial);
			Engine.DrawString("TIME", new Vector2(0, 15), Color.Yellow, arial);
	        Engine.DrawString(""+(int)time, new Vector2(28, 15), Color.White, arial);
			Engine.DrawString("RINGS", new Vector2(0, 30), Color.Yellow, arial);
			Engine.DrawString("" + flowers, new Vector2(35, 30), Color.White, arial);



			//bottom left scoreboard elements
			Engine.DrawTexture(pikaPlaceholder, new Vector2(6, 197), size: new Vector2(19, 17));
			Engine.DrawString("PIPER", new Vector2(26, 195), Color.Yellow, arial);
			Engine.DrawString("x      " + extraLives, new Vector2(26, 204), Color.White, arial);
		}


		//modify flower count
		public void modifyFlowers(int x) 
		{
			flowers += x;
			modifyScore(x * 10);
		}


		//if an enemy is killed, add 100 to score
		public void modifyScore(int x)
		{
			score += x;
		}

		//input -1 if a life is lost
		//input 1 if an extra life is gained
		public void modifyLives(int x)
		{
			extraLives += x;
		}

		public void gameOver()
		{
			
		}	
}


