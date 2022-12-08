using System;
using System.Collections.Generic;
using System.Text;

internal static class InputHandler
{
    public static Key getPlayerInput(Sonic piper, Vector2 position)
    {
        Key k = Key.Q; // defaults to unused key "Q"
        if (Engine.GetKeyHeld(Key.Space))
        {
            k = Key.Space;
            piper.jump();
        }
        else if (Engine.GetKeyHeld(Key.A))
        {
            k = Key.A;
        }
        else if (Engine.GetKeyHeld(Key.D))
        {
            k = Key.D;
        }

        // TESTING
        else if (Engine.GetKeyHeld(Key.LeftAlt) && Engine.GetKeyHeld(Key.F))
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

        piper.setFrameIndex(Animator.animatePiper(piper, position, k));
        return k;
    }
}
