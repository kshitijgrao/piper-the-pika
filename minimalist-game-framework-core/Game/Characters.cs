using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

class Sonic : PhysicsSprite
{
    public static readonly float jumpHeight = 50;
    public static readonly int boostFrameTime = 200;
    public static readonly float maxHorVel = 100;
    public static readonly float maxHorVelBoost = 50;
    public static readonly float jumpImpulseMag = (float) Math.Sqrt(2.0 * Physics.g.Length() * jumpHeight);
    public static readonly float accelerationMag = 300;
    public static readonly float brakeAccMag = 150;
    public static readonly float accelerationBoostFactor = (float) 1.3;
    public static readonly float flowerAccBoost = (float) 1.5;

    public static readonly float sonicMass = 2;
    


    private float flows;
    

    public Sonic(Vector2 loc, Texture sprites, Vector2 hitboxes):base(loc, sprites, hitboxes)
    {
        flows = 0;
        this.mass = sonicMass;
    }
    public Sonic(Vector2 loc, Texture sprites) : base(loc, sprites)
    {
        flows = 0;
        onGround = Game.map.onGround(this.getBotPoint());
        this.mass = sonicMass;
    }

    public void jump()
    {
        if (onGround)
        {
            this.vel = this.vel + jumpImpulseMag * Game.map.getNormalVector(loc);
        }
        Console.Write("jumping bruh");
        
    }

    public void setAcceleration(Key key)
    {
        Vector2 tempLoc = this.getBotPoint();
        if(key == Key.D)
        {
            if (onGround)
                this.acc = accelerationMag * Game.map.getNormalVector(tempLoc).Rotated(90);
            else
                this.acc = accelerationMag * (new Vector2(1, 0));
        }
        else if(key == Key.A)
        {
            if(onGround)
               this.acc = accelerationMag * Game.map.getNormalVector(tempLoc).Rotated(270);
            else
                this.acc = accelerationMag * (new Vector2(-1, 0));
        }
        else
        {
            if (Game.map.onGround(tempLoc))
            {
                this.acc = vel.Normalized() * (-1) * brakeAccMag;
            }
                
            else
                this.acc = Vector2.Zero;
        }
        if(flows > 0)
        {
            acc.X *= accelerationBoostFactor;
        }

        if (Game.map.inAir(loc))
        {
            acc.X *= accelerationBoostFactor;
        }

        this.acc += Physics.getPhysicsAcceleration(tempLoc, this.vel);

        //account for weird floating point errors
        this.acc.round(2);
    }

    public override void updateState()
    {
        base.updateState();
        Debug.WriteLine(this.vel.X);
        
        float horVelCap = maxHorVel + (flows > 0 ? maxHorVelBoost : 0);

        if(this.vel.X >= 0)
        {
            this.vel.X = Math.Min(this.vel.X, horVelCap);
        }
        else
        {
            this.vel.X = Math.Max(this.vel.X, -1 * horVelCap);
        }
        Console.WriteLine(this.loc.Y);

       

        if(flows > 0)
           flows -= 1 / ((float) boostFrameTime);
    }

    public void addFlower()
    {
        flows += 1;
    }

}

class Enemy : PhysicsSprite
{
    Bounds2 path; // holds the max and minumum vector displacement (from loc) of an enemy
    float speed = 20;
    public static readonly Texture wolfTexture = Engine.LoadTexture("wolf-enemy-spritemap.png");
    public static readonly Texture hawkTexture = Engine.LoadTexture("hawk-enemy-spritemap.png");
    public static readonly Vector2 wolfHit = new Vector2(40, 34);
    public static readonly Vector2 hawkHit = new Vector2(54, 37);

    public static readonly float killSpeed = 60;

    public Enemy(Vector2 loc, Bounds2 path, bool flying) : base(flying ? loc : (loc + new Vector2(0, 4)), flying ? hawkTexture : wolfTexture, flying ? hawkHit : wolfHit, false)
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

    public override void collide(PhysicsSprite other, float timeLeft)
    {
        Debug.WriteLine("collided");
        if (other is PhysicsSprite) {
            if (((PhysicsSprite)other).vel.Length() > killSpeed)
            {
                Game.sb.enemyKilled(1);
                base.collide(other);
            }
            else
            {
                other.loc += other.vel * (Engine.TimeDelta - timeLeft);
                other.vel = Physics.coeffRestitution * (-1) * (other.vel - this.vel) + this.vel;

                Animator.animatePiperTakingDamage(Game.piper);

                if (Game.gameDifficulty == Game.EASY)
                {
                    if (Scoreboard.flowers > 0)
                    {
                        Scoreboard.flowers = 0;
                    }
                    else
                    {
                        Scoreboard.lives--;
                    }
                }
                else if (Game.gameDifficulty == Game.MEDIUM)
                {
                    Scoreboard.lives--;
                }
                else if (Game.gameDifficulty == Game.HARD)
                {
                    Scoreboard.lives = 0;
                }

                base.collide(other, timeLeft);

            }

        }
    }
}

class Flower : Sprite
{
    public static readonly Vector2 defaultFlowerHitbox = new Vector2(13, 14);
    public static readonly Texture defaultFlower = Engine.LoadTexture("flower.png");
    public Flower(Vector2 loc) : base(loc + defaultFlowerHitbox / 2, defaultFlower, defaultFlowerHitbox)
    {

    }

    public override void collide(Sprite mainCharacter)
    {
        Game.sb.addFlower();
        if (mainCharacter is Sonic)
        {
            ((Sonic)mainCharacter).addFlower();
            if (mainCharacter.isLeft())
            {
                ((Sonic)mainCharacter).vel += new Vector2(-50, 0);
            }
            else
            {
                ((Sonic)mainCharacter).vel += new Vector2(50, 0);
            }
        }
        ((Sonic) mainCharacter).addFlower();
        base.collide(mainCharacter);
    }
}