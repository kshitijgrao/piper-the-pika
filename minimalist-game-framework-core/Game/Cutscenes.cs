using System;

public class Scenes
	{
		//fonts
        static Font h1 = Engine.LoadFont("Arial.ttf", 40);
		static Font h2 = Engine.LoadFont("Arial.ttf", 20);
		static Font h3 = Engine.LoadFont("Arial.ttf", 12);

		//startScene variables
		static Texture piper = Engine.LoadTexture("pika-spritemap.png");
		static int frameCount = 0;
		
		//endScene variables
		static Boolean scoreUpdate = false;
		static int line1X = -100;
		static int line2X = -100;
		static int line3X = 320;
		static int line4x = 320;
		static int textSlideSpeed = 10;
		static int bonus;


        //Title Scene
		public static int titleScene()
		{
		if (Engine.GetKeyDown(Key.Space))
		{
			return 2;
		}
		if (Engine.GetMouseButtonDown(MouseButton.Left))
		{
			return 0;
            frameCount = 0;
        }

        Engine.DrawTexture(piper, new Vector2(135, 50), size: new Vector2(50, 50));
        Engine.DrawString("PIPER", new Vector2(100, 100), Color.White, h1);
        Engine.DrawString("THE PIKA", new Vector2(115, 140), Color.White, h2);
		Engine.DrawString("Press SPACE for Instructions", new Vector2(0, 0), Color.White, h3);
		Engine.DrawString(">   Easy", new Vector2(267,5), Color.White, h3);
		Engine.DrawString("Medium", new Vector2(273,25), Color.White, h3);
		Engine.DrawString("Hard", new Vector2(280,45), Color.White, h3);
		if (frameCount%90<=45)
		{
			Engine.DrawString("Click to Start", new Vector2(125, 170), Color.Yellow, h3);
		}

		frameCount++;
		return 1;
			
		}

		//Instructions Scene
		public static int instructionsScene()
		{
		if (Engine.GetMouseButtonDown(MouseButton.Left) || Engine.GetKeyDown(Key.Space))
		{ 
			return 1;
		}
		Engine.DrawString("BACK", new Vector2(0, 0), Color.White, h3);
		Engine.DrawString("Press A and D to move", new Vector2(100,60), Color.White, h3);
		Engine.DrawString("Press SPACE to jump", new Vector2(100, 90), Color.White, h3);
        Engine.DrawString("Collect rings for a speed boost", new Vector2(80, 120), Color.White, h3);
        return 2;
		}


        //End Scene
		public static void endScene()

		{
			//exit game
			if (Engine.GetMouseButtonDown(MouseButton.Left))
			{
				Environment.Exit(0);
            }

			//line sliding animation
			if (line1X < 100)
			{
				line1X += textSlideSpeed;
			}
			else if (line2X < 115)
			{
				line2X += textSlideSpeed;
			}
			else if (line3X > 170)
			{
				line3X -= textSlideSpeed;
			}
			else if (line4x > 75)
			{
				line4x -= textSlideSpeed;
			}


            //score update animation
            else if (bonus > 0)
			{
				Scoreboard.updateScore(25);
				bonus -= 25;
			}

			//flashing exit text
			else if (frameCount % 90 <= 45)
			{
				Engine.DrawString("Exit", new Vector2(150, 180), Color.Yellow, h3);
			}
			frameCount++;


			//draw text
			Engine.DrawString("PIPER  HAS", new Vector2(line1X, 50), Color.White, h2); //100, 50
			Engine.DrawString("PASSED", new Vector2(line2X, 75), Color.White, h2); //15 , 75
			Engine.DrawString("Act", new Vector2(line3X + 5, 97), Color.Orange, h3); //170, 97
			Engine.DrawString("1", new Vector2(line3X + 30, 72), Color.Yellow, h1) ; //200, 72

			if (!scoreUpdate)
			{
				bonus = Scoreboard.timeBonus();
				scoreUpdate = true;
			}

			//draw score
			Engine.DrawString("SCORE", new Vector2(line4x+5, 125), Color.Yellow, h3); //75, 125
			Engine.DrawString(Scoreboard.getScore() + "", new Vector2(line4x+140, 125), Color.White, h3); //210, 125
			Engine.DrawString("TIME BONUS", new Vector2(line4x+5, 145), Color.Yellow, h3); //75, 145
			Engine.DrawString("" + bonus, new Vector2(line4x+140, 145), Color.White, h3); //210, 145

		}
	
	}

