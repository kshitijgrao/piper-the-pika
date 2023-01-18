using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.Emit;

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
        {"through" , "#FF0000" },
        {"flying_enemy", "#0E770B" },
        {"ground_enemy", "#05FF00"},
        {"flower", "#FF00F5" },
        {"spike", "#0028FC" }
    };
    public static void findAllElementsAndAdd(Map map, string path)
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
                addPath(line, map);
            }

        }
    }

    public static Enemy[] findEnemies(string path)
    {
        string[] lines = File.ReadAllLines(path);

        List<Enemy> enemiesOut = new List<Enemy>();

        foreach(string line in lines)
        {
            if (line.Length < 5 || line.Substring(0, 5) != "<line") { continue; }
            if (line.Contains(ColorTypes["flying_enemy"]))
            {
                enemiesOut.Add(getEnemy(line, true));
            }
            else if (line.Contains(ColorTypes["ground_enemy"]))
            {
                enemiesOut.Add(getEnemy(line, false));
            }
        }

        return enemiesOut.ToArray();
    }

    private static Enemy getEnemy(string line, bool flying)
    {
        float[] coords = new float[4];
        string[] stringCoords = line.Substring(10, line.Length - 30).Replace("y", "").Replace("x", "").Replace("1=", "").Replace("2=", "").Replace("\"", "").Split(' ');

        for (int i = 0; i < 4 && i < stringCoords.Length; i++)
        {
            coords[i] = float.Parse(stringCoords[i]);
        }

        System.Diagnostics.Debug.WriteLine(coords[0] + " " + coords[1] + " ");
        Enemy enemyToAdd = new Enemy(new Vector2(coords[0], coords[1]), new Bounds2(coords[0], coords[1], coords[2], coords[3]), flying);
        enemyToAdd.setState(State.Walk);
        return enemyToAdd;
    }

    public static Flower[] findFlowers(string path)
    {
        string[] lines = File.ReadAllLines(path);

        List<Flower> flowersOut = new List<Flower>();

        foreach (string line in lines)
        {
            if (line.Length < 5 || line.Substring(0, 5) != "<rect") { continue; }
            if (line.Contains(ColorTypes["flower"]))
            {
                string[] stringCoords = line.Substring(9, line.Length - 48).Split("\" y=\"");
                flowersOut.Add(new Flower(new Vector2(float.Parse(stringCoords[0]), float.Parse(stringCoords[1]))));
            }
        }

        return flowersOut.ToArray();
    }


    private static bool isCurve(string line)
    {
        if(line.Length <= 16)
        {
            return false;
        }
        return line.Substring(0, 5) == "<rect" && (line.Substring(line.Length - 10, 7) == ColorTypes["ground"] || line.Substring(line.Length - 10, 7) == ColorTypes["through"] || line.Substring(line.Length - 10, 7) == ColorTypes["spike"]);
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
    float getBoost(float t);

    bool contains(Vector2 loc);
    float nearestFraction(Vector2 loc);
}


class PathGroup : Path2 
{
    private Path2[] paths;

    public PathGroup(params Path2[] pathList)
    {
        paths = pathList;
    }

    public Vector2 getPoint(float t) { return paths[getIndex(t)].getPoint(convertTime(t)); }
    public Vector2 getTangent(float t) { return paths[getIndex(t)].getTangent(convertTime(t)); }
    public float getCurvature(float t) { return paths[getIndex(t)].getCurvature(convertTime(t)); }
    public float getSpeed(float t) { return paths[getIndex(t)].getSpeed(convertTime(t)) * paths.Length; }
    public float getNextFraction(float t, float arcLength) { return paths[getIndex(t)].getNextFraction(convertTime(t), arcLength) / paths.Length + ((float)getIndex(t)) / paths.Length; }
    public float getBoost(float t) { return paths[getIndex(t)].getBoost(convertTime(t)); }
    public bool contains(Vector2 loc) 
    {
        foreach (Path2 path in paths)
        {
            if (path.contains(loc))
            {
                return true;
            }
        }
        return false;
    }
    private float convertTime(float t)
    {
        return (t - ((float)getIndex(t)) / paths.Length) * paths.Length;
    }

    public float nearestFraction(Vector2 loc)
    {
        foreach(Path2 path in paths)
        {
            if (path.contains(loc))
            {
                return path.nearestFraction(loc);
            }

        }
        return paths[paths.Length - 1].nearestFraction(loc);
    }

    private int getIndex(float t)
    {
        if (t == 1)
        {
            return paths.Length - 1;
        }
        return (int)Math.Floor(t * paths.Length);
    }
}


class BezierGroup : PathGroup
{
    public BezierGroup(params Vector2[] coordList) : base(calculatePaths(coordList))
    {
        
        
    }

    public BezierGroup(params float[] coordList) : base(calculatePaths(coordList))
    {
    }

