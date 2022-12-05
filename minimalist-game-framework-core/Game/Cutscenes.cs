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
    }

