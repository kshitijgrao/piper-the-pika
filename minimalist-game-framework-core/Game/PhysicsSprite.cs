using System;
using System.Collections.Generic;
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

    public PhysicsSprite(Vector2 loc, Texture sprites, Vector2 hitboxes) : base(loc,sprites,hitboxes)
    {
        vel = new Vector2(0, 0);
        acc = new Vector2(0, 0);
        collided = false;
        timeLeft = 0;
        airTime = 0;
        onGround = Game.map.onGround(this.getBotPoint());
    }

    public PhysicsSprite(Vector2 loc, Texture sprites, Vector2 hitboxes, bool onGround) : base(loc, sprites, hitboxes)
    {
        vel = new Vector2(0, 0);
        acc = new Vector2(0, 0);
        collided = false;
        timeLeft = 0;
        this.onGround = false;
    }

    public PhysicsSprite(Vector2 loc, Texture sprites) : base(loc, sprites)
    {
        vel = new Vector2(0, 0);
        acc = new Vector2(0, 0);
        collided = false;
        timeLeft = 0;
        airTime = 0;
        onGround = Game.map.onGround(this.getBotPoint());
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
        if (onGround && Game.map.inAir(loc))
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
        vel = vel - Game.map.getNormalVector(loc) * Vector2.Dot(Game.map.getNormalVector(loc), vel);
        collided = true;
        this.timeLeft = timeLeft;
    }

    //holy cow clean up the spaghetti code here
    public void keepOnSurface()
    {
        Vector2 pos = this.loc;
        if (onGround)
        {
            float shift = (Game.map.getSurfaceY(pos) - pos.Y);
            if (Math.Abs(shift) > 10)
                return;
            this.loc.Y += shift;

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
