using System;
using System.Collections.Generic;
using System.Text;

internal static class InputHandler
{
    public static float getPlayerInput(Sprite piper)
    {
        Key k = Key.Q; // defaults to unused key "Q"
        if (Engine.GetKeyHeld(Key.Space))
        {
            k = Key.Space;
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
            Animator.piperSprinting(true);
        }
        else if (Engine.GetKeyHeld(Key.LeftAlt) && Engine.GetKeyHeld(Key.J))
        {
            Animator.piperSprinting(false);
        }

        return Animator.animatePiper(piper, k);
    }
}
