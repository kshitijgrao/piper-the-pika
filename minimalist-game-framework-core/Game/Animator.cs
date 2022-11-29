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
        if (Engine.GetKeyHeld(Key.A))
        {
            piper.setState(1);
            piper.move(new Vector2(-speed, 0));
        }
        else if (Engine.GetKeyHeld(Key.D))
        {
            piper.setState(1);
            piper.move(new Vector2(speed, 0));
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
