using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

/*
class BezierCurve
{
    private Vector2 start;
    private Vector2 startHandle;
    private Vector2 end;
    private Vector2 endHandle;

    public BezierCurve(Vector2 start, Vector2 startHandle, Vector2 end, Vector2 endHandle)
    {
        this.start = start;
        this.startHandle = startHandle;
        this.end = end;
        this.endHandle = endHandle;
    }


}*/

static class SVGReader
{
    public static readonly Dictionary<string, string> ColorTypes = new Dictionary<string, string>
    {
        {"path" , "#EBFF00" },
        {"ground" , "#710000" },
        {"through" , "#FF0000" }
    };
    public static void findElementsAndAdd(Map map, string path)
    {
        string[] lines = File.ReadAllLines(path);
        foreach (string line in lines)
        {
            if (isCurve(line))
            {
                addCurve(line, map);
            }
            else if (isPath(line))
            {
                System.Diagnostics.Debug.WriteLine("asdf");
                addPath(line, map);
            }

        }
    }
    private static bool isCurve(string line)
    {
        if(line.Length <= 16)
        {
            return false;
        }
        return line.Substring(0, 5) == "<rect" && (line.Substring(line.Length - 10, 7) == ColorTypes["ground"] || line.Substring(line.Length - 10, 7) == ColorTypes["through"]);
    }

    private static void addCurve(string line, Map map)
    {
        if (line.Contains("y"))
        {
            string[] rectVals = line.Substring(9, line.Length - 18 - 9).Replace("=", "").Replace("width", "").Replace("height", "").Replace("y", "").Replace(" ", "").Replace("\"\"", "\"").Split('\"');
            map.addCurve(new Rect(rectVals, line.Substring(line.Length - 9, 6)));
        }
        else if (line.Contains("transform"))
        {
            string[] rectVals = line.Substring(13, line.Length - 19 - 13).Replace("transform=\"matrix(", "").Replace("height=", "").Replace("\"", "").Split(' ');
            map.addCurve(new Rect(rectVals, line.Substring(line.Length - 9, 6)));
        }
    }

    private static bool isPath(string line)
    {
        if(line.Length < 7)
        {
            return false;
        }
        bool hello = line.Contains(ColorTypes["path"]);
        return line.Substring(0, 5) == "<path" && line.Contains(ColorTypes["path"]);
    }

    private static void addPath(string line, Map map)
    {
        List<float> coords = new List<float>();
        foreach (string coord in line.Substring(10, line.IndexOf("stroke") - 12).Replace('C', ' ').Split(' '))
        {
            coords.Add(float.Parse(coord));
        }
        map.addPath(new BezierGroup(coords.ToArray()));
    }
}


interface Curve
{
    Vector2 getNearestNormal(Vector2 pos); //for getting normal vector generally
    float getNearestCurvature(Vector2 pos); //for getting curvature
    bool contains(Vector2 pos);
}

interface Path2
{
    Vector2 getPoint(float t);
    Vector2 getTangent(float t);
    float getCurvature(float t);
    float getSpeed(float t);
    float getNextFraction(float t, float arcLength);
    float getBoost(float t, Key key);

    bool contains(Vector2 loc);
    float nearestFraction(Vector2 loc);
}


class BezierGroup : Path2
{
    private BezierCurveNoStroke[] bezierCurves;

    public BezierGroup(params Vector2[] coordList)
    {
        bezierCurves = new BezierCurveNoStroke[(coordList.Length - 1) / 3];
        for (int i = 0; i < bezierCurves.Length; i++)
        {
            bezierCurves[i] = new BezierCurveNoStroke(coordList[3 * i], coordList[3 * i + 1], coordList[3 * i + 2], coordList[3 * i + 3]);
        }
    }

    public BezierGroup(params float[] coordList)
    {
        bezierCurves = new BezierCurveNoStroke[(coordList.Length - 2) / 6];
        for (int i = 0; i < bezierCurves.Length; i++)
        {
            bezierCurves[i] = new BezierCurveNoStroke(new Vector2(coordList[6 * i], coordList[6 * i + 1]),
                new Vector2(coordList[6 * i + 2], coordList[6 * i + 3]),
                new Vector2(coordList[6 * i + 4], coordList[6 * i + 5]),
                new Vector2(coordList[6 * i + 6], coordList[6 * i + 7]));
        }
    }

