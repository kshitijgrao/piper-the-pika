using System;

public class Cutscenes
	{
        Font title = Engine.LoadFont("Arial.ttf", 30);
		Font subtitle = Engine.LoadFont("Arial.ttf", 15);

        public Cutscenes()
		{
		}


        //Title Scene
		public void titleScene()
		{
			Engine.DrawString("PIPER", new Vector2(100, 150), Color.White, title);
			Engine.DrawString("THE PIKA", new Vector2(100, 180), Color.White, subtitle); 
		}


        //End Scene
		public void endScene()
	{
		Engine.DrawString("PIPER HAS", new Vector2(100, 150), Color.White, title);
		Engine.DrawString("PASSED", new Vector2(0, 0), Color.White, title);
		Engine.DrawString("Act", new Vector2(0, 0), Color.Orange, title);
		Engine.DrawString("1", new Vector2(0, 0), Color.Yellow, title);
	}
	
    }

