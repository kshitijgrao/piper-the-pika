using System;

class Chunking
{
    private Texture img1;
    private Texture img2;
    private Texture img3;

    public Chunking(String img1)
    {
        this.img1 = Engine.LoadTexture(img1);
    }

    public Chunking(String img1, String img2, String img3) : this(img1)
    {
        this.img2 = Engine.LoadTexture(img2);
        this.img3 = Engine.LoadTexture(img3);
    }

    public void draw(Vector2 topLeft)
    {
        Engine.DrawTexture(img1, topLeft);
        if (img2 != null)
        {
            Engine.DrawTexture(img2, topLeft + new Vector2(img1.Width, 0));
        }
        if (img3 != null)
        {
            Engine.DrawTexture(img3, topLeft + new Vector2(img1.Width + img2.Width, 0));
        }
    }

    public 
}

