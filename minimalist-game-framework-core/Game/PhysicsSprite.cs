using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

class PhysicsSprite : Sprite
{
    internal readonly float sprintSpeed = 230;

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
    private int invincibleFramesLeft;

    public static readonly float invincibleTime = 1;

    internal bool isSpinning;

    public Vector2 collisionBox;


    //TODO: clean up these constructors
    public PhysicsSprite(Vector2 loc, Texture sprites, Vector2 hitboxes, Vector2 collisionBox) : base(loc,sprites,hitboxes)
    {
        vel = new Vector2(0, 0);
        acc = new Vector2(0, 0);
        collided = false;
        timeLeft = 0;
        airTime = 0;
        onGround = false;
        isSpinning = false;
        currPath = null;
        fractionOfPath = 0;

        this.collisionBox = collisionBox;
    }

    public PhysicsSprite(Vector2 loc, Texture spritemap, Texture blinkmap, Vector2 collisionBox) : base(loc, spritemap, blinkmap)
    {
        vel = new Vector2(0, 0);
        acc = new Vector2(0, 0);
        collided = false;
        timeLeft = 0;
        airTime = 0;
        onGround = false;
        isSpinning = false;
        currPath = null;
        fractionOfPath = 0;
        this.collisionBox = collisionBox;
    }

    public PhysicsSprite(Vector2 loc, Texture sprites, Vector2 hitboxes, bool onGround, Vector2 collisionBox) : base(loc, sprites, hitboxes)
    {
        vel = new Vector2(0, 0);
        acc = new Vector2(0, 0);
        collided = false;
        timeLeft = 0;
        this.onGround = false;
        isSpinning = false;
        currPath = null;
        fractionOfPath = 0;
        this.collisionBox = collisionBox;
    }

    public PhysicsSprite(Vector2 loc, Texture sprites, Vector2 collisionBox) : base(loc, sprites)
    {
        vel = new Vector2(0, 0);
        acc = new Vector2(0, 0);
        collided = false;
        timeLeft = 0;
        airTime = 0;
        onGround = false;
        isSpinning = false;
        currPath = null;
        fractionOfPath = 0;
        this.collisionBox = collisionBox;
    }

    public override Bounds2 getPhysicsHitbox()
    {
        return (new Bounds2(loc - collisionBox / 2, collisionBox));
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
        airTime += time;
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

    public void setOnPath(bool onPath)
    {
        this.onPath = onPath;
        Animator.setPiperSpinning(onPath, this);
    }

    public override void updateState(Map map)
    {
        float time = collided ? timeLeft : Engine.TimeDelta;
        collided = false;
        Vector2 locOrig = loc;

        //with this implementation one frame is kind of glitched
        if (onPath)
        {
            fractionOfPath = currPath.getNextFraction(fractionOfPath, velPath * time);

            velPath += accPath * time;


            if (fractionOfPath > 1 || (fractionOfPath == 1 & velPath > 0))
            {
                fractionOfPath = 1;
                setOnPath(false);
            }
            if (fractionOfPath < 0 || (fractionOfPath == 0 & velPath < 0))
            {
                fractionOfPath = 0;
                setOnPath(false);
            }


            loc = currPath.getPoint(fractionOfPath);

            Vector2 currTangent = currPath.getTangent(fractionOfPath);
            float curvature = currPath.getCurvature(fractionOfPath);

            vel = currTangent * velPath;
            acc = currTangent * accPath + (currTangent).Rotated(270) * velPath * velPath * curvature;



            if (velPath * velPath * curvature < Vector2.Dot(Physics.g, currTangent.Rotated(270)))
            {
                setOnPath(false);
            }

        }
        else
        {
            loc = loc + vel * time;
            vel += acc * time;
        }


        //checks if its leaving the ground in some way--maybe this might not work in some edge cases... will have to rethink
        if (onGround && !onPath && map.inAir(loc - map.getNormalVector(locOrig)))
        {
            onGround = false;
            isSpinning = true;
        }
        if (onPath && !onGround)
        {
            onGround = true;
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
        Animator.checkPiperTurn(this);
        
        
        keepOnSurface(map);
        
        
        if (map.closeToSurface(loc))
        {
            this.rotationAngle = (float) (-1 * Math.Asin(Vector2.Cross(map.getNormalVector(loc), Vector2.UP)) * 180 / Math.PI);
        }
        else
        {
            this.rotationAngle = 0;
        }
        
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

    public virtual void collideSpike (float timeLeft)
    {
        collided = true;
        this.timeLeft = timeLeft;

    }

    public void collideSolid(float timeLeft)
    {

        collided = true;
        this.timeLeft = timeLeft;
    }

    public void collidePath(float timeLeft)
    {
        collided = true;
        onPath = true;
        fractionOfPath = currPath.nearestFraction(loc);
        velPath = Vector2.Dot(vel, currPath.getTangent(fractionOfPath));
        Animator.setPiperSpinning(true, this);

    }

    //holy cow clean up the spaghetti code here
    public void keepOnSurface(Map map)
    {
        Vector2 pos = this.loc;
        if (onGround)
        {
            Vector2 newLoc = map.getNearestHoveringPoint(pos);
            if ((newLoc - pos).Length() > 10)
                return;
            this.loc = newLoc;
            Vector2 norm = map.getNormalVector(map.getNearestSurfacePoint(pos));

            vel = vel - norm * Vector2.Dot(vel, norm);

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
