using System;

public class Scoreboard
{
    public int score;
    static float time;
    public static int flowers;
    public static int lives;
    static int oneUps;
    public Boolean instructions;

    Texture pikaPlaceholder = Engine.LoadTexture("pikaPlaceholder.png");
    Texture flowerTexture = Engine.LoadTexture("flower.png");
    Font arial = Engine.LoadFont("Arial.ttf", 10);

    public Scoreboard()
    {
        score = 0;
        time = 0;
        flowers = 0;
        lives = 3;
        oneUps = 0;
        instructions = false;

    }

    public Scene updateScoreboard()
    {
        //check for game over
        if (lives > 0)
        {
            //instructions control
            if (Engine.GetKeyDown(Key.I) || Engine.GetKeyDown(Key.Space))
            {
                if (instructions)
                {
                    instructions = false;
                }
                else if (Engine.GetKeyDown(Key.I))
                {
                    instructions = true;
                }
            }

            //draw scoreboard if not on instructions
            if (!instructions)
            {
                renderScoreboard();
            }
            else
            {
                Engine.DrawString("BACK", new Vector2(0, 0), Color.Black, arial);
                Engine.DrawString("Press A and D to move", new Vector2(100, 30), Color.Black, arial);
                Engine.DrawString("Press SPACE to jump", new Vector2(100, 60), Color.Black, arial);
                Engine.DrawString("Collect rings for a speed boost", new Vector2(80, 90), Color.Black, arial);
            }

            //check for oneUps
            if (oneUps < (score / 50000))
            {
                oneUps++;
                lives++;
            }
            //update time
            time += Engine.TimeDelta;

        }
        if (lives <= 0)
        {
            return Scene.end;
            Game.message = "FAILED";
        }
        return Scene.game;
    }

    public void renderScoreboard()
    {
        // top left scoreboard elements
        Engine.DrawString("Press I for Instructions", new Vector2(210, 0), Color.Black, arial);
        Engine.DrawString("SCORE", new Vector2(0, 0), Color.Black, arial); //yellow
        Engine.DrawString("" + score, new Vector2(40, 0), Color.Black, arial); //white
        Engine.DrawString("TIME", new Vector2(0, 15), Color.Black, arial); //yellow

        String timeDisplay = (int)time / 60 + ":";
        if (time % 60 < 10)
        {
            timeDisplay += 0;
        }

        timeDisplay += (int)time % 60;

        Engine.DrawString(timeDisplay, new Vector2(28, 15), Color.Black, arial); //white
        Engine.DrawTexture(flowerTexture, new Vector2(0, 30));
        Engine.DrawString("FLOWERS", new Vector2(13, 30), Color.Black, arial); //yellow
        Engine.DrawString("" + flowers, new Vector2(68, 30), Color.Black, arial); //white



        //bottom left scoreboard elements
        Engine.DrawTexture(pikaPlaceholder, new Vector2(6, 197), size: new Vector2(19, 17));
        Engine.DrawString("PIPER", new Vector2(26, 195), Color.Black, arial); //yellow
        Engine.DrawString("x      " + lives, new Vector2(26, 204), Color.Black, arial); //white
    }

    //modify flower count
    public void addFlower()
    {
        flowers++;
        score += 10;
    }


    //if an enemy is killed, increase score based on how many enemies have been killed
    public void enemyKilled(int killNumber)
    {
        if (killNumber == 0)
        {
            score += 100;
        }
        else if (killNumber == 1)
        {
            score += 200;
        }
        else if (killNumber == 2)
        {
            score += 500;
        }
        else if (killNumber < 15)
        {
            score += 1000;
        }
        else
        {
            score += 10000;
        }
    }

    //input -1 if a life is lost
    //input 1 if an extra life is gained
    public static void modifyLives(int x)
    {
        lives += x;
    }

    //time bonus after level
    public static int timeBonus()
    {
        if ((int)time < 29)
        {
            return 50000;
        }
        else if ((int)time < 45)
        {
            return 10000;
        }
        else if ((int)time < 60)
        {
            return 5000;
        }
        else if ((int)time < 90)
        {
            return 4000;
        }
        else if ((int)time < 120)
        {
            return 3000;
        }
        else if ((int)time < 180)
        {
            return 2000;
        }
        else if ((int)time < 240)
        {
            return 1000;
        }
        else if ((int)time < 300)
        {
            return 500;
        }
        return 0;
    }

    public static float getTime()
    {
        return time;
    }

    public int getScore()
    {
        return score;
    }

    public void updateScore(int x)
    {
        score += x;
    }
}


