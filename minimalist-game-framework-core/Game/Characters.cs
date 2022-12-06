using System;
using System.Collections.Generic;
using System.Text;

class Sonic : PhysicsSprite
{
    public static readonly int boostFrameTime = 10;
    public static readonly float maxHorVel = 10;
    public static readonly float maxHorVelBoost = 2;
    public static readonly float jumpImpulseMag = 10;
    public static readonly float accelerationMag = 5;
    public static readonly float brakeAccMag = 4;
    public static readonly float accelerationBoostFactor = (float) 1.2;

    private int flows;
    private bool[] recentFlows;
    

    public Sonic(Vector2 loc, Texture sprites, Vector2[] hitboxes):base(loc, sprites, hitboxes)
    {
        flows = 0;
        recentFlows = new bool[boostFrameTime];
        
    }
    public Sonic(Vector2 loc, Texture sprites) : base(loc, sprites)
    {
        flows = 0;
        recentFlows = new bool[boostFrameTime];
        onGround = Game.map.onGround(this.getBotPoint());
    }

    public void jump()
    {
        if (!Game.map.onGround(this.getBotPoint()))
        {
            return;
        }
        this.vel = this.vel + jumpImpulseMag * Game.map.getNormalVector(loc);
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
    }

    public override void updateState()
    {
        base.updateState();
    }
}