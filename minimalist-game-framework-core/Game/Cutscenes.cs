﻿using System;

public class Scenes
{
    //fonts
    static Font h1 = Engine.LoadFont("Arial.ttf", 40);
    static Font h2 = Engine.LoadFont("Arial.ttf", 20);
    static Font h3 = Engine.LoadFont("Arial.ttf", 12);
    static Font h4 = Engine.LoadFont("Arial.ttf", 70);
    static Font h5 = Engine.LoadFont("Arial.ttf", 80);

    //startScene variables
    static Texture piper = Engine.LoadTexture("piper-spritemap.png");
    static Texture level1 = Engine.LoadTexture("level1thumbTest.png");
    static Texture level2 = Engine.LoadTexture("level2thumb.png");
    static Texture level2BW = Engine.LoadTexture("level2thumbBW.png");
    static Texture level3 = Engine.LoadTexture("level3thumb.png");
    static Texture level3BW = Engine.LoadTexture("level3thumbBW.png");
    static int frameCount = 0;

    //endScene variables
    static Boolean scoreUpdate = false;
    static int line1X = -100;
    static int line2X = -100;
    static int line3X = 320;
    static int line4x = 320;
    static int textSlideSpeed = 10;
    static int bonus;

    //levelSelect variables
    static int enterCount = 0;
    static int levelState = 1;
    static Difficulty diffState = Difficulty.none;


    //Title Scene
    public static Scene titleScene()
    {
        if (Engine.GetKeyDown(Key.I))
        {
            return Scene.instructions;
        }
        if (Engine.GetMouseButtonDown(MouseButton.Left) || Engine.GetKeyDown(Key.Space))
        {
            return Scene.levels;
            frameCount = 0;
        }

        Engine.DrawTexture(piper, new Vector2(135, 50), size: new Vector2(50, 50));
        Engine.DrawString("PIPER", new Vector2(100, 100), Color.White, h1);
        Engine.DrawString("THE PIKA", new Vector2(115, 140), Color.White, h2);
        Engine.DrawString("Press I for Instructions", new Vector2(0, 0), Color.White, h3);
        //Engine.DrawString(">   Easy", new Vector2(267,5), Color.White, h3);
        //Engine.DrawString("Medium", new Vector2(273,25), Color.White, h3);
        //Engine.DrawString("Hard", new Vector2(280,45), Color.White, h3);
        if (frameCount % 90 <= 45)
        {
            Engine.DrawString("SPACE to Start", new Vector2(125, 170), Color.Yellow, h3);
        }

        frameCount++;
        return Scene.start;

    }

