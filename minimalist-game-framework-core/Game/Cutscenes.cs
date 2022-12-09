using System;

public class Scenes
	{

        static Font h1 = Engine.LoadFont("Arial.ttf", 40);
		static Font h2 = Engine.LoadFont("Arial.ttf", 20);
		static Font h3 = Engine.LoadFont("Arial.ttf", 12);
		static Texture piper = Engine.LoadTexture("pika-spritemap.png");
		static Boolean scoreUpdate = false;


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
        Engine.DrawString("Click to Start", new Vector2(125, 170), Color.Yellow, h3);
        return true;
			
		}


        //End Scene

		public static Boolean endScene()

	{
		if (Engine.GetMouseButtonDown(MouseButton.Left))
		{
			return false;
		}
		Engine.DrawString("PIPER HAS", new Vector2(100, 150), Color.White, h1);
		Engine.DrawString("PASSED", new Vector2(0, 0), Color.White, h2);
		Engine.DrawString("Act", new Vector2(0, 0), Color.Orange, h2);
		Engine.DrawString("1", new Vector2(0, 0), Color.Yellow, h1);
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

