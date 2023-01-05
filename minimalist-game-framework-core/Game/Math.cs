using System;
using System.Collections.Generic;
using System.Text;

class Polynomial
{
    public static readonly Polynomial Zero = new Polynomial(-1);

    float[] coeffs;
    int deg;
    public Polynomial(float[] coeffs)
    {
        this.coeffs = coeffs;
        this.deg = coeffs.Length - 1;
    }
    
    public Polynomial(int deg)
    {
        this.deg = deg;
        coeffs =  new float[deg + 1];
    }

    public float evaluateAt(float x)
    {
        float output = 0;
        for (int i = 0; i < coeffs.Length; i++)
        {
            output += (float) Math.Pow(x, i) * coeffs[i];
        }
        return output;
    }

    public Polynomial differentiate()
    {
        if(coeffs.Length == 0)
        {
            return new Polynomial(0);
        }
        float[] newCoeffs = new float[coeffs.Length - 1];
        for(int i = 1; i < coeffs.Length; i++)
        {
            newCoeffs[i - 1] = coeffs[i] * i;
        }
        return new Polynomial(newCoeffs);
    }

    public static Polynomial operator +(Polynomial p1, Polynomial p2)
    {
        if(p1.deg < 0 && p2.deg < 0)
        {
            return Polynomial.Zero;
        }
        float[] newCoeffs = new float[Math.Max(p1.deg + 1, p2.deg + 1)];
        for(int i = 0; i < newCoeffs.Length; i++)
        {
            if (i < p1.deg + 1)
            {
                newCoeffs[i] += p1.coeffs[i];
            }
            if (i < p2.deg + 1)
            {
                newCoeffs[i] += p2.coeffs[i];
            }

        }
        return new Polynomial(newCoeffs);
    }

    public static Polynomial operator *(float s, Polynomial p2)
    {
        float[] newCoeffs = new float[p2.deg + 1];
        for(int i = 0; i < p2.deg + 1; i++)
        {
            newCoeffs[i] = p2.coeffs[i] * s;
        }
        return new Polynomial(newCoeffs);
    }
    
    public static Polynomial operator *(Polynomial p, float s)
    {
        return s * p;
    }
}