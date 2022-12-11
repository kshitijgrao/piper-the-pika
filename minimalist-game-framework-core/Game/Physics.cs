using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;


//hitbox class to handle general hitboxe
//TODO: this class + subclasses for different shaped hitboxes
class Physics
{
    public static readonly Vector2 g = new Vector2(0,40);
    public static readonly int collisionSteps = 5;

    //detect collisions for things that are within the window
    public static void detectCollisions(List<Flower> flowers)
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
        
        if (obj2.notCollidable())
        {
            return;
        }
        Bounds2 b1 = obj1.getHitbox();
        Bounds2 b2 = obj2.getHitbox();
        if (b1.Overlaps(b2))
        {
            obj2.collide(obj1);
            return;
        }
        bool secondPhysics = obj2 is PhysicsSprite;

        //check for tunneling
        Vector2 relvel = obj1.vel - (secondPhysics ? ((PhysicsSprite) obj2).vel : Vector2.Zero);
        b2.Position -= b1.Size / 2;
        b2.Size += b1.Size;

        float minXt, maxXt, minYt, maxYt;

        if(relvel.X == 0)
        {
            if (b2.Min.X <= b1.Position.X && b2.Max.X >= b1.Position.X)
            {
                minXt = 0;
                maxXt = Engine.TimeDelta;
            }
            //no possiblity to collide
            else
            {
                return;
            }
        }
        else
        {
            minXt = (b2.Min.X - b1.Position.X) / relvel.X;
            maxXt = (b2.Max.X - b1.Position.X) / relvel.X;
        }
        if (relvel.Y == 0)
        {
            if (b2.Min.Y <= b1.Position.Y && b2.Max.Y >= b1.Position.Y)
            {
                minYt = 0;
                maxYt = Engine.TimeDelta;
            }
            //no possiblity to collide
            else
            {
                return;
            }
        }
        else
        {
            minYt = (b2.Min.Y - b1.Position.Y) / relvel.Y;
            maxYt = (b2.Max.Y - b1.Position.Y) / relvel.Y;
        }




        float tEnter = Math.Max(Math.Min(minXt, maxXt), Math.Min(minYt, maxYt));
        float tExit = Math.Min(Math.Max(minXt, maxXt), Math.Max(minYt, maxYt));

        //no interval of collision
        if (tEnter > tExit)
            return;

        if(tExit >= 0 && tExit <= Engine.TimeDelta)
        {
            obj2.collide(obj1);
        }
    }

    //detecting ground

    public static void detectGround(PhysicsSprite obj)
    {
        Vector2 pos = obj.getBotPoint();
        Vector2 finalPos = pos + obj.vel * Engine.TimeDelta;

        if(Game.map.inAir(pos) && (Game.map.onGround(finalPos) || (Game.map.throughThrough(finalPos) && Vector2.Dot(obj.vel,Game.map.getNormalVector(finalPos)) < 0 && Game.map.closeToSurface(finalPos))))
        {
            Vector2 diff = finalPos - pos;
            while (!(Game.map.onGround(pos) || Game.map.throughThrough(finalPos)) && (pos.X <= finalPos.X && pos.Y <= finalPos.Y))
                pos += diff / collisionSteps;

            obj.loc += diff - (finalPos - pos);
            obj.loc.Y += (Game.map.getSurfaceY(pos) - pos.Y);

            Vector2 posNew = obj.getBotPoint();
            
            obj.vel = obj.vel - Game.map.getNormalVector(posNew) * Vector2.Dot(Game.map.getNormalVector(posNew), obj.vel);

            //might have to check for some 0 cases with the dividing here
            obj.collideGround((finalPos - pos).Length() / diff.Length() * Engine.TimeDelta);
        }
    }

    //detecting unpenetrable thingies
    public static void detectUnpenetrable(PhysicsSprite obj)
    {
        Vector2 direc = Game.map.getNormalVector(obj.getBotPoint()).Rotated(90);

        Vector2 pos = obj.getPoint(direc);

        if (Game.map.impenetrable(pos))
        {
            obj.vel = obj.vel - (new Vector2(-1, 0)) * Math.Min(0,Vector2.Dot(new Vector2(-1, 0), obj.vel));
            obj.acc = obj.acc - (new Vector2(-1, 0)) * Math.Min(0,Vector2.Dot(new Vector2(-1, 0), obj.acc));
        }
        
        Vector2 finalPos = pos + obj.vel * Engine.TimeDelta;

        if (Game.map.inAir(pos) && Game.map.impenetrable(finalPos))
        {
            Vector2 diff = finalPos - pos;
            while (!(Game.map.impenetrable(pos)) && (pos.X <= finalPos.X && pos.Y <= finalPos.Y))
                pos += diff / collisionSteps;
            obj.loc += diff - (finalPos - pos);

            obj.vel = obj.vel - (new Vector2 (-1,0)) * Vector2.Dot(new Vector2(-1, 0), obj.vel);

            obj.collideWall((finalPos - pos).Length() / diff.Length() * Engine.TimeDelta);

        }

    }

    //detecting unpenetrable thingies from a certain direction
    //TODO: should be able to reduced redundancy by 
    public static void detectUnpenetrable(PhysicsSprite obj, Vector2 direc)
    {
        Vector2 pos = obj.getPoint(direc);

        //not allowing futher movement within
        //NOTE:This is SLIGHTLY broken and gets stuck within wall until we implement full bottom layer collision
        //detection or switch to an SVG encoding of the map
        if (Game.map.impenetrable(pos))
        {
            obj.vel = obj.vel - (new Vector2(-1, 0)) * Math.Min(0, Vector2.Dot(new Vector2(-1, 0), obj.vel));
            obj.acc = obj.acc - (new Vector2(-1, 0)) * Math.Min(0, Vector2.Dot(new Vector2(-1, 0), obj.acc));
        }

        Vector2 finalPos = pos + obj.vel * Engine.TimeDelta;

        if (Game.map.inAir(pos) && Game.map.impenetrable(finalPos))
        {
            Vector2 diff = finalPos - pos;
            while (!(Game.map.impenetrable(pos)) && (pos.X <= finalPos.X && pos.Y <= finalPos.Y))
                pos += diff / collisionSteps;
            obj.loc += diff - (finalPos - pos);

            obj.vel = obj.vel - (new Vector2(-1, 0)) * Vector2.Dot(new Vector2(-1, 0), obj.vel);

            obj.collideWall((finalPos - pos).Length() / diff.Length() * Engine.TimeDelta);

        }


    }

    public static Vector2 getPhysicsAcceleration(Vector2 loc, Vector2 vel)
    {
        Vector2 norm = Game.map.getNormalVector(loc);
        float radius = Game.map.getSurfaceRadius(loc);

        if (Game.piper.onGround)
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