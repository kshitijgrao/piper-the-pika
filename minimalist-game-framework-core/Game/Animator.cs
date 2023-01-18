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
    static Random rng = new Random();

    // animation constants
    static float piperFramerate = 5;
    public static float generalFramerate = 5;
    static float jumpTime = 30;
    public static float sparkleFramesLeft = 0;

    // blink constants
    static int minFramesUntilBlink = 10;
    static int maxFramesUntilBlink = 30;
    static int blinkFrames = 1;

    // variables
    static State movementState = State.Walk;
    static float timeUntilBlink = rng.Next(minFramesUntilBlink, maxFramesUntilBlink);

    public static float animatePiper(Sonic piper, Vector2 position, Key key)
    {
        float currentFrame = piper.getFrameIndex();

        // make piper blink
        checkBlink(piper);

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
            if (piper.getAirTime() > jumpTime)
            {
                piper.setState(State.Spinning);
            }
        }
        else
        {
            piper.addAirTime(-piper.getAirTime());
        }

        if (piper.flows > 0)
        {
            piper.isSparkling = true;
        }
        else
        {
            piper.isSparkling = false;
        }

        return changeFrame(piper, position, 4, piperFramerate);
    }

    public static float animateEnemy(Enemy enemy, Vector2 position)
    {
        Vector2 minPosition = enemy.getPath().Size;
        Vector2 maxPosition = enemy.getPath().Position;

        Debug.WriteLine(minPosition.ToString() + " " + maxPosition.ToString());

        if (enemy.getState() != State.Damage)
        {
            if (enemy.loc.X >= maxPosition.X)
            {
                enemy.setVelocity((minPosition - maxPosition).Normalized() * enemy.getSpeed());
            }
            if (enemy.loc.X <= minPosition.X)
            {
                enemy.setVelocity((maxPosition - minPosition).Normalized() * enemy.getSpeed());
            }
        }
        else if (enemy.getState() == State.Damage)
        {
            if (enemy.isBlinking == true)
            {
                System.Diagnostics.Debug.WriteLine(enemy.getFrameIndex() + " >= 3");
                if (enemy.getFrameIndex() >= 3)
                {
                    enemy.invisible = true;
                    enemy.setState(State.Dead);
                }
                else
                {
                    enemy.updateBlink();
                }
            }
            else
            {
                enemy.setVelocity(Vector2.Zero);
                enemy.setBlinking();
            }
        }

        return changeFrame(enemy, position, enemy.totalFramesInCurrentState, generalFramerate);
    }

    public static float animateFlowers(Flower flower, Vector2 position)
    {
        if (flower.collected == true && flower.invisible == false)
        {
            if (flower.getFrameIndex() >= 3)
            {
                flower.invisible = true;
            }
            return changeFrame(flower, position, 4, 15);
        }
        return changeFrame(flower, position, 1, 8);
    }

    private static float changeFrame(Sprite sprite, Vector2 position, int totalFrames, float Framerate)
    {
        // find frame
        sprite.setFrameIndex((sprite.getFrameIndex() + Engine.TimeDelta * Framerate) % (float)totalFrames);
        float frameIndex = sprite.getFrameIndex();

        // find bounds on spritemap and draw
        Vector2 FrameStart = new Vector2((int)frameIndex * sprite.getHitboxNoCalc().X, (int)sprite.getState() * sprite.getHitboxNoCalc().Y);
        Bounds2 FrameBounds = new Bounds2(FrameStart, sprite.getHitboxNoCalc());
        sprite.draw(FrameBounds, position);

        // return current frame
        return frameIndex;
    }

    private static void checkBlink(Sprite piper)
    {
        timeUntilBlink -= Engine.TimeDelta * piperFramerate;
        if (-blinkFrames < timeUntilBlink && timeUntilBlink <= 0)
        {
            piper.blink(true);
        }
        else if (timeUntilBlink < -blinkFrames)
        {
            piper.blink(false);
            timeUntilBlink = rng.Next(minFramesUntilBlink, maxFramesUntilBlink);
        }
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

    public static void checkTurn(PhysicsSprite sprite)
    {
        if (sprite is Enemy)
        {
            if ((!sprite.isLeft() != sprite.vel.X < 0) && Math.Abs(sprite.vel.X) > 2)
            {
                sprite.turn();
            }
        }
        else
        {
            if ((sprite.isLeft() != sprite.vel.X < 0) && Math.Abs(sprite.vel.X) > 2)
            {
                sprite.turn();
            }
        }
    }

    public static void animateEnemyTakingDamage(Enemy enemy)
    {
        enemy.setState(State.Damage);
        enemy.setFrameIndex(0);
        enemy.changeLocked(true);
        enemy.totalFramesInCurrentState = 7;
    }

    public static void changeFramerate(float framerate)
    {
        piperFramerate = framerate;
    }
}
