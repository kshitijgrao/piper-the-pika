using System;
using System.Collections.Generic;
using System.Text;


/**
 *   Animator is a static class that handles all character animation
 *   (including Piper, Hawk, and the enemies)
 */
internal static class Animator
{
    // animation constants
    static readonly float Framerate = 5;

    public static float animatePiper(Sprite piper, float speed, float frameIndex)
    {
        // player input
        if (Engine.GetKeyHeld(Key.A) || Engine.GetKeyHeld(Key.Left))
        {
            piper.setState(1);
            piper.move(new Vector2(-speed, 0));
            if (!piper.isLeft())
            {
                piper.turn();
            }
        }
        else if (Engine.GetKeyHeld(Key.D) || Engine.GetKeyHeld(Key.Right))
        {
            piper.setState(1);
            piper.move(new Vector2(speed, 0));
            if (piper.isLeft())
            {
                piper.turn();
            }
        }
        else
        {
            piper.setState(0);
        }

        frameIndex = (frameIndex + Engine.TimeDelta * Framerate) % 4.0f;
        Vector2 piperFrame = new Vector2((int)frameIndex * 24, piper.getState() * 24);
        Bounds2 piperFrameBounds = new Bounds2(piperFrame, new Vector2(24, 24));
        piper.draw(piperFrameBounds);
        return frameIndex;
    }
}
