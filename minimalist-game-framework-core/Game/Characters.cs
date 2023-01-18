using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

class Sonic : PhysicsSprite
{
    public static readonly float jumpHeight = 100;
    public static readonly int boostFrameTime = 250;
    public static readonly float maxHorVel = 250;
    public static readonly float maxHorVelBoost = 100;
    public static readonly float jumpImpulseMag = (float) Math.Sqrt(2.0 * Physics.g.Length() * jumpHeight);
    public static readonly float accelerationMag = 300;
    public static readonly float brakeAccMag = 150;
    public static readonly float accelerationBoostFactor = (float) 1.3;
    public static readonly float flowerAccBoost = (float) 1.5;

    public static readonly Vector2 sonicBox = new Vector2(20, 20);

    public static readonly float sonicMass = 2;

    private float flows;
    private int flowerCount;


    public int enemiesKilled;

    public static readonly Texture piperTexture = Engine.LoadTexture("piper-spritemap.png");
    public static readonly Texture piperTextureBlink = Engine.LoadTexture("piper-spritemap-blink.png");

    public Sonic(Vector2 loc, Texture sprites, Vector2 hitboxes):base(loc, sprites, hitboxes, sonicBox)
    {
        flows = 0;
        flowerCount = 0;
        this.mass = sonicMass;
    }

    public Sonic(Vector2 loc):base(loc, piperTexture, piperTextureBlink, sonicBox)
    {
        flows = 0;
        flowerCount = 0;
        this.mass = sonicMass;
    }

    public void jump()
    {
        if (onGround)
        {
            this.vel = this.vel + jumpImpulseMag * Game.currentLevel.getMap().getNormalVector(loc);
        }
        
    }

    public void setAcceleration(Key key, Map map)
    {
        if(key == Key.D)
        {
            if (onPath)
                accPath = accelerationMag + accelerationMag * this.currPath.getBoost(fractionOfPath);
            else if (onGround)
                this.acc = accelerationMag * map.getNormalVector(loc).Rotated(90);
            else
                this.acc = accelerationMag * (new Vector2(1, 0));
        }
        else if(key == Key.A)
        {
            if (onPath)
               accPath = -1 * accelerationMag + accelerationMag * this.currPath.getBoost(fractionOfPath);
            else if (onGround)
               this.acc = accelerationMag * map.getNormalVector(loc).Rotated(270);
            else
                this.acc = accelerationMag * (new Vector2(-1, 0));
        }
        else
        {
            if (onPath)
            {
                accPath = (velPath > 0 ? -1 : 1) * Math.Min(brakeAccMag, vel.Length() / Engine.TimeDelta) +  accelerationMag * this.currPath.getBoost(fractionOfPath);
            }
            else if (onGround)
            {
                this.acc = vel.Normalized() * (-1) * Math.Min(brakeAccMag, vel.Length() / Engine.TimeDelta);
            }
            else
            {
                this.acc = Vector2.Zero;
            }
        }
        if(flows > 0)
        {
            acc *= accelerationBoostFactor;
        }

        if (map.inAir(loc))
        {
            acc.X *= accelerationBoostFactor;
        }
        if (onPath) 
        {
            accPath += Physics.getPhysicsAcceleration(this, this.loc, this.vel, map).X;
        }
        else
        {
            this.acc += Physics.getPhysicsAcceleration(this, this.loc, this.vel, map);
        }



        //account for weird floating point errors
        accPath = (float) Math.Round(accPath, 2);
        this.acc.round(2);

    }

    public override void updateState(Map map)
    {
        float horVelCap = maxHorVel + (flows > 0 ? maxHorVelBoost : 0);

        if (this.vel.X >= 0)
        {
            this.vel.X = Math.Min(this.vel.X, horVelCap);
        }
        else
        {
            this.vel.X = Math.Max(this.vel.X, -1 * horVelCap);
        }
        base.updateState(map);
        
        if(flows > 0)
           flows -= 1 / ((float) boostFrameTime);
    }

    public override void collideSpike(float timeLeft)
    {
        base.collideSpike(timeLeft);

        Animator.animatePiperTakingDamage(this);
        Difficulty currDiff = Game.currentLevel.diff;

        if (currDiff == Difficulty.easy)
        {
            if (Game.currentLevel.sb.flowers > 0)
            {
                Game.currentLevel.sb.flowers = 0;
            }
            else
            {
                Game.currentLevel.sb.lives--;
            }
        }
        else if (currDiff == Difficulty.medium)
        {
            Game.currentLevel.sb.lives--;
        }
        else if (currDiff == Difficulty.hard)
        {
            Game.currentLevel.sb.lives = 0;
        }

    }

    public void addFlower()
    {
        flows += 1;
        flowerCount += 1;
    }

    public int getFlowers()
    {
        return flowerCount;
    }
}

