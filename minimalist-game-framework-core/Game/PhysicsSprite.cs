using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

class PhysicsSprite : Sprite
{
    internal readonly float sprintSpeed = 200;

    public float mass;
    public Vector2 vel;
    public Vector2 acc;
    public float airTime;

    private bool collided;
    private float timeLeft;
    internal bool onGround;
    internal bool onPath;
    internal Path2 currPath;
    internal float fractionOfPath;
    internal float velPath;
    internal float accPath;

    public PhysicsSprite(Vector2 loc, Texture sprites, Vector2 hitboxes) : base(loc,sprites,hitboxes)
    {
        vel = new Vector2(0, 0);
        acc = new Vector2(0, 0);
        collided = false;
        timeLeft = 0;
        airTime = 0;
        onGround = Game.map.onGround(this.getBotPoint());
        currPath = null;
        fractionOfPath = 0;
    }

    public PhysicsSprite(Vector2 loc, Texture sprites, Vector2 hitboxes, bool onGround) : base(loc, sprites, hitboxes)
    {
        vel = new Vector2(0, 0);
        acc = new Vector2(0, 0);
        collided = false;
        timeLeft = 0;
        this.onGround = false;
        currPath = null;
        fractionOfPath = 0;
    }

    public PhysicsSprite(Vector2 loc, Texture sprites) : base(loc, sprites)
    {
        vel = new Vector2(0, 0);
        acc = new Vector2(0, 0);
        collided = false;
        timeLeft = 0;
        airTime = 0;
        onGround = Game.map.onGround(this.getBotPoint());
        currPath = null;
        fractionOfPath = 0;
    }

    public void setVelocity(Vector2 vel)
    {
        this.vel = vel;
    }

    public void setAcceleration(Vector2 acc)
    {
        this.acc = acc;
    }

    public float getAirTime()
    {
        return airTime;
    }

    public void addAirTime(float time)
    {
        airTime+= time;
    }

    public void setAccelerationDirect(Vector2 acc)
    {
        this.acc = acc;
    }

    //visualization method
    public void drawVectors(Vector2 start)
    {
        Engine.DrawLine(start, start + vel, Color.Black);
    }

    public override void updateState()
    {
        Vector2 locOrig = loc;
        
        //with this implementation one frame is kind of glitched
        if (onPath)
        {
            fractionOfPath = currPath.getNextFraction(fractionOfPath, velPath * Engine.TimeDelta);

            velPath += accPath * Engine.TimeDelta;


            if(fractionOfPath > 1 || (fractionOfPath == 1 & velPath > 0))
            {
                fractionOfPath = 1;
                onPath = false;
            }
            if(fractionOfPath < 0 || (fractionOfPath == 0 & velPath < 0))
            {
                fractionOfPath = 0;
                onPath = false;
            }


            loc = currPath.getPoint(fractionOfPath);

            Vector2 currTangent = currPath.getTangent(fractionOfPath);
            float curvature = currPath.getCurvature(fractionOfPath);

            vel = currTangent * velPath;
            acc = currTangent * accPath + (currTangent).Rotated(270) * velPath * velPath * curvature;


            if(velPath * velPath * curvature < Vector2.Dot(Physics.g, currTangent.Rotated(270)))
            {
                onPath = false;
            }

        }
        else
        {
            if (collided)
            {
                loc = loc + vel * timeLeft;
                vel += acc * timeLeft;
                collided = false;
            }
            else
            {
                loc = loc + vel * Engine.TimeDelta;
                vel += acc * Engine.TimeDelta;
            }
        }
        

        //checks if its leaving the ground in some way--maybe this might not work in some edge cases... will have to rethink
        if (onGround && Game.map.inAir(loc - Game.map.getNormalVector(locOrig)))
        {
            onGround = false;
        }


        // sprint if moving fast enough
        if (vel.Length() > sprintSpeed)
        {
            Animator.setPiperSprinting(true);
        } 
        else
        {
            Animator.setPiperSprinting(false);
        }
        keepOnSurface();
    }

    public override void collide(Sprite other)
    {
        base.collide(other);
    }

    public override void collide(PhysicsSprite other, float timeLeft)
    {
        collided = true;
        this.timeLeft = timeLeft;
    }

    public void collideGround(float timeLeft)
    {
        collideSolid(timeLeft);

        onGround = true;
        this.setState(Sprite.landState);
        if (airTime > 50)
        {
            Animator.animatePiperLanding(this);
        }

    }

    public void collideSolid(float timeLeft)
    {
        
        collided = true;
        this.timeLeft = timeLeft;
    }

    //holy cow clean up the spaghetti code here
    public void keepOnSurface()
    {
        Vector2 pos = this.loc;
        if (onGround)
        {
            Vector2 newLoc = Game.map.getNearestHoveringPoint(pos);
            if ((newLoc - pos).Length() > 10)
                return;
            this.loc = newLoc;

            vel = vel - Game.map.getNormalVector(loc) * Vector2.Dot(vel, Game.map.getNormalVector(loc));

        }
        /*
        else if (Game.map.onGround(pos))
        {
            float shift = (Game.map.getSurfaceY(pos) - pos.Y);
            if (Math.Abs(shift) > 10)
                return;
            this.loc.Y += shift;

            vel = vel - Game.map.getNormalVector(loc) * Vector2.Dot(vel, Game.map.getNormalVector(loc));
        }*/
    }

}
