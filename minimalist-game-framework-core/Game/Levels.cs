using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

interface Level
{
    void playLevel();
    void reset();
    Map getMap();
    Difficulty getDiff();
    void setDiff(Difficulty diff);
    Scoreboard getSb();

}

class StageLevel : Level
{
    //physics
    private Map map;
    private Vector2 startingCoord;
    private int finishingThresh;
    private string svg_path;
    public Difficulty diff;

    public Difficulty getDiff() { return diff; }
    public void setDiff(Difficulty diff) { this.diff = diff; }


    //drawing
    private Rendering render;
    string front_pic_path;
    string back_pic_path;
    Vector2 pos;
    Vector2 bgOff;
    Vector2 mapOff;

    //display
    public Scoreboard sb;
    private float maxProgress;
    public LevelPassed passed;
    private int highScore;
    public int levelNum;

    public Scoreboard getSb() { return sb; }

    private Sonic piper;
    public Enemy[] enemies;
    public Flower[] flowers;

    public StageLevel(string map_path, string svg_path, string front_pic_path, string back_pic_path, Vector2 startingCoord, Vector2 pos, Vector2 bgOff, Vector2 mapOff, int finalX, LevelPassed startLevel)
    {
        this.map = new Map(map_path);
        this.render = new Rendering(front_pic_path, back_pic_path, pos, bgOff, mapOff);
        this.svg_path = svg_path;

        this.front_pic_path = front_pic_path;
        this.back_pic_path = back_pic_path;

        this.pos = pos;
        this.bgOff = bgOff;
        this.mapOff = mapOff;

        SVGReader.findAllElementsAndAdd(map, svg_path);

        passed = startLevel;
        maxProgress = 0;
        highScore = 0;
        finishingThresh = finalX;
        this.startingCoord = startingCoord;

        //handling all sprites
        reset();


    }

    public void reset()
    {
        piper = new Sonic(startingCoord);
        sb = new Scoreboard();

        //double reading, but whatever
        enemies = SVGReader.findEnemies(svg_path);
        flowers = SVGReader.findFlowers(svg_path);

        render = new Rendering(front_pic_path, back_pic_path, pos, bgOff, mapOff);

    }


    //what to do in the level every frame
    public void playLevel()
    {
        // collect player input
        Key currentKey = InputHandler.getPlayerInput(piper);


        //collision detection
        //ground and walls
        Physics.detectSolid(piper, map);

        //other sprites
        Physics.detectCollisions(piper, flowers);
        Physics.detectCollisions(piper, enemies);


        //update acceleration
        piper.setAcceleration(currentKey, map);

        //update overall physics
        Physics.updatePhysics(map, piper);
        Physics.updatePhysics(map, enemies);

        // collect input and draw frame

        render.scrollingMotion(piper);

        if (Game.debugToggle)
        {
            if (Engine.GetKeyDown(Key.C))
            {
                render.pos = Game.Resolution / 2 - piper.loc;
            }
        }


        foreach (Enemy enemy in enemies)
        {
            enemy.setFrameIndex(Animator.animateEnemy(enemy, render.pos + enemy.loc));
        }
        foreach (Flower flower in flowers)
        {
            flower.draw(new Bounds2(Vector2.Zero, Flower.defaultFlowerHitbox), flower.loc + render.pos);
        }

        piper.setFrameIndex(Animator.animatePiper(piper, render.pos + piper.loc, currentKey));

        //rings[0].draw(new Bounds2(0, 0, 24, 24), render.pos + rings[0].loc - new Vector2(10,10));


        if (sb.updateScoreboard() == Scene.end)
        {
            Game.currentScene = Scene.end;
        }

        if (piper.loc.X >= finishingThresh)
        {
            if (Game.progress <= passed)
            {
                Game.progress++;
            }
            Game.currentScene = Scene.end;
        }

        if (Engine.GetKeyDown(Key.F3))
        {
            Game.debugToggle = !Game.debugToggle;
        }
        if (Game.debugToggle)
        {
            Engine.DrawRectSolid(new Bounds2(render.pos + piper.loc - new Vector2(1, 1), new Vector2(3, 3)), Color.Red);
            piper.drawVectors(render.pos + piper.loc);

            Engine.DrawString("Pos: " + piper.loc.Rounded(2).ToString(), new Vector2(Game.Resolution.X - 12, 12), Color.Black, Game.arial, TextAlignment.Right);
            Engine.DrawString("onGround? " + piper.onGround, new Vector2(Game.Resolution.X - 12, 24), Color.Black, Game.arial, TextAlignment.Right);
            Engine.DrawString("onPath? " + piper.onPath + " with fraction: " + Math.Round(piper.fractionOfPath, 3), new Vector2(Game.Resolution.X - 12, 36), Color.Black, Game.arial, TextAlignment.Right);
            Engine.DrawString("current normal: " + map.getNormalVector(piper.loc).Rounded(2).ToString() + " current radius: " + Math.Round(map.getSurfaceRadius(piper.loc), 7), new Vector2(Game.Resolution.X - 12, 48), Color.Black, Game.arial, TextAlignment.Right);
            Engine.DrawString("isSpinning? " + piper.isSpinning, new Vector2(Game.Resolution.X - 12, 60), Color.Black, Game.arial, TextAlignment.Right);

            Engine.DrawString("FPS: " + Math.Round(1 / Engine.TimeDelta), new Vector2(Game.Resolution.X - 12, 72), Color.Black, Game.arial, TextAlignment.Right);

            Debug.WriteLine(map.getSurfaceRadius(piper.loc));
        }

        maxProgress = Math.Max(maxProgress, (piper.loc.X - startingCoord.X) / (finishingThresh - startingCoord.X));
        highScore = Math.Max(highScore, sb.getScore());
    }

    public Map getMap()
    {
        return map;
    }

    public LevelPassed getPassed()
    {
        return (LevelPassed) Math.Min((int) Game.progress, (int) passed + 1);
    }

    public float returnPercentCompletion()
    {
        return (float) Math.Round(maxProgress * 100,2);
    }

    public int returnHighScore()
    {
        return highScore;
    }


}

class BossLevel : Level
{
    private Difficulty diff;
    private Map map;
    private Scoreboard sb;

    private Sonic piper;
    private 

    public Difficulty getDiff() { return diff; }
    public void setDiff(Difficulty diff) { this.diff = diff; }
    public Scoreboard getSb() { return sb; }
    public Map getMap() { return map; }

    public BossLevel()
    {

    }


}