    public Vector2 getPoint(float t) { return bezierCurves[getIndex(t)].getPoint(convertTime(t)); }
    public Vector2 getTangent(float t) { return bezierCurves[getIndex(t)].getTangent(convertTime(t)); }
    public float getCurvature(float t) { return bezierCurves[getIndex(t)].getCurvature(convertTime(t)); }
    public float getSpeed(float t) { return bezierCurves[getIndex(t)].getSpeed(convertTime(t)) * bezierCurves.Length; }
    public float getNextFraction(float t, float arcLength) { return bezierCurves[getIndex(t)].getNextFraction(convertTime(t), arcLength) / bezierCurves.Length + ((float)getIndex(t)) / bezierCurves.Length; }
    public float getBoost(float t, Key key) { return bezierCurves[getIndex(t)].getBoost(convertTime(t), key); }
    public bool contains(Vector2 loc) { return bezierCurves[0].contains(loc) || bezierCurves[bezierCurves.Length - 1].contains(loc); }
    private float convertTime(float t)
    {
        return (t - ((float)getIndex(t)) / bezierCurves.Length) * bezierCurves.Length;
    }

    public float nearestFraction(Vector2 loc)
    {
        if (bezierCurves[0].contains(loc))
        {
            return bezierCurves[0].nearestFraction(loc);
        }
        else
        {
            return bezierCurves[bezierCurves.Length - 1].nearestFraction(loc);
        }
    }

    private int getIndex(float t)
    {
        if (t == 1)
        {
            return bezierCurves.Length - 1;
        }
        return (int)Math.Floor(t * bezierCurves.Length);
    }

}

class BezierCurveNoStroke : Path2
{
    private Vector2 start;
    private Vector2 startHandle;
    private Vector2 end;
    private Vector2 endHandle;

    public BezierCurveNoStroke(Vector2 start, Vector2 startHandle, Vector2 endHandle, Vector2 end)
    {
        this.start = start;
        this.startHandle = startHandle;
        this.end = end;
        this.endHandle = endHandle;
    }

    //gets location at linearly interpolated time t
    public Vector2 getPoint(float t)
    {
        return (float)Math.Pow(1 - t, 3) * start + 3 * (float)Math.Pow(1 - t, 2) * t * startHandle + 3 * (1 - t) * (float)Math.Pow(t, 2) * endHandle + (float)Math.Pow(t, 3) * end;
    }

    //gets velocity at linearly interpolated time t
    private Vector2 getVelocity(float t)
    {
        return 3 * (float)Math.Pow(1 - t, 2) * (startHandle - start) + 6 * (1 - t) * t * (endHandle - startHandle) + 3 * (float)Math.Pow(t, 2) * (end - endHandle);
    }
    public float getSpeed(float t)
    {
        return getVelocity(t).Length();
    }

    public Vector2 getTangent(float t)
    {
        return getVelocity(t).Normalized();
    }


    //gets acceleration at linearly interpolated time t
    private Vector2 getAcc(float t)
    {
        return 6 * (1 - t) * (endHandle - 2 * startHandle + start) + 6 * t * (end - 2 * endHandle + startHandle);
    }

    public float getCurvature(float t)
    {
        return Math.Abs(Vector2.Cross(getAcc(t), getVelocity(t))) / (float)Math.Pow(getVelocity(t).Length(), 3);
    }

    public float getNextFraction(float t, float arcLength)
    {
        return t + arcLength / getSpeed(t);
    }

    public virtual float getBoost(float t, Key key)
    {
        if (key == Key.A)
        {
            return 1;
        }
        if (key == Key.D)
        {
            return 1.5f;
        }
        return 0;
    }

    //fake, but should work for all practical purposes
    public bool contains(Vector2 loc)
    {
        return loc.X >= start.X && loc.X <= end.X;
    }

    public float nearestFraction(Vector2 loc)
    {
        if(Math.Abs(loc.X - start.X) <= Math.Abs(loc.X - end.X))
        {
            return 0;
        }
        return 1;
    }
}


/*
class CurveGroup
{
    private BezierCurve[] curves;

    public CurveGroup(BezierCurve[] curves)
    {
        this.curves = curves;
    }

    public virtual Vector2 getNearestNormalVector(Vector2 pos)
    {
        return new Vector2(0, -1);
    }

}*/

