using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;


//hitbox class to handle general hitboxe
//TODO: this class + subclasses for different shaped hitboxes
class Physics
{
    public static readonly Vector2 g = new Vector2(0,150);
    public static readonly int collisionSteps = 5;

    //detect collisions for things that are within the window
    public static void detectCollisions(Sprite[] sprites)
    {

    }

    //detect collisions with specific object
    public static void detectCollisions(PhysicsSprite obj, Sprite[] colliders)
    {
        foreach (Sprite c in colliders)
        {
            detectCollision(obj, c);
        }
    }

    public static void detectCollision(PhysicsSprite obj1, Sprite obj2)
    {
        Bounds2 b1 = obj1.getHitbox();
        Bounds2 b2 = obj2.getHitbox();
        if (b1.Overlaps(b2))
        {
            obj1.collide(obj2);
            return;
        }

        bool secondPhysics = obj2 is PhysicsSprite;

        //check for tunneling
        Vector2 relvel = obj1.vel - (secondPhysics ? ((PhysicsSprite) obj2).vel : Vector2.Zero);
        b2.Position -= b1.Size / 2;
        b2.Size += b1.Size;

        float minXt = (b2.Min.X - b1.Position.X) / relvel.X;
        float maxXt = (b2.Min.X - b1.Position.X) / relvel.X;

        float tEnter=  Math.Min(maxXt, minXt);
        if (tEnter >= Engine.TimeDelta) return;

        float tExit= Math.Min(Engine.TimeDelta,Math.Max(maxXt, minXt));

        float yEnter = b1.Position.Y + relvel.Y * tEnter;
        float yExit = b2.Position.Y + relvel.Y * tExit;

        if ((yExit > b2.Max.Y && yEnter > b2.Max.Y) || (yExit < b2.Min.Y && yEnter < b2.Min.Y)) 
            return;
        



        //collided
        else
        {

        }

    }

    //detecting ground

    public static void detectGround(PhysicsSprite obj)
    {
        Vector2 pos = obj.getBotPoint();
        Vector2 finalPos = pos + obj.vel * Engine.TimeDelta;

        if(Game.map.inAir(pos) && Game.map.onGround(finalPos))
        {
            Vector2 diff = finalPos - pos;
            while (!Game.map.onGround(pos) && pos.X <= finalPos.X)
                pos += diff / collisionSteps;

            obj.loc += diff - (finalPos - pos);
            obj.keepOnSurface();
            pos = obj.getBotPoint();
            obj.vel = obj.vel - Game.map.getNormalVector(pos) * Vector2.Dot(Game.map.getNormalVector(pos), obj.vel);


            //might have to check for some 0 cases with the dividing here
            obj.collideGround((finalPos - pos).Length() / diff.Length() * Engine.TimeDelta);
        }
    }

    public static Vector2 getPhysicsAcceleration(Vector2 loc, Vector2 vel)
    {
        Vector2 norm = Game.map.getNormalVector(loc);
        float radius = Game.map.getSurfaceRadius(loc);
        if (Game.map.onGround(loc))
        {
            if(radius < 0)
            {
                return g - norm * Vector2.Dot(g, norm);
            }
            return g - norm * Vector2.Dot(g, norm) + vel.Length() * vel.Length() / radius * norm;
        }
        else
        {
            return g;
        }

    }


    //update physics for things that are within the window
    public static void updatePhysics(Sprite[] sprites)
    {
        foreach (Sprite s in sprites)
        {
            s.updateState();
        }
    }


}