using System;
using System.Collections.Generic;
using System.Text;


//hitbox class to handle general hitboxe
//TODO: this class + subclasses for different shaped hitboxes
class Physics
{
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
        Vector2 relvel = obj1.vel - (obj2 is PhysicsSprite ? ((PhysicsSprite) obj2).vel : Vector2.Zero);
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


    public static void detectGround(PhysicsSprite obj)
    {
        Vector2 pos = obj.getBotPoint();

        if(Game.map.getPixelType(pos) == Map.GROUND_CODE)
        {


        }



    }

    public static void keepOnSurface(PhysicsSprite obj)
    {

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