/*
class BezierCurve : Curve
{
    private Dictionary<Vector2, Vector2> normalVectors;
    private Dictionary<Vector2, float> curvature;

    private Vector2 start;
    private Vector2 startHandle;
    private Vector2 end;
    private Vector2 endHandle;
    private float halfStroke;

    public BezierCurve(Vector2 start, Vector2 startHandle, Vector2 end, Vector2 endHandle)
    {
        this.start = start;
        this.startHandle = startHandle;
        this.end = end;
        this.endHandle = endHandle;

        //getting normal vectors and curvature at points it crosses
        //NOTE, however, this is an approximation, with the incrementing, but should work for all practical purposes
        float t = 0;
        Vector2 currVel = getVelocity(t);
        Vector2 currLoc = getVelocity(0);
        while (t < 1)
        {
            //have to decide if rounding is correct or not here
            normalVectors.Add(currLoc.Rounded(0), currVel.Normalized().Rotated(270));

            t += Math.Min(timeToNearest(currLoc, currVel, CoordinateAxis.X), timeToNearest(currLoc, currVel, CoordinateAxis.Y));
            currVel = getVelocity(t);
            currLoc = getLoc(t);

        }


    }

    private float timeToNearest(Vector2 loc, Vector2 vel, CoordinateAxis direc)
    {
        float velComp = vel.getComp(direc);
        float locComp = loc.getComp(direc);
     
       if (velComp == 0)
        {
            return float.MaxValue;
        } 

        return (float)(velComp > 0 ? (Math.Ceiling(locComp) - locComp) : (Math.Floor(locComp) - locComp)) / vel.Y;
    }

    //gets location at linearly interpolated time t
    private Vector2 getLoc(float t)
    {
        return (float)Math.Pow(1 - t, 3) * start + 3 * (float)Math.Pow(1 - t, 2) * t * startHandle + 3 * (1 - t) * (float)Math.Pow(t, 2) * endHandle + (float)Math.Pow(t, 3) * end;
    }

    //gets velocity at linearly interpolated time t
    private Vector2 getVelocity(float t)
    {
        return 3 * (float)Math.Pow(1 - t, 2) * (startHandle - start) + 6 * (1 - t) * t * (endHandle - startHandle) + 3 * (float)Math.Pow(t, 2) * (end - endHandle);
    }

    //gets acceleration at linearly interpolated time t
    private Vector2 getAcc(float t)
    {
        return 6 * (1 - t) * (endHandle - 2 * startHandle + start) + 6 * t * (end - 2 * endHandle + startHandle);
    }

    private float getCurvature(float t)
    {
        return Vector2.Cross(getAcc(t), getVelocity(t)) / (float) Math.Pow(getVelocity(t).Length(), 3);
    }


}*/

class Rect : Curve
{
    Bounds2 rect;
    string color;
    public Rect(Bounds2 rect, string color)
    {
        this.rect = rect;
        this.color = color;
    }

    public Rect(string[] s, string color)
    {
        rect = new Bounds2(0, 0, 0, 0);
        if (s.Length == 8)
        {
            rect.Position.X = (float)Math.Round(Double.Parse(s[6]));
            rect.Position.Y = (float)Math.Round(Double.Parse(s[7]));
            rect.Size.X = (float)Math.Round(Double.Parse(s[0])) - 1;
            rect.Size.Y = (float)Math.Round(Double.Parse(s[1])) - 1;
        }
        else if (s.Length >= 4)
        {
            rect.Position.X = (float)Math.Round(Double.Parse(s[0]));
            rect.Position.Y = (float)Math.Round(Double.Parse(s[1]));
            rect.Size.X = (float)Math.Round(Double.Parse(s[2])) - 1;
            rect.Size.Y = (float)Math.Round(Double.Parse(s[3])) - 1;
        }
        this.color = color;
    }

    public Vector2 getNearestNormal(Vector2 pos)
    {
        if (!contains(pos))
        {
            return Vector2.UP;
        }
        if (color == "FF0000")
        {
            return Vector2.UP;
        }
        if (pos.Y == rect.Min.Y)
        {
            return Vector2.UP;
        }
        else if (pos.X == rect.Max.X)
        {
            return Vector2.RIGHT;
        }
        else if (pos.X == rect.Min.X)
        {
            return Vector2.LEFT;
        }
        else if (pos.Y == rect.Max.Y)
        {
            return Vector2.DOWN;
        }
        return Vector2.UP;

    }

    public float getNearestCurvature(Vector2 pos)
    {
        return -1;
    }

    public bool contains(Vector2 pos)
    {
        return rect.Contains(pos);
    }
}