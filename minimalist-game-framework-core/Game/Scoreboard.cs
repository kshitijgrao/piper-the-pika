using System;

public class Scoreboard
{
    static int score;
    static float time;
    static int flowers;
    static int lives;
    static int oneUps;

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
        if (lives != 0)
        {
            renderScoreboard();

            //check for oneUps
            if (oneUps < (score / 50000))
            {
                oneUps++;
                lives++;
            }
            //update time
            time += Engine.TimeDelta;

        }
    }

    public void renderScoreboard()
        {
            //topLeft scoreboard elements

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
            Engine.DrawString("FLOWERS", new Vector2(0, 30), Color.Black, arial); //yellow
            Engine.DrawString("" + flowers, new Vector2(55, 30), Color.Black, arial); //white



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

    public static int getScore()
    {
        return score;
    }

    public static void updateScore(int x)
    {
        score += x;
    }
}