    private static Path2[] calculatePaths(Vector2[] coordList)
    {
        BezierCurveNoStroke[] bezierCurves = new BezierCurveNoStroke[(coordList.Length - 1) / 3];
        for (int i = 0; i < bezierCurves.Length; i++)
        {
            bezierCurves[i] = new BezierCurveNoStroke(coordList[3 * i], coordList[3 * i + 1], coordList[3 * i + 2], coordList[3 * i + 3]);
        }
        return bezierCurves;
    }

    private static Path2[] calculatePaths(float[] coordList)
    {
        BezierCurveNoStroke[] bezierCurves = new BezierCurveNoStroke[(coordList.Length - 2) / 6];
        for (int i = 0; i < bezierCurves.Length; i++)
        {
            bezierCurves[i] = new BezierCurveNoStroke(new Vector2(coordList[6 * i], coordList[6 * i + 1]),
                new Vector2(coordList[6 * i + 2], coordList[6 * i + 3]),
                new Vector2(coordList[6 * i + 4], coordList[6 * i + 5]),
                new Vector2(coordList[6 * i + 6], coordList[6 * i + 7]));
        }
        return bezierCurves;
    }
}

class BezierCurveNoStroke : Path2
{
    private Vector2 start;
    private Vector2 startHandle;
    private Vector2 end;
    private Vector2 endHandle;

    private Bounds2 bounds;

    public BezierCurveNoStroke(Vector2 start, Vector2 startHandle, Vector2 endHandle, Vector2 end)
    {
        float minX, minY, maxX, maxY;
        minX = Math.Min(start.X, end.X);
        maxX = Math.Max(start.X, end.X);
        minY = Math.Min(start.Y, end.Y);
        maxY = Math.Max(start.Y, end.Y);

        bounds = new Bounds2(minX, minY, maxX - minX, maxY - minY);
        bounds.Size += new Vector2(24, 24);
        bounds.Position -= new Vector2(12, 12);

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

    public virtual float getBoost(float t)
    {
        return 1;
    }

    //fake, but should work for all practical purposes
    public bool contains(Vector2 loc)
    {
        return bounds.Contains(loc);
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


class Arc : Path2
{
    private Vector2 center;
    private float radius;
    private float startAngle;
    private float endAngle;
    

    public Arc(Vector2 center, float radius, float startAngle, float endAngle)
    {
        this.center = center;
        this.radius = radius;
        this.startAngle =  startAngle * (float)Math.PI / 180;
        this.endAngle = endAngle * (float)Math.PI / 180;
    }
    public Vector2 getPoint(float t)
    {
        return center + radius * (Vector2.RIGHT * (float) Math.Cos(getAngle(t)) + Vector2.UP * (float) Math.Sin(getAngle(t)));
    }
    public Vector2 getTangent(float t)
    {
        return Vector2.UP.Rotated(-1 * getAngle(t) * 180 / (float) Math.PI);
    }
    public float getCurvature(float t)
    {
        return 1 / radius;
    }
    public float getSpeed(float t)
    {
        return radius * Math.Abs(endAngle - startAngle);
    }
    public float getNextFraction(float t, float arcLength)
    {
        return t + arcLength / getSpeed(t);
    }
    public float getBoost(float t)
    {
        return 0;
    }

    public bool contains(Vector2 loc)
    {
        Vector2 shift = loc - center;
        if(Math.Abs(shift.Length() - radius) < Map.CLOSE_THRESHOLD)
        {
            float angle = findAngle(loc);
            if(angle <= endAngle && angle >= startAngle)
            {
                return true;
            }
        }
        return false;
    }
    public float nearestFraction(Vector2 loc)
    {
        float test = (findAngle(loc) - startAngle) / (endAngle - startAngle);
        if(test < 0)
        {
            return 0;
        }
        if(test > 1)
        {
            return 1;
        }
        return test;
    }

    //in radians
    private float findAngle(Vector2 loc)
    {
        Vector2 shift = loc - center;
        float initialAngle = (float)Math.Acos(Vector2.Dot(shift.Normalized(), Vector2.RIGHT));
        if (shift.Y < 0)
        {
            initialAngle = (float)(2 * Math.PI) - initialAngle;
        }
        return initialAngle;
    }

    private float getAngle(float t)
    {
        return (endAngle - startAngle) * t + startAngle;
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

            rect.Position -= rect.Size;
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

        Dictionary<float, Vector2> storeNormal = new Dictionary<float, Vector2>{
            {pos.Y - rect.Min.Y, Vector2.UP},
            {pos.Y - rect.Max.Y, Vector2.DOWN },
            {pos.X - rect.Min.X, Vector2.LEFT },
            {pos.X - rect.Max.X, Vector2.RIGHT }
        };

        float minimum = Math.Min(Math.Min(Math.Abs(pos.Y - rect.Min.Y), Math.Abs(pos.Y - rect.Max.Y)), Math.Min(Math.Abs(pos.X - rect.Min.X), Math.Abs(pos.X - rect.Max.X)));

        return storeNormal[minimum];


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