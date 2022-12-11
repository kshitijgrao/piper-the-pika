using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


/**
 *   Animator is a static class that handles all character animation
 *   (including Piper and the enemies)
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
            else if ((int)Math.Abs(piper.vel.X) == 0)
            {
                piper.setState(0);
            }
            else
            {
                piper.setState(movementState);
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
        else if (piper.getState() == 5 && (int)currentFrame == 1) // landing lasts 1 frames
        {
            piper.changeLocked(false);
        }
        
        return changeFrame(piper, position, 4);
    }

    public static float animateEnemy(Enemy enemy, Vector2 position)
    {
        Vector2 maxPosition = enemy.getPath().Size;
        Vector2 minPosition = enemy.getPath().Position;
        if (!enemy.isLeft()) // CURRENTLY BROKEN. IF ENEMY "IS LEFT" THEY ARE ACTUALLY FACING RIGHT. THIS WILL BE FIXED LATER
        {
            if (enemy.loc.X > minPosition.X)
            {
                enemy.setVelocity(new Vector2(-enemy.getSpeed(), 0));
            } else
            {
                enemy.turn();
            }
        }
        else
        {
            if (enemy.loc.X < maxPosition.X)
            {
                enemy.setVelocity(new Vector2(enemy.getSpeed(), 0));
            }
            else
            {
                enemy.turn();
            }
        }
        return changeFrame(enemy, position, 5);
    }

    private static float changeFrame(Sprite sprite, Vector2 position, int totalFrames)
    {
        // find frame
        sprite.setFrameIndex((sprite.getFrameIndex() + Engine.TimeDelta * Framerate) % (float)totalFrames);
        float frameIndex = sprite.getFrameIndex();

        // find bounds on spritemap and draw
        Vector2 piperFrameStart = new Vector2((int)frameIndex * sprite.getHitboxNoCalc().X, sprite.getState() * sprite.getHitboxNoCalc().Y);
        Bounds2 piperFrameBounds = new Bounds2(piperFrameStart, sprite.getHitboxNoCalc());
        sprite.draw(piperFrameBounds, position);

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
    public static void setPiperSpinning(Boolean isSpinning, Sprite piper)
    {
        if (isSpinning)
        {
            piper.setState(4);
            piper.changeLocked(true);
        }
        else
        {
            piper.setState(5);
            //piper.changeLocked(false);
        }
    }

    public static void checkPiperTurn(Sonic piper)
    {
        if (piper.isLeft() != piper.vel.X < 0)
        {
            piper.turn();
        }
    }
}
