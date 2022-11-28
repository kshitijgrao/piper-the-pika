using System;

	public class Scoreboard
	{
	int score;
	float time;
	int flowers;
    int lives;
	int oneUps;
	Texture pikaPlaceholder = Engine.LoadTexture("pikaPlaceholder.png");
	Font arial = Engine.LoadFont("Arial.ttf", 10);

		public Scoreboard()
		{
			score = 0;
			time = 0;
			flowers = 0;
			lives = 3;
			oneUps = 0;
			
		}
		
		public void updateScoreboard()
		{
			//check for game over
			if (lives == 0)
			{
				gameOver();
			}

			//check for oneUps
			if (oneUps < (score / 50000))
			{
				oneUps++;
				lives++;
			}

			//update time
			time += Engine.TimeDelta;

			//topLeft scoreboard elements
			Engine.DrawString("SCORE", new Vector2(0, 0), Color.Yellow, arial);
			Engine.DrawString("" + score, new Vector2(40, 0), Color.White, arial);
			Engine.DrawString("TIME", new Vector2(0, 15), Color.Yellow, arial);
			String timeDisplay = (int)time / 60 + ":";
			if (time % 60 < 10)
			{
				timeDisplay += 0;
			}	
			timeDisplay += (int)time % 60;

	        Engine.DrawString(timeDisplay, new Vector2(28, 15), Color.White, arial);
			Engine.DrawString("FLOWERS", new Vector2(0, 30), Color.Yellow, arial);
			Engine.DrawString("" + flowers, new Vector2(55, 30), Color.White, arial);



			//bottom left scoreboard elements
			Engine.DrawTexture(pikaPlaceholder, new Vector2(6, 197), size: new Vector2(19, 17));
			Engine.DrawString("PIPER", new Vector2(26, 195), Color.Yellow, arial);
			Engine.DrawString("x      " + lives, new Vector2(26, 204), Color.White, arial);
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
			lives += x;
		}

		//time bonus after level
		public void timeBonus(int seconds)
	{
		if ((int)time < 29)
		{

		}
	}

		//death sequence
		public void gameOver()
		{
			
		}


}


