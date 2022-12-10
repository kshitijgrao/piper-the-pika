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
		static int textSlideSpeed = 10;


        //Title Scene
		public static Boolean titleScene()
		{
		if (Engine.GetMouseButtonDown(MouseButton.Left))
		{
			return false;
		}

        Engine.DrawTexture(piper, new Vector2(140, 50), size: new Vector2(50, 50));
        Engine.DrawString("PIPER", new Vector2(100, 100), Color.White, h1);
        Engine.DrawString("THE PIKA", new Vector2(115, 140), Color.White, h2);
		if (frameCount%90<=45)
		{
			Engine.DrawString("Click to Start", new Vector2(125, 170), Color.Yellow, h3);
		}

		frameCount++;
		return true;
			
		}


        //End Scene
		public static Boolean endScene()

	{
		if (Engine.GetMouseButtonDown(MouseButton.Left))
		{
			return false;
		}


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

		Engine.DrawString("PIPER  HAS", new Vector2(line1X, 50), Color.White, h2); //100, 50
		Engine.DrawString("PASSED", new Vector2(line2X, 75), Color.White, h2); //15 , 75


		Engine.DrawString("Act", new Vector2(line3X + 5, 97), Color.Orange, h3); //170, 97
		Engine.DrawString("1", new Vector2(line3X + 30, 72), Color.Yellow, h1) ; //200, 72
		Scoreboard.addFlowerScore();

		if (!scoreUpdate)
		{

			Scoreboard.addFlowerScore();
			int tBonus = Scoreboard.timeBonus();
			Engine.DrawString(tBonus + "", new Vector2(100, 00), Color.Black, h2);

			scoreUpdate = true;
		}

		return true;
	}
	
    }

