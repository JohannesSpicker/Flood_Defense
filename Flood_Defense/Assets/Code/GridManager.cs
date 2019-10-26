using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public enum SpriteType { dirt, grass, stone, water_step1, water_step2 }

    public class Field
    {
        public SpriteType _spriteType = (SpriteType)0;
        public int groundHeight = 0;
        public int waterHeight = 0;
        public int combinedHeight { get => groundHeight + waterHeight; }

        public Field(int i)
        {
            this.groundHeight = i;

            if (i < 0)
            {
                this._spriteType = SpriteType.dirt;
            }
            else if (i == 0)
            {
                this._spriteType = SpriteType.grass;
            }
            else if (i > 0)
            {
                this._spriteType = SpriteType.stone;
            }

        }

        public SpriteType getSpriteType()
        {
            return _spriteType;
        }

        public int getGroundHeight()
        {
            return groundHeight;
        }

        public int getWaterHeight()
        {
            return waterHeight;
        }

        public int getCombinedHeight()
        {
            return combinedHeight;
        }
    }

    public Sprite[] sprite;
    public float[,] grid;
    public Field[,] fields;
    int vertical, horizontal, columns, rows;

    // Start is called before the first frame update
    void Start()
    {
        vertical = (int)Camera.main.orthographicSize;
        horizontal = vertical * (Screen.width / Screen.height);
        columns = horizontal * 2;
        rows = vertical * 2;
        grid = new float[columns, rows];
        fields = new Field[columns, rows];

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                fields[i, j] = new Field(createSpriteType(i, j));
                SpawnTile(i, j, fields[i, j].getSpriteType());
            }
        }


    }

    private void SpawnTile(int x, int y, SpriteType _type)
    {
        GameObject g = new GameObject("X: " + x + "Y: " + y);
        g.transform.position = new Vector3(x - (horizontal - 0.5f), y - (vertical - 0.5f));
        //g.transform.position = new Vector3(x - (horizontal), y - (vertical));
        SpriteRenderer s = g.AddComponent<SpriteRenderer>();
        //s.size = new Vector2(0.0f, 0.5f);
        s.sprite = sprite[(int)_type];
        //s.color = new Color(value, value, value);
    }

    public int createSpriteType(int i, int j)
    {
        int minHeight = 0;
        int maxHeight = 0;

        if ((i > 0 && i < horizontal) && (j > 0 && j < vertical))
        {
            if (fields[i - 1, j - 1] != null)
            {
                maxHeight = minHeight = fields[i - 1, j - 1].getCombinedHeight();
            }
            else
            {
                maxHeight = 2;
                minHeight = -2;
            }

            //i-1, j
            if (fields[i - 1, j] != null && fields[i - 1, j].getCombinedHeight() > maxHeight)
            {
                maxHeight = fields[i - 1, j].getCombinedHeight();
            }
            else if (fields[i - 1, j] != null && fields[i - 1, j].getCombinedHeight() < minHeight)
            {
                minHeight = fields[i - 1, j].getCombinedHeight();
            }

            //i-1, j+1
            if (fields[i, j + 1] != null && fields[i, j + 1].getCombinedHeight() > maxHeight)
            {
                maxHeight = fields[i, j + 1].getCombinedHeight();
            }
            else if (fields[i, j + 1] != null && fields[i, j + 1].getCombinedHeight() < minHeight)
            {
                minHeight = fields[i, j + 1].getCombinedHeight();
            }

            //i, j-1
            if (fields[i, j - 1] != null && fields[i, j - 1].getCombinedHeight() > maxHeight)
            {
                maxHeight = fields[i, j - 1].getCombinedHeight();
            }
            else if (fields[i, j - 1] != null && fields[i, j - 1].getCombinedHeight() < minHeight)
            {
                minHeight = fields[i, j - 1].getCombinedHeight();
            }

            //i, j+1
            if (fields[i, j + 1] != null && fields[i, j + 1].getCombinedHeight() > maxHeight)
            {
                maxHeight = fields[i - 1, j].getCombinedHeight();
            }
            else if (fields[i, j + 1] != null && fields[i, j + 1].getCombinedHeight() < minHeight)
            {
                minHeight = fields[i, j + 1].getCombinedHeight();
            }

            //i+1, j-1
            if (fields[i + 1, j - 1] != null)
            {
                maxHeight = fields[i + 1, j - 1].getCombinedHeight();
            }
            else if (fields[i + 1, j - 1] != null && fields[i + 1, j - 1].getCombinedHeight() < minHeight)
            {
                minHeight = fields[i + 1, j - 1].getCombinedHeight();
            }

            //i+1, j
            if (fields[i + 1, j] != null && fields[i + 1, j].getCombinedHeight() > maxHeight)
            {
                maxHeight = fields[i + 1, j].getCombinedHeight();
            }
            else if (fields[i + 1, j] != null && fields[i + 1, j].getCombinedHeight() < minHeight)
            {
                minHeight = fields[i + 1, j].getCombinedHeight();
            }

            //i+1, j+1
            if (fields[i + 1, j + 1] != null && fields[i + 1, j + 1].getCombinedHeight() > maxHeight)
            {
                maxHeight = fields[i, j + 1].getCombinedHeight();
            }
            else if (fields[i + 1, j + 1] != null && fields[i + 1, j + 1].getCombinedHeight() < minHeight)
            {
                minHeight = fields[i + 1, j + 1].getCombinedHeight();
            }

        }
        else
        {
            maxHeight = 2;
            minHeight = -2;
        }

        int groundheight = Random.Range(minHeight, maxHeight);

        return groundheight;



        //i-1, j-1



    }
}
