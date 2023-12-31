﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

class Sprite {

    public Vector2 loc;
    private Texture currentmap;
    private Texture spritemap;
    private Texture blinkmap;
    private float frameIndex;
    private protected State state;
    private Boolean spriteFaceLeft;
    private Boolean AnimationLocked; // a sprite is locked if stuck finishing an animation
    private protected Vector2 hitbox;

    public bool invisible;
    public bool isSparkling;
    public bool notCollable;

    internal float rotationAngle;

    readonly Texture sparkles = Engine.LoadTexture("sparkles-effect.png");
    public static readonly State landState = State.Landing;

    public Sprite(Vector2 loc, Texture spritemap)
    {
        this.loc = loc;
        this.spritemap = spritemap;
        currentmap = spritemap;
        blinkmap = spritemap;
        frameIndex = 0;
        hitbox = new Vector2(24, 24);
        spriteFaceLeft = false;
        AnimationLocked = false;
        state = 0;
        invisible = false;
    }

    public Sprite(Vector2 loc, Texture spritemap, Texture blinkmap)
    {
        this.loc = loc;
        this.spritemap = spritemap;
        this.blinkmap = blinkmap;
        currentmap = spritemap;
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
        currentmap = spritemap;
        blinkmap = spritemap;
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

    public void setInvisible()
    {
        invisible = true;
    }

    public Vector2 getHitboxNoCalc()
    {
        return hitbox;
    }

    public virtual void collide(Sprite other)
    {
        if (this is Enemy)
        {
            Animator.animateEnemyTakingDamage((Enemy)this);
        }
        else if (this is Flower)
        {
            this.setState(State.Walk); // flower is not actually walking, uhh its walking out of the screen :) **its an exit animation
        }
        else
        {
            invisible = true;
        }
    }

    public virtual void collide(PhysicsSprite other, float timeLeft)
    {

    }

    public void move(Vector2 v)
    {
        loc += v;
    }

    public void blink(Boolean willBlink)
    {
        if (willBlink)
        {
            currentmap = blinkmap;
        }
        else
        {
            currentmap = spritemap;
        }
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
            


            Engine.DrawTexture(spritemap, position - hitbox / 2, source: bounds, rotation: rotationAngle, mirror: mirror, scaleMode: TextureScaleMode.Nearest);
            if (Game.debugToggle)
            {
                Engine.DrawRectEmpty(new Bounds2(position - getPhysicsHitbox().Size / 2, getPhysicsHitbox().Size), Color.Yellow);
            }
        }
        if (isSparkling)
        {
            Bounds2 sparkleBounds = new Bounds2(new Vector2(bounds.Min.X, 0), new Vector2(24, 24));
            Engine.DrawTexture(sparkles, position - hitbox / 2, source: sparkleBounds);
        }
    }

    public void setState(State state)
    {
        this.state = state;
        if (state == State.StartingJump)
        {
            Animator.changeFramerate(10);
        }
        else if (state == State.Landing)
        {
            Animator.changeFramerate(5);
        }
    }

    public State getState()
    {
        return state;
    }

    public virtual void updateState(Map map)
    {
        state = (State)(((int)state + 1) % 5);
    }

    public float getFrameIndex()
    {
        return frameIndex;
    }

    public void setFrameIndex(float frameIndex)
    {
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

    public virtual Bounds2 getPhysicsHitbox()
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

    public virtual bool notCollidable()
    {
        return invisible || notCollable;
    }

}

enum State
{
    // states --> idle 0, walk 1, sprint 2, starting jump 3, spining 4, landing 5, damage 6
    Idle,
    Walk,
    Sprint,
    StartingJump,
    Spinning,
    Landing,
    Damage,
    Dead
}

