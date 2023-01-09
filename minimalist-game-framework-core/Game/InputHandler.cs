using System;
using System.Collections.Generic;
using System.Text;

internal static class InputHandler
{
    public static Key getPlayerInput(Sonic piper, Vector2 position)
    {
        Key k = Key.Q; // defaults to unused key "Q"
        if (Engine.GetKeyDown(Key.Space) || Engine.GetKeyDown(Key.Up) || Engine.GetKeyDown(Key.W)) 
        {
            k = Key.Space;
            piper.jump();
        }
        else if (Engine.GetKeyHeld(Key.A) || Engine.GetKeyHeld(Key.Left))
        {
            k = Key.A;
            Animator.checkPiperTurn(piper);
        }
        else if (Engine.GetKeyHeld(Key.D) || Engine.GetKeyHeld(Key.Right))
        {
            k = Key.D;
            Animator.checkPiperTurn(piper);
        }

        // TESTING
        else if (Engine.GetKeyHeld(Key.LeftAlt) && Engine.GetKeyHeld(Key.L))
        {
            Animator.animatePiperLanding(piper);
        }
        else if (Engine.GetKeyHeld(Key.LeftAlt) && Engine.GetKeyHeld(Key.G))
        {
            piper.takeDamage();
        }
        else if (Engine.GetKeyHeld(Key.LeftAlt) && Engine.GetKeyHeld(Key.H))
        {
            Animator.setPiperSprinting(true);
        }
        else if (Engine.GetKeyHeld(Key.LeftAlt) && Engine.GetKeyHeld(Key.J))
        {
            Animator.setPiperSprinting(false);
        }
        else if (Engine.GetKeyHeld(Key.LeftAlt) && Engine.GetKeyHeld(Key.K))
        {
            Animator.setPiperSpinning(true, piper);
        }
        else if (Engine.GetKeyHeld(Key.LeftAlt) && Engine.GetKeyHeld(Key.L))
        {
            Animator.setPiperSpinning(false, piper);
        }

        return k;
    }
}
