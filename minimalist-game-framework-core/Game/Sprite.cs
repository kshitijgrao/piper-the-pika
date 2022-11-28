﻿using System;
using System.Collections.Generic;
using System.Text;

class Sprite {

    public Vector2 loc;
    private Texture textures;
    private protected Vector2[] hitboxes;
    private protected int state;
    private float[] hitboxCoord;
    
    
    public Sprite(Vector2 loc, Texture sprites, Vector2[] hitboxes)
    {
        this.loc = loc;
        textures = sprites;
        this.hitboxes = hitboxes;
        state = 0;
        hitboxCoord = new float[hitboxes.Length];
        hitboxCoord[0] = 0;
        for(int i = 0; i < hitboxes.Length - 1; i++)
        {
            hitboxCoord[i + 1] = hitboxCoord[i] + hitboxes[i].X;
        }
    }


    public Sprite(float x, float y, Texture sprites, Vector2[] hitboxes) : this(new Vector2(x, y), sprites, hitboxes)
    {
        
    }

    public Sprite(Vector2 loc, String location, Vector2[] hitboxes) : this(loc, Engine.LoadTexture(location), hitboxes)
    {

    }


    public virtual void collide(Sprite other)
    {

    }



    private Bounds2 getTextureSource()
    {
        return new Bounds2(hitboxCoord[state], 0, hitboxes[state].X, hitboxes[state].Y);
    }

    
    public void draw()
    {
        Engine.DrawTexture(textures, loc - hitboxes[state] / 2, source: getTextureSource());
    }

    public void setState(int index)
    {
        state = index;
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
