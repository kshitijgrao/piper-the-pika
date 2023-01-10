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
    private int invincibleFramesLeft;

    public static readonly float invincibleTime = 1;

    internal bool isSpinning;


    //TODO: clean up these constructors
    public PhysicsSprite(Vector2 loc, Texture sprites, Vector2 hitboxes) : base(loc,sprites,hitboxes)
    {
        vel = new Vector2(0, 0);
        acc = new Vector2(0, 0);
        collided = false;
        timeLeft = 0;
        airTime = 0;
        onGround = Game.map.onGround(loc);
        isSpinning = false;
    }

    public PhysicsSprite(Vector2 loc, Texture sprites, Vector2 hitboxes, bool onGround) : base(loc, sprites, hitboxes)
    {
        vel = new Vector2(0, 0);
        acc = new Vector2(0, 0);
        collided = false;
        timeLeft = 0;
        this.onGround = false;
        isSpinning = false;
    }

    public PhysicsSprite(Vector2 loc, Texture sprites) : base(loc, sprites)
    {
        vel = new Vector2(0, 0);
        acc = new Vector2(0, 0);
        collided = false;
        timeLeft = 0;
        airTime = 0;
        onGround = Game.map.onGround(loc);
        isSpinning = false;
    }

    public override bool notCollidable()
    {
        return (invincibleFramesLeft > 0) || base.notCollidable();
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

    public void setInvincible()
    {
        invincibleFramesLeft = (int)Math.Round(invincibleTime / Engine.TimeDelta);
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

        //checks if its leaving the ground in some way--maybe this might not work in some edge cases... will have to rethink
        if (onGround && Game.map.inAir(loc - Game.map.getNormalVector(locOrig)))
        {
            onGround = false;
            isSpinning = true;
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

        //subtract invisible frames
        if(invincibleFramesLeft > 0)
        {
            invincibleFramesLeft -= 1;
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
        isSpinning = false;
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
