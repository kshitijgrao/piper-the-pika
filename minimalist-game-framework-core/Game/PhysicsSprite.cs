using System;
using System.Collections.Generic;
using System.Text;

class PhysicsSprite : Sprite
{
    public float mass;
    public Vector2 vel;
    public Vector2 acc;

    private bool collided;
    private float timeLeft;
    internal bool onGround;

    public PhysicsSprite(Vector2 loc, Texture sprites, Vector2 hitboxes) : base(loc,sprites,hitboxes)
    {
        vel = new Vector2(0, 0);
        acc = new Vector2(0, 0);
        collided = false;
        timeLeft = 0;
        onGround = Game.map.onGround(this.getBotPoint());
    }

    public PhysicsSprite(Vector2 loc, Texture sprites) : base(loc, sprites)
    {
        vel = new Vector2(0, 0);
        acc = new Vector2(0, 0);
        collided = false;
        timeLeft = 0;
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

    public void setAccelerationDirect(Vector2 acc)
    {
        this.acc = acc;
    }

    //visualization method
    public void drawVectors()
    {
        Engine.DrawLine(loc, loc + vel, Color.White);
        Engine.DrawLine(loc, loc + acc, Color.Blue);
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
        keepOnSurface();
    }

    public override void collide(Sprite other)
    {

    }

    public void collideGround(float timeLeft)
    {
        onGround = true;
        this.setState(Sprite.landState);
        this.timeLeft = timeLeft;
    }

    public void keepOnSurface()
    {
        Vector2 pos = this.getBotPoint();
        if (Game.map.onGround(pos))
        {
            this.loc.Y += (Game.map.getSurfaceY(pos) - pos.Y);
        }
    }

}
