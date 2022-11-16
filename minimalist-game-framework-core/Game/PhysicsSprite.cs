using System;
using System.Collections.Generic;
using System.Text;

class PhysicsSprite : Sprite
{

    private Vector2 vel;
    private Vector2 acc;

    public PhysicsSprite(Vector2 loc, Texture sprites, Vector2[] hitboxes) : base(loc,sprites,hitboxes)
    {
        vel = new Vector2(0, 0);
        acc = new Vector2(0, 0);
    }

    public void setVelocity(Vector2 vel)
    {
        vel.Y *= -1;
        this.vel = vel;
    }

    public void setAcceleration(Vector2 acc)
    {
        acc.Y *= -1;
        this.acc = acc;
    }

    public void updateState()
    {
        loc = loc + vel * Engine.TimeDelta;
        vel += acc * Engine.TimeDelta;
    }

}
