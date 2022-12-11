using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

class Sprite {

    public Vector2 loc;
    private Texture spritemap;
    private float frameIndex;
    private protected int state;
    private Boolean spriteFaceLeft;
    private Boolean AnimationLocked; // a sprite is locked if stuck finishing an animation
    private protected Vector2 hitbox;

    private bool invisible;

    public static readonly int landState = 6;

    public Sprite(Vector2 loc, Texture spritemap)
    {
        this.loc = loc;
        this.spritemap = spritemap;
        frameIndex = 0;
        hitbox = new Vector2(24, 24);
        spriteFaceLeft = false;
        AnimationLocked = false;
        state = 0;
        invisible = false;
    }

    public Sprite(Vector2 loc, Texture spritemap, Vector2 hitbox)
    {
        this.loc = loc;
        this.spritemap = spritemap;
        this.hitbox = hitbox;
        spriteFaceLeft = false;
        state = 0;
        invisible = false;
    }

    public Sprite(float x, float y, Texture sprites, Vector2 hitbox) : this(new Vector2(x, y), sprites, hitbox)
    {
        
    }

    public Boolean isInvisible()
    {
        return invisible;
    }

    public Vector2 getHitboxNoCalc()
    {
        return hitbox;
    }

    public virtual void collide(Sprite other)
    {
        invisible = true;
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

    public void draw(Bounds2 bounds, Vector2 position)
    {
        if (!invisible) {
            TextureMirror mirror = spriteFaceLeft ? TextureMirror.Horizontal : TextureMirror.None;
            Engine.DrawTexture(spritemap, position, source: bounds, mirror: mirror);
        }
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

    public float getFrameIndex()
    {
        return frameIndex;
    }

    public void setFrameIndex(float frameIndex)
    {
        if (frameIndex == 0)
        {
            Debug.WriteLine("Resetting frame index");
        }
        this.frameIndex = frameIndex;
    }

    public Boolean animationIsLocked()
    {
        return AnimationLocked;
    }

    public void takeDamage()
    {
        Animator.animatePiperTakingDamage(this);
    }

    public void changeLocked(Boolean isLocked)
    {
        AnimationLocked = isLocked;
    }

    public Bounds2 getHitbox()
    {
        return new Bounds2(loc - hitbox / 2, hitbox);
    }


    public Vector2 getBotPoint() { return getPoint(new Vector2(0, 1)); }
    public Vector2 getRightPoint() { return getPoint(new Vector2(1, 0)); }
    public Vector2 getLeftPoint() { return getPoint(new Vector2(-1, 0)); }

    public Vector2 getPoint(Vector2 direc)
    {
        return loc + hitbox.X / 2 * direc;
    }

    public bool notCollidable()
    {
        return invisible;
    }

}

