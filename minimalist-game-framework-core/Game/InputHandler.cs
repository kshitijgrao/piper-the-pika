using System;
using System.Collections.Generic;
using System.Text;

internal static class InputHandler
{
    public static Key getPlayerInput(Sonic piper)
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
            
        }
        else if (Engine.GetKeyHeld(Key.D) || Engine.GetKeyHeld(Key.Right))
        {
            k = Key.D;
        }
        
        return k;
    }
}
