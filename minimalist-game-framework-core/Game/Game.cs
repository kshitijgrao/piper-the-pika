using System;
using System.Collections.Generic;
using System.Collections;

class Game
{
    public static readonly string Title = "Piper the Pika";
    public static readonly Vector2 Resolution = new Vector2(320, 224);
    Texture piperTexture = Engine.LoadTexture("pika-with-dots");

    // animation constants
    static readonly float Framerate = 30;
    static readonly float WalkSpeed = 50;

    // sprites
    Sprite piper;
    float piperFrameIndex;
    bool piperFaceLeft = false;
    ArrayList sprites = new ArrayList();

    Scoreboard sb;
    float speed = 5;

    public Game()
    {
        //scoreboard
        sb = new Scoreboard();

        // create piper sprite
        Vector2[] piperHB = new Vector2[1];
        piperHB[0] = new Vector2(24, 24);
        piper = new Sprite(Resolution / 2, piperTexture, piperHB);
        sprites.Add(piper);
    }

    public void Update()
    {
        // player input
        if (Engine.GetKeyHeld(Key.A))
        {
            piper.setState(1);
            piperFaceLeft = true;
            piper.move(new Vector2(-speed, 0));
        }
        else if (Engine.GetKeyHeld(Key.D))
        {
            piper.setState(1);
            piperFaceLeft = false;
            piper.move(new Vector2(speed, 0));
        }
        else
        {
            piper.setState(0);
        }

        piperFrameIndex = (piperFrameIndex + Engine.TimeDelta * Framerate) % 4.0f;
        Vector2 piperFrame = new Vector2((int)piperFrameIndex * 24, piper.getState() * 24);
        Bounds2 piperFrameBounds = new Bounds2(piperFrame, piperFrame + new Vector2(24, 24));
        
        // animate each sprite onscreen
        //foreach (Sprite s in sprites)
        //{
            //Animator.draw(s);
        //}

        sb.updateScoreboard();
    }
}
