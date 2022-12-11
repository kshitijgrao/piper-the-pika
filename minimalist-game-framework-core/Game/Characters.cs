using System;
using System.Collections.Generic;
using System.Text;

class Sonic : PhysicsSprite
{
    public static readonly int boostFrameTime = 10;
    public static readonly float maxHorVel = 10;
    public static readonly float maxHorVelBoost = 2;
    public static readonly float jumpImpulseMag = 50;
    public static readonly float accelerationMag = 30;
    public static readonly float brakeAccMag = 20;
    public static readonly float accelerationBoostFactor = (float) 2;
    


    private int flows;
    

    public Sonic(Vector2 loc, Texture sprites, Vector2 hitboxes):base(loc, sprites, hitboxes)
    {
        flows = 0;
        
    }
    public Sonic(Vector2 loc, Texture sprites) : base(loc, sprites)
    {
        flows = 0;
        onGround = Game.map.onGround(this.getBotPoint());
    }

    public void jump()
    {
        if (onGround)
        {
            this.vel = this.vel + jumpImpulseMag * Game.map.getNormalVector(loc);
        }
        
    }

    public void setAcceleration(Key key)
    {
        Vector2 tempLoc = this.getBotPoint();
        if(key == Key.D)
        {
            this.acc = accelerationMag * Game.map.getNormalVector(tempLoc).Rotated(90);
        }
        else if(key == Key.A)
        {
            this.acc = accelerationMag * Game.map.getNormalVector(tempLoc).Rotated(270);
        }
        else
        {
            if (Game.map.onGround(tempLoc))
                this.acc = vel.Normalized() * (-1) * brakeAccMag;
            else
                this.acc = Vector2.Zero;
        }

        if (Game.map.inAir(loc))
        {
            acc.X *= accelerationBoostFactor;
        }

        this.acc += Physics.getPhysicsAcceleration(tempLoc, this.vel);

        System.Diagnostics.Debug.WriteLine(this.acc.ToString());
        //account for weird floating point errors
        this.acc.round(2);
    }

    public override void updateState()
    {
        base.updateState();
        flows *= 2;
        flows %= ((int)Math.Pow(2, boostFrameTime));
    }

    public void addFlower()
    {

    }
}

class Enemy : PhysicsSprite
{
    Bounds2 path; // holds the max and minumum vector displacement (from loc) of an enemy
    float speed = 20;

    public Enemy(Vector2 loc, Texture sprites, Vector2 hitboxes, Bounds2 path) : base(loc, sprites, hitboxes)
    {
        this.path = path;
    }

    public Enemy(Vector2 loc, Texture sprites, Bounds2 path) : base(loc, sprites)
    {
        this.path = path;
    }
    public Enemy(Vector2 loc, Texture sprites, Bounds2 path, float speed) : base(loc, sprites)
    {
        this.path = path;
        this.speed = speed;
    }


    public Bounds2 getPath()
    {
        return path;
    }

    public float getSpeed()
    {
        return speed;
    }
}

class Flower : Sprite
{
    public static readonly Vector2 defaultFlowerHitbox = new Vector2(13, 14);
    public static readonly Texture defaultFlower = Engine.LoadTexture("flower.png");
    public Flower(Vector2 loc) : base(loc, defaultFlower, defaultFlowerHitbox)
    {

    }

    public override void collide(Sprite mainCharacter)
    {
        Game.sb.addFlower();
        ((Sonic) mainCharacter).addFlower();
        base.collide(mainCharacter);
    }
}