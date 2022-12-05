using System;
using System.Collections.Generic;
using System.Text;

class Sonic : PhysicsSprite
{
    public static readonly int boostFrameTime = 10;
    public static readonly float maxHorVel = 6;
    public static readonly float maxHorVelBoost = 2;
    public static readonly float jumpImpulseMag = 3;
    public static readonly float accelerationMag = 1;
    public static readonly float brakeAccMag = 1;
    public static readonly float accelerationBoostFactor = 1.2;

    private int flows;
    private bool[] recentFlows;

    public Sonic(Vector2 loc, Texture sprites, Vector2[] hitboxes):base(loc, sprites, hitboxes)
    {
        flows = 0;
        recentFlows = new bool[boostFrameTime];
    }

    public void jump()
    {
        vel = vel + jumpImpulseMag * Game.map.getNormalVector(loc);
    }

    public void setAcceleration(String key)
    {
        loc = this.getBotPoint();
        if(key == Game.RIGHT)
        {
            acc = accelerationMag * Game.map.getNormalVector(loc).Rotated(90);
        }
        else if(key == Game.LEFT)
        {
            acc = accelerationMag * Game.map.getNormalVector(loc).Rotated(270);
        }
        else
        {
            if (Game.map.onGround(loc))
                acc = vel.Normalized() * (-1) * brakeAccMag;
            else
                acc = Vector2.Zero;
        }

        if (Game.map.inAir(loc))
        {
            acc *= accelerationBoostFactor;
        }

        acc += Physics.getPhysicsAcceleration(loc, vel);
    }

    public override void updateState()
    {
        base.updateState();
        base.keepOnSurface();
    }
}