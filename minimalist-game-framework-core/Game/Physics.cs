using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;


//hitbox class to handle general hitboxe
//TODO: this class + subclasses for different shaped hitboxes
class Physics
{
    public static readonly Vector2 g = new Vector2(0, 600);
    public static readonly int collisionSteps = 100;
    public static readonly int collisionPixelThresh = 1;
    public static readonly float coeffRestitution = 12f;

    //detect collisions for things that are within the window
    public static void detectCollisions(List<Flower> flowers)
    {

    }

    //detect collisions with specific object
    public static void detectCollisions(PhysicsSprite obj, params Sprite[] colliders)
    {
        foreach (Sprite c in colliders)
        {
            detectCollision(obj, c);
        }
    }

    public static void detectCollision(PhysicsSprite obj1, Sprite obj2)
    {
        
        if (obj2.notCollidable() || obj1.notCollidable())
        {
            return;
        }
        Bounds2 b1 = obj1.getPhysicsHitbox();
        Bounds2 b2 = obj2.getPhysicsHitbox();
        if (b1.Overlaps(b2))
        {
            if (obj2 is Enemy)
                obj2.collide(obj1, Engine.TimeDelta);
            else
                obj2.collide(obj1);
            return;
        }
        bool secondPhysics = obj2 is PhysicsSprite;

        //check for tunneling
        Vector2 relvel = obj1.vel - (secondPhysics ? ((PhysicsSprite)obj2).vel : Vector2.Zero);
        b2.Position -= b1.Size / 2;
        b2.Size += b1.Size;

        float minXt, maxXt, minYt, maxYt;

        if (relvel.X == 0)
        {
            if (b2.Min.X <= b1.Center.X && b2.Max.X >= b1.Center.X)
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
            minXt = (b2.Min.X - b1.Center.X) / relvel.X;
            maxXt = (b2.Max.X - b1.Center.X) / relvel.X;
        }
        if (relvel.Y == 0)
        {
            if (b2.Min.Y <= b1.Center.Y && b2.Max.Y >= b1.Center.Y)
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
            minYt = (b2.Min.Y - b1.Center.Y) / relvel.Y;
            maxYt = (b2.Max.Y - b1.Center.Y) / relvel.Y;
        }

        float tEnter = Math.Max(Math.Min(minXt, maxXt), Math.Min(minYt, maxYt));
        float tExit = Math.Min(Math.Max(minXt, maxXt), Math.Max(minYt, maxYt));

        //no interval of collision
        if (tEnter > tExit)
            return;

        if (tExit >= 0 && tExit <= Engine.TimeDelta)
        {
            if (obj2 is Enemy && ((PhysicsSprite)obj2).mass > 0)
            {
                obj2.collide(obj1, Math.Max(0, tExit));
            }
            else
            {
                obj2.collide(obj1);
            }
        }
    }

    //possibly implement case where onGround = false but loc is onGround? I don't see how that would happen, but it's an edge case
    public static void detectSolid(PhysicsSprite obj, Map map)
    {
        int steps = (int)Math.Ceiling(obj.vel.Length() * Engine.TimeDelta / collisionPixelThresh);
        Vector2 pos = obj.loc;
        Vector2 diff = obj.vel * Engine.TimeDelta / steps;
        Vector2 finalPos = pos + diff;

        for (int i = 0; i < steps; i++)
        {
            if (map.passingSolid(pos, finalPos))
            {
                Console.WriteLine("test");
                Vector2 norm = map.getNormalVector(finalPos);

                
                if(norm.Y > 0)
                {
                    obj.vel = obj.vel - 2 * norm * Vector2.Dot(norm, obj.vel);
                }
                else
                {
                    obj.vel = obj.vel - norm * Vector2.Dot(norm, obj.vel);
                }



                
                obj.loc = map.getNearestHoveringPoint(finalPos);

                if (Math.Round(norm.Y, 2) == 0)
                {
                    obj.collideSolid((steps - i) * Engine.TimeDelta / steps);
                }
                else
                {
                    obj.collideGround((steps - i) * Engine.TimeDelta / steps);
                }


                break;

            }
            if (map.onSpike(finalPos) && map.getNormalVector(finalPos).X == 0)
            {
                if(map.getNormalVector(finalPos).Y < 0)
                {
                    obj.vel.Y = (float)(-1 * Math.Sqrt(2 * g.Y * 100));
                }
                else
                {
                    obj.vel.Y *= -1;
                }
                obj.loc = map.getNearestHoveringPoint(finalPos);

                obj.collideSpike((steps - i) * Engine.TimeDelta / steps);

                break;
            }
            //detecting paths
            if (!obj.onPath && map.onPath(finalPos))
            {
                obj.currPath = map.getPath(finalPos);
                obj.loc = finalPos;
                obj.collidePath((steps - i) * Engine.TimeDelta / steps);

                break;
            }
            pos = finalPos;
            finalPos += diff;
        }
    }




    public static Vector2 getPhysicsAcceleration(PhysicsSprite obj, Vector2 loc, Vector2 vel, Map map)
    {
        if (obj.simpleObject)
        {
            return g;
        }

        Vector2 norm = map.getNormalVector(loc);
        float radius = map.getSurfaceRadius(loc);


        //able to do fake physics on path because all the curvature stuff is taken care of
        if (obj.onPath)
        {
            Vector2 tangent = obj.currPath.getTangent(obj.fractionOfPath);
            return Vector2.RIGHT * Vector2.Dot(tangent, g);
        }
        else if (obj.onGround)
        {
            //return g - norm * Vector2.Dot(g, norm);
            if (radius < 0 || Game.debugToggle)
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
    public static void updatePhysics(Map map, params Sprite[] sprites)
    {
        foreach (Sprite s in sprites)
        {
            s.updateState(map);
        }
    }
}