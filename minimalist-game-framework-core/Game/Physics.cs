﻿using System;
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
    public static void detectCollisions(PhysicsSprite obj, Sprite[] colliders)
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
        Bounds2 b1 = obj1.getHitbox();
        Bounds2 b2 = obj2.getHitbox();
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
    public static void detectSolid(PhysicsSprite obj)
    {
        int steps = (int)Math.Ceiling(obj.vel.Length() * Engine.TimeDelta / collisionPixelThresh);
        Vector2 pos = obj.loc;
        Vector2 diff = obj.vel * Engine.TimeDelta / steps;
        Vector2 finalPos = pos + diff;

        for (int i = 0; i < steps; i++)
        {
            if (Game.map.passingSolid(pos, finalPos))
            {
                Vector2 norm = Game.map.getNormalVector(finalPos);

                obj.vel = obj.vel - norm * Vector2.Dot(norm, obj.vel);
                obj.loc = Game.map.getNearestHoveringPoint(finalPos);

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
            pos = finalPos;
            finalPos += diff;
        }
    }

    //TODO
    public static void detectPath(PhysicsSprite obj)
    {
        if (obj.onPath)
        {
            return;
        }
        int steps = (int)Math.Ceiling(obj.vel.Length() * Engine.TimeDelta / collisionPixelThresh);
        Vector2 pos = obj.loc;
        Vector2 diff = obj.vel * Engine.TimeDelta / steps;
        Vector2 finalPos = pos + diff;

        for (int i = 0; i < steps; i++)
        {
            if (Game.map.onPath(finalPos))
            {
                obj.currPath = Game.map.getPath(finalPos);
                obj.loc = finalPos;
                obj.collidePath((steps - i) * Engine.TimeDelta / steps);
            }
            pos = finalPos;
            finalPos += diff;
        }
    }


    //detecting ground
    public static void detectGround(PhysicsSprite obj)
    {
        Vector2 pos = obj.getBotPoint();
        Vector2 finalPos = pos + obj.vel * Engine.TimeDelta / collisionSteps;

        for (int i = 0; i < collisionSteps; i++)
        {
            if (Game.map.inAir(pos) && (Game.map.onGround(finalPos) || (Game.map.throughThrough(finalPos) && Vector2.Dot(obj.vel, Game.map.getNormalVector(finalPos)) < 0 && Game.map.closeToSurface(finalPos))))
            {
                Vector2 diff = finalPos - pos;


                obj.loc += diff - (finalPos - pos);
                int? surfaceY = Game.map.getSurfaceY(pos);
                if (!surfaceY.HasValue || Math.Abs(surfaceY.Value - pos.Y) > 10)
                {
                    return;
                }
                obj.loc.Y += (surfaceY.Value - pos.Y);

                Vector2 posNew = obj.getBotPoint();

                obj.vel = obj.vel - Game.map.getNormalVector(posNew) * Vector2.Dot(Game.map.getNormalVector(posNew), obj.vel);

                //might have to check for some 0 cases with the dividing here
                obj.collideGround((finalPos - pos).Length() / diff.Length() * Engine.TimeDelta);
                break;

            }
            pos = finalPos;
            finalPos += obj.vel * Engine.TimeDelta / collisionSteps;

        }



        /* if(Game.map.inAir(pos) && (Game.map.onGround(finalPos) || (Game.map.throughThrough(finalPos) && Vector2.Dot(obj.vel,Game.map.getNormalVector(finalPos)) < 0 && Game.map.closeToSurface(finalPos))))
         {
             System.Diagnostics.Debug.WriteLine("bruh: " + pos.ToString() + " to:" + finalPos.ToString());

             while (!(Game.map.onGround(pos) || Game.map.throughThrough(finalPos)) && (pos.X <= finalPos.X && pos.Y <= finalPos.Y))
                 pos += diff / collisionSteps;


         }*/


    }

    //detecting unpenetrable thingies
    public static void detectUnpenetrable(PhysicsSprite obj)
    {
        Vector2 direc = Game.map.getNormalVector(obj.getBotPoint()).Rotated(90);

        Vector2 pos = obj.getPoint(direc);

        if (Game.map.onSolid(pos))
        {
            obj.vel = obj.vel - (new Vector2(-1, 0)) * Math.Min(0, Vector2.Dot(new Vector2(-1, 0), obj.vel));
            obj.acc = obj.acc - (new Vector2(-1, 0)) * Math.Min(0, Vector2.Dot(new Vector2(-1, 0), obj.acc));
        }

        Vector2 finalPos = pos + obj.vel * Engine.TimeDelta;

        if (Game.map.inAir(pos) && Game.map.onSolid(finalPos))
        {
            Vector2 diff = finalPos - pos;
            while (!(Game.map.onSolid(pos)) && (pos.X <= finalPos.X && pos.Y <= finalPos.Y))
                pos += diff / collisionSteps;
            obj.loc += diff - (finalPos - pos);

            obj.vel = obj.vel - (new Vector2(-1, 0)) * Vector2.Dot(new Vector2(-1, 0), obj.vel);

            obj.collideSolid((finalPos - pos).Length() / diff.Length() * Engine.TimeDelta);

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
        if (Game.map.onSolid(pos))
        {
            obj.vel = obj.vel - (new Vector2(-1, 0)) * Math.Min(0, Vector2.Dot(new Vector2(-1, 0), obj.vel));
            obj.acc = obj.acc - (new Vector2(-1, 0)) * Math.Min(0, Vector2.Dot(new Vector2(-1, 0), obj.acc));
        }

        Vector2 finalPos = pos + obj.vel * Engine.TimeDelta;

        if (Game.map.inAir(pos) && Game.map.onSolid(finalPos))
        {
            Vector2 diff = finalPos - pos;
            while (!(Game.map.onSolid(pos)) && (pos.X <= finalPos.X && pos.Y <= finalPos.Y))
                pos += diff / collisionSteps;
            obj.loc += diff - (finalPos - pos);

            obj.vel = obj.vel - (new Vector2(-1, 0)) * Vector2.Dot(new Vector2(-1, 0), obj.vel);

            obj.collideSolid((finalPos - pos).Length() / diff.Length() * Engine.TimeDelta);

        }


    }

    public static Vector2 getPhysicsAcceleration(PhysicsSprite obj, Vector2 loc, Vector2 vel)
    {
        Vector2 norm = Game.map.getNormalVector(loc);
        float radius = Game.map.getSurfaceRadius(loc);


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
    public static void updatePhysics(Sprite[] sprites)
    {
        foreach (Sprite s in sprites)
        {
            s.updateState();
        }
    }
}