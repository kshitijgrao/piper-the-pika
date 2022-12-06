using System;
using System.Collections.Generic;
using System.Text;

class Sprite {

    public Vector2 loc;
    private Texture spritemap;
    private protected Vector2 hitbox;
    private protected int state;
    private Boolean spriteFaceLeft;

    public static readonly int landState = 6;

    public Sprite(Vector2 loc, Texture spritemap)
    {
        this.loc = loc;
        this.spritemap = spritemap;
        hitbox = new Vector2(24, 24);
        spriteFaceLeft = false;
        state = 0;

    }

    public Sprite(Vector2 loc, Texture spritemap, Vector2 hitbox)
    {
        this.loc = loc;
        this.spritemap = spritemap;
        this.hitbox = hitbox;
        spriteFaceLeft = false;
        state = 0;
    }

    public Sprite(float x, float y, Texture sprites, Vector2 hitbox) : this(new Vector2(x, y), sprites, hitbox)
    {
        
    }

    public virtual void collide(Sprite other)
    {

    }

    public void move(Vector2 v)
    {
        loc += v;
    }

    public void turn()
    {
        spriteFaceLeft = !spriteFaceLeft;
    }

    public Boolean isLeft()
    {
        return spriteFaceLeft;
    }
    
    public void testDraw()
    {
        Engine.DrawTexture(spritemap, loc - hitbox / 2, source: new Bounds2(0,0,24,24));
    }

    public void draw(Bounds2 bounds)
    {
        TextureMirror mirror = spriteFaceLeft ? TextureMirror.Horizontal : TextureMirror.None;
        Engine.DrawTexture(spritemap, loc, source: bounds, mirror: mirror);
    }

    public void setState(int state)
    {
        this.state = state;
    }

    public int getState()
    {
        return state;
    }

    public virtual void updateState()
    {
        state = (state + 1) % 5;
    }

    public Bounds2 getHitbox()
    {
        return new Bounds2(loc - hitbox / 2, hitbox);
    }


    public Vector2 getBotPoint()
    {
        return loc + hitbox / 2;
    }

}

