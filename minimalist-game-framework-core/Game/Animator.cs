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

    // variables
    static int movementState = 1;

    public static float animatePiper(Sonic piper, Vector2 position, Key key)
    {
        float currentFrame = piper.getFrameIndex();

        // only changes state if piper if not spinning or taking damage (aka not locked)
        // states --> idle 0, walk 1, sprint 2, starting jump 3, spining 4, landing 5, damage 6
        if (!piper.animationIsLocked())
        {
            if (key.Equals(Key.Space))
            {
                piper.setState(3);
                piper.setFrameIndex(0);
                piper.changeLocked(true);
            }
            else if (key.Equals(Key.A) || key.Equals(Key.D))
            {
                piper.setState(movementState);
                if (piper.isLeft() != piper.vel.X < 0)
                {
                    piper.turn();
                }
            }
            else
            {
                piper.setState(0);
            }
        } 
        else if (piper.getState() == 3 && (int)currentFrame == 3) // spin starts on frame 3
        {
            piper.setState(4);
        } 
        else if (piper.getState() == 6 && (int)currentFrame == 2) // damage lasts 2 frames 
        {
            piper.changeLocked(false);
        } 
        else if (piper.getState() == 5 && (int)currentFrame == 2) // landing lasts 2 frames
        {
            piper.changeLocked(false);
        }
        
        return changeFrame(piper, position);
    }

    private static float changeFrame(Sprite piper, Vector2 position)
    {
        // find frame
        piper.setFrameIndex((piper.getFrameIndex() + Engine.TimeDelta * Framerate) % 4.0f);
        float frameIndex = piper.getFrameIndex();

        // find bounds on spritemap and draw
        Vector2 piperFrameStart = new Vector2((int)frameIndex * 24, piper.getState() * 24);
        Bounds2 piperFrameBounds = new Bounds2(piperFrameStart, new Vector2(24, 24));
        piper.draw(piperFrameBounds, position);

        // return current frame
        return frameIndex;
    }

    public static void animatePiperLanding(Sprite piper)
    {
        piper.setState(5);
        piper.setFrameIndex(0);
    }

    public static void animatePiperTakingDamage(Sprite piper)
    {
        piper.setState(6);
        piper.setFrameIndex(0);
        piper.changeLocked(true);
    }

    public static void setPiperSprinting(Boolean isSprinting)
    {
        if (isSprinting)
        {
            movementState = 2;
        } 
        else
        {
            movementState = 1;
        }
    }
}
