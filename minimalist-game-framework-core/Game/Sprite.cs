using System;
using System.Collections.Generic;
using System.Text;

class Sprite {

    private Vector2 loc;
    private Texture[] textures;
    private Vector2[] hitboxes;
    private int state;
    
    
    public Sprite(Vector2 loc, Texture[] sprites, Vector2[] hitboxes)
    {
        this.loc = loc;
        textures = sprites;
        this.hitboxes = hitboxes;
        state = 0;
    }

    public Sprite(float x, float y, Texture[] sprites, Vector2[] hitboxes)
    {
        loc.X = x;
        loc.Y = y;
        textures = sprites;
        this.hitboxes = hitboxes;
        state = 0;
    }


    public void draw()
    {
        Engine.DrawTexture(textures[state], loc - hitboxes[state] / 2, size: hitboxes[state]);
    }

    public void setState(int index)
    {
        state = index;
    }

    public void changeState()
    {
        state = (state + 1) % textures.Length;
    }
}

