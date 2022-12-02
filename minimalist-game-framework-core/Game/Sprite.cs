using System;
using System.Collections.Generic;
using System.Text;

class Sprite {

    public Vector2 loc;
    private Texture spritemap;
    private protected Vector2[] hitboxes;
    private protected int state;
    private Boolean spriteFaceLeft;
    private float[] hitboxCoord;
    
    
    public Sprite(Vector2 loc, Texture spritemap)
    {
        this.loc = loc;
        this.spritemap = spritemap;
        hitboxes = new Vector2[1];
        spriteFaceLeft = false;
        state = 0;
        hitboxCoord = new float[hitboxes.Length];
        hitboxCoord[0] = 0;
        for(int i = 0; i < hitboxes.Length - 1; i++)
        {
            hitboxCoord[i + 1] = hitboxCoord[i] + hitboxes[i].X;
        }
    }

    public Sprite(Vector2 loc, Texture spritemap, Vector2[] hitboxes)
    {
        this.loc = loc;
        this.spritemap = spritemap;
        this.hitboxes = hitboxes;
        spriteFaceLeft = false;
        state = 0;
        hitboxCoord = new float[hitboxes.Length];
        hitboxCoord[0] = 0;
        for (int i = 0; i < hitboxes.Length - 1; i++)
        {
            hitboxCoord[i + 1] = hitboxCoord[i] + hitboxes[i].X;
        }
    }

    public Sprite(float x, float y, Texture sprites, Vector2[] hitboxes) : this(new Vector2(x, y), sprites, hitboxes)
    {
        
    }

    public virtual void collide(Sprite other)
    {

    }

    private Bounds2 getTextureSource()
    {
        return new Bounds2(hitboxCoord[state], 0, hitboxes[state].X, hitboxes[state].Y);
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
    
    public void draw()
    {
        Engine.DrawTexture(spritemap, loc - hitboxes[state] / 2, source: getTextureSource());
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
        state = (state + 1) % hitboxes.Length;
    }

    public Bounds2 getHitbox()
    {
        return new Bounds2(loc - hitboxes[state] / 2, hitboxes[state]);
    }

}

