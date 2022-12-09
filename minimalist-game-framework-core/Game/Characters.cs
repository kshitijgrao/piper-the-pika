using System;
using System.Collections.Generic;
using System.Text;

class Sonic : PhysicsSprite
{
    public static readonly int boostFrameTime = 10;
    public static readonly float maxHorVel = 10;
    public static readonly float maxHorVelBoost = 2;
    public static readonly float jumpImpulseMag = 30;
    public static readonly float accelerationMag = 15;
    public static readonly float brakeAccMag = 8;
    public static readonly float accelerationBoostFactor = (float) 1.2;
    


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

    public void setAcceleration(String key)
    {
        Vector2 tempLoc = this.getBotPoint();
        if(key.Equals(Game.RIGHT))
        {
            this.acc = accelerationMag * Game.map.getNormalVector(tempLoc).Rotated(90);
        }
        else if(key.Equals(Game.LEFT))
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


class Flower : Sprite
{
    public static readonly Vector2 defaultFlowerHitbox = new Vector2(20, 20);
    public static readonly Texture defaultFlower = Engine.LoadTexture("flower.png");
    public Flower(Vector2 loc) : base(loc, defaultFlower, defaultFlowerHitbox)
    {

    }

    public override void collide(Sprite mainCharacter)
    {
        Game.sb.modifyFlowers(1);
        ((Sonic) mainCharacter).addFlower();
        base.collide(mainCharacter);
    }
}