    public static Scene levelSelect()
    {
        if (enterCount >= 2)
        {
            if (levelState == 1)
            {
                Game.currentLevel = Game.level1;
            } else if (levelState == 2)
            {
                Game.currentLevel = Game.level2;
            } else
            {
                Game.currentLevel = Game.level3;
            }

            Game.currentLevel.diff = diffState;
            enterCount = 0;
            diffState = 0;
            levelState = 1;
            return Scene.game;
        }

        if (Engine.GetKeyDown(Key.Left))
        {
            if (enterCount == 0)
            {
                if (levelState > 1)
                {
                    levelState--;
                }
            } else
            {
                if ((int) diffState > 1)
                {
                    diffState--;
                }
            }
        }

        if (Engine.GetKeyDown(Key.Right))
        {
            if (enterCount == 0)
            {
                if (levelState < ((int) Game.progress + 1))
                {
                    levelState++;
                }
            } else
            {
                if ((int) diffState < 3)
                {
                    diffState++;
                }
            }
        }

        if (Engine.GetKeyDown(Key.Return))
        {
            enterCount++;
            if (enterCount == 1)
            {
                diffState = Difficulty.easy;
            }
        }

        Engine.DrawString("LEVEL SELECT", new Vector2(87, 10), Color.White, h2);

        if (levelState == 1)
        {
            Engine.DrawTexture(level1, new Vector2(6, 62), size: new Vector2(100, 100));
            Engine.DrawString("1", new Vector2(36, 69), Color.Black, h5);
            Engine.DrawString("1", new Vector2(34, 67), Color.Yellow, h5);
        } else
        {
            Engine.DrawTexture(level1, new Vector2(16, 66), size: new Vector2(80, 80));
            Engine.DrawString("1", new Vector2(36, 67), Color.Black, h4);
            Engine.DrawString("1", new Vector2(34, 65), Color.White, h4);
        }

        Console.WriteLine(Game.progress);
        if (levelState == 2)
        {
            Engine.DrawTexture(level2, new Vector2(110, 62), size: new Vector2(100, 100));
            Engine.DrawString("2", new Vector2(140, 67), Color.Black, h5);
            Engine.DrawString("2", new Vector2(138, 65), Color.Yellow, h5);
        } else if (Game.progress < LevelPassed.onePassed)
        {
            Engine.DrawTexture(level2BW, new Vector2(120, 66), size: new Vector2(80, 80));
            Engine.DrawString("2", new Vector2(143, 67), Color.Black, h4);
            Engine.DrawString("2", new Vector2(141, 65), Color.White, h4);
        } else
        {
            Engine.DrawTexture(level2, new Vector2(120, 66), size: new Vector2(80, 80));
            Engine.DrawString("2", new Vector2(143, 67), Color.Black, h4);
            Engine.DrawString("2", new Vector2(141, 65), Color.White, h4);
        }
        {
            Engine.DrawTexture(level2, new Vector2(120, 66), size: new Vector2(80, 80));
            Engine.DrawString("2", new Vector2(143, 67), Color.Black, h4);
            Engine.DrawString("2", new Vector2(141, 65), Color.White, h4);
        }

        if (levelState == 3)
        {
            Engine.DrawTexture(level3, new Vector2(213, 62), size: new Vector2(100, 100));
            Engine.DrawString("3", new Vector2(244, 69), Color.Black, h5);
            Engine.DrawString("3", new Vector2(242, 67), Color.Yellow, h5);
        } else if (Game.progress < LevelPassed.twoPassed)
        {
            Engine.DrawTexture(level3BW, new Vector2(223, 66), size: new Vector2(80, 80));
            Engine.DrawString("3", new Vector2(245, 67), Color.Black, h4);
            Engine.DrawString("3", new Vector2(243, 65), Color.White, h4);
        }
        {
            Engine.DrawTexture(level3, new Vector2(223, 66), size: new Vector2(80, 80));
            Engine.DrawString("3", new Vector2(245, 67), Color.Black, h4);
            Engine.DrawString("3", new Vector2(243, 65), Color.White, h4);
        }

        Engine.DrawString("DIFFICULTY:", new Vector2(42, 180), Color.White, h3);

        if (diffState == Difficulty.easy)
        {
            Engine.DrawString("EASY", new Vector2(118, 181), Color.White, h2);
            Engine.DrawString("EASY", new Vector2(117, 180), Color.Yellow, h2);
        } else
        {
            Engine.DrawString("EASY", new Vector2(127, 180), Color.White, h3);
        }

        if (diffState == Difficulty.medium)
        {
            Engine.DrawString("MEDIUM", new Vector2(159, 181), Color.White, h2);
            Engine.DrawString("MEDIUM", new Vector2(158, 180), Color.Yellow, h2);
        } else
        {
            Engine.DrawString("MEDIUM", new Vector2(174, 180), Color.White, h3);
        }

        if (diffState == Difficulty.hard)
        {
            Engine.DrawString("HARD", new Vector2(229, 181), Color.White, h2);
            Engine.DrawString("HARD", new Vector2(228, 180), Color.Yellow, h2);
        } else
        {
            Engine.DrawString("HARD", new Vector2(238, 180), Color.White, h3);
        }
        
        return Scene.levels;
    }

    //Instructions Scene
    public static Scene instructionsScene()
    {
        if (Engine.GetMouseButtonDown(MouseButton.Left) || Engine.GetKeyDown(Key.Space) || Engine.GetKeyDown(Key.I))
        {
            return Scene.start;
        }
        Engine.DrawString("BACK", new Vector2(0, 0), Color.White, h3);
        Engine.DrawString("Press A and D to move", new Vector2(100, 60), Color.White, h3);
        Engine.DrawString("Press SPACE to jump", new Vector2(100, 90), Color.White, h3);
        Engine.DrawString("Collect rings for a speed boost", new Vector2(80, 120), Color.White, h3);
        Engine.DrawString("Press F to fullscreen", new Vector2(90, 150), Color.White, h3);
        return Scene.instructions;
    }


    //End Scene
    public static void endScene(String message)
    {
        //exit game
        if (Engine.GetMouseButtonDown(MouseButton.Left) || Engine.GetKeyDown(Key.Space))
        {
            Game.currentLevel.reset();
            Game.currentScene = Scene.levels;
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
            Game.currentLevel.sb.updateScore(25);
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
        Engine.DrawString(message, new Vector2(line2X, 75), Color.White, h2); //15 , 75
        Engine.DrawString("Act", new Vector2(line3X + 5, 97), Color.Orange, h3); //170, 97
        Engine.DrawString("1", new Vector2(line3X + 30, 72), Color.Yellow, h1); //200, 72

        if (!scoreUpdate)
        {
            bonus = Scoreboard.timeBonus();
            scoreUpdate = true;
        }

        //draw score
        if (message.Equals("PASSED"))
        {
            Engine.DrawString("SCORE", new Vector2(line4x + 5, 125), Color.Yellow, h3); //75, 125
            Engine.DrawString(Game.currentLevel.sb.getScore() + "", new Vector2(line4x + 140, 125), Color.White, h3); //210, 125
            Engine.DrawString("TIME BONUS", new Vector2(line4x + 5, 145), Color.Yellow, h3); //75, 145
            Engine.DrawString("" + bonus, new Vector2(line4x + 140, 145), Color.White, h3); //210, 145
        }

    }

}

