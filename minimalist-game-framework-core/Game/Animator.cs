using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;


/**
 *   Animator is a static class that handles all character animation
 *   (including Piper and the enemies)
 */
internal static class Animator
{
    // animation speed
    static float Framerate = 5;

    // variables
    static State movementState = State.Walk;

    public static float animatePiper(Sonic piper, Vector2 position, Key key)
    {
        float currentFrame = piper.getFrameIndex();

        // only changes state if piper if not spinning or taking damage (aka not locked)
        if (!piper.animationIsLocked())
        {
            if (key.Equals(Key.Space))
            {
                piper.setState(State.StartingJump);
                piper.setFrameIndex(0);
                piper.changeLocked(true);
            }
            else if ((int)Math.Abs(piper.vel.X) == 0 && !key.Equals(Key.A) && !key.Equals(Key.D))
            {
                piper.setState(0);
            }
            else
            {
                piper.setState(movementState);
            }
        } 
        else if (piper.getState() == State.StartingJump && (int)currentFrame == 3) // spin starts on frame 3
        {
            piper.setState(State.Spinning);
        } 
        else if (piper.getState() == State.Damage && (int)currentFrame == 2) // damage lasts 2 frames 
        {
            piper.changeLocked(false);
        } 
        else if (piper.getState() == State.Landing && (int)currentFrame == 1) // landing lasts 1 frames
        {
            piper.changeLocked(false);
        }
        if (!piper.onGround)
        {
            piper.addAirTime(1);
            if (piper.getAirTime() > 50)
            {
                piper.setState(State.Spinning);
            }
        }
        else
        {
            piper.addAirTime(-piper.getAirTime());
        }

        return changeFrame(piper, position, 4);
    }

    public static float animateEnemy(Enemy enemy, Vector2 position)
    {
        Vector2 maxPosition = enemy.getPath().Size;
        Vector2 minPosition = enemy.getPath().Position;
        if (!enemy.isLeft()) // TODO: CURRENTLY INCORRECT. IF ENEMY "IS LEFT" THEY ARE ACTUALLY FACING RIGHT. THIS WILL BE FIXED LATER
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
        Vector2 piperFrameStart = new Vector2((int)frameIndex * sprite.getHitboxNoCalc().X, (int)sprite.getState() * sprite.getHitboxNoCalc().Y);
        Bounds2 piperFrameBounds = new Bounds2(piperFrameStart, sprite.getHitboxNoCalc());
        sprite.draw(piperFrameBounds, position);

        // return current frame
        return frameIndex;
    }

    public static void animatePiperLanding(Sprite piper)
    {
        piper.setState(State.Landing);
        piper.setFrameIndex(0);
    }

    public static void animatePiperTakingDamage(Sprite piper)
    {
        piper.setState(State.Damage);
        piper.setFrameIndex(0);
        piper.changeLocked(true);
    }

    public static void setPiperSprinting(Boolean isSprinting)
    {
        if (isSprinting)
        {
            movementState = State.Sprint;
        } 
        else
        {
            movementState = State.Walk;
        }
    }
    public static void setPiperSpinning(Boolean isSpinning, Sprite piper)
    {
        if (isSpinning)
        {
            piper.setState(State.Spinning);
            piper.changeLocked(true);
        }
        else
        {
            piper.setState(State.Landing);
        }
    }

    public static void checkPiperTurn(Sonic piper)
    {
        if (piper.isLeft() != piper.vel.X < 0)
        {
            piper.turn();
        }
    }

    public static void changeFramerate(float framerate)
    {
        Framerate = framerate;
    }
}