class Enemy : PhysicsSprite
{
    Bounds2 path; // holds the max and minumum vector displacement (from loc) of an enemy
    float speed = 20;
    public static readonly Texture wolfTexture = Engine.LoadTexture("wolf-enemy-spritemap-3.png");
    public static readonly Texture hawkTexture = Engine.LoadTexture("hawk-enemy-spritemap-3.png");
    public static readonly Vector2 wolfHit = new Vector2(40, 34);
    public static readonly Vector2 hawkHit = new Vector2(54, 37);

    public static readonly Vector2 wolfCollisionHit = new Vector2(30, 24);
    public static readonly Vector2 hawkCollisionHit = new Vector2(40, 20);

    public static readonly float killSpeed = 150;
    public int totalFramesInCurrentState = 5;

    // animation variables
    public Boolean isBlinking = false;
    public float framesBlinked = 0; // time spent blinking (in frames)
    public float additionUntilNextBlinkFrame = 0;
    public float nextBlinkFrame = 0;

    public Enemy(Vector2 loc, Bounds2 path, bool flying) : base(loc, flying ? hawkTexture : wolfTexture, flying ? hawkHit : wolfHit, false, flying ? hawkCollisionHit : wolfCollisionHit)
    {
        this.path = path;
        this.mass = 10;
    }


    //public Enemy(Vector2 loc, Texture sprites, Bounds2 path, bool flying) : base(loc, flying ? hawkTexture : wolfTexture, false)
    //{
    //    this.path = path;
    //}
    //public Enemy(Vector2 loc, Texture sprites, Bounds2 path, float speed, bool flying) : base(loc, flying ? hawkTexture : wolfTexture)
    //{
    //    this.path = path;
    //    this.speed = speed;
    //}


    public Bounds2 getPath()
    {
        return path;
    }

    public float getSpeed()
    {
        return speed;
    }

    public void setBlinking()
    {
        isBlinking = true;
        framesBlinked = 0;
        additionUntilNextBlinkFrame = 0.8f;
        nextBlinkFrame += additionUntilNextBlinkFrame;
    }

    public void updateBlink()
    {
        framesBlinked += Engine.TimeDelta * Animator.generalFramerate;
        System.Diagnostics.Debug.WriteLine(framesBlinked + " >= " + nextBlinkFrame);
        if (framesBlinked >= nextBlinkFrame)
        {
            additionUntilNextBlinkFrame *= 0.8f ;
            nextBlinkFrame += additionUntilNextBlinkFrame;
            this.invisible = !this.invisible;
        }
    }

    public override void collide(PhysicsSprite other, float timeLeft)
    {
        if (other is PhysicsSprite) {
            if (other.isSpinning && base.getState() != State.Damage)
            {
                Game.currentLevel.sb.enemyKilled(1);
                base.collide(other);
                base.setFrameIndex(0);

                // bounce piper upwards
                other.loc += other.vel * (Engine.TimeDelta - timeLeft);
                other.vel = Physics.coeffRestitution * new Vector2(0, -30);
            }
            else if (base.getState() != State.Damage)
            {
                other.loc += other.vel * (Engine.TimeDelta - timeLeft);
                other.vel = Physics.coeffRestitution * (-1) * (other.vel - this.vel) + this.vel;
                other.setInvincible();

                Animator.animatePiperTakingDamage(other);
                Difficulty currDiff = Game.currentLevel.diff;

                if (currDiff == Difficulty.easy)
                {
                    if (Game.currentLevel.sb.flowers > 0)
                    {
                        Game.currentLevel.sb.flowers = 0;
                    }
                    else
                    {
                        Game.currentLevel.sb.lives--;
                    }
                }
                else if (currDiff == Difficulty.medium)
                {
                    Game.currentLevel.sb.lives--;
                }
                else if (currDiff == Difficulty.hard)
                {
                    Game.currentLevel.sb.lives = 0;
                }

                base.collide(other, timeLeft);

            }

        }
    }
}

class Flower : Sprite
{
    public static readonly Vector2 defaultFlowerHitbox = new Vector2(13, 16);
    public static readonly Vector2 collisionFlowerHitbox = new Vector2(10, 10);
    public static readonly Texture defaultFlower = Engine.LoadTexture("flower-3.png");
    public Boolean collected = false;

    public Flower(Vector2 loc) : base(loc + defaultFlowerHitbox / 2, defaultFlower, defaultFlowerHitbox)
    {

    }

    public override void collide(Sprite mainCharacter)
    {
        if (!collected)
        {
            collected = true;
            Game.currentLevel.sb.addFlower();
            if (mainCharacter is Sonic)
            {
                ((Sonic)mainCharacter).addFlower();
            }
            mainCharacter.isSparkling = true;
            Animator.sparkleFramesLeft = 5;
            base.collide(mainCharacter);
        }
    }
}