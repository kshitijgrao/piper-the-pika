using System;
using System.Collections.Generic;
using System.Text;

class PhysicsSprite : Sprite
{
    public float mass;
    public Vector2 vel;
    public Vector2 acc;

    public PhysicsSprite(Vector2 loc, Texture sprites, Vector2[] hitboxes) : base(loc,sprites,hitboxes)
    {
        vel = new Vector2(0, 0);
        acc = new Vector2(0, 0);
    }

    public void setVelocity(Vector2 vel)
    {
        vel.Y *= 1;
        this.vel = vel;
    }

    public void setAcceleration(Vector2 acc)
    {
        acc.Y *= 1;
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
        loc = loc + vel * Engine.TimeDelta;
        vel += acc * Engine.TimeDelta;
    }

    public override void collide(Sprite other)
    {

    }

}
