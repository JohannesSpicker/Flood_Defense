using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public enum SpriteType { dirt, grass, stone, village, water_step1, water_step2 }

    public class Field
    {
        public SpriteType _spriteType = SpriteType.dirt;
        public int groundHeight = 0;//-1..1
        public int waterHeight = 0;//0..3
        public int combinedHeight { get => groundHeight + waterHeight; }
        public bool hasVillage = false;

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

        #region Accessors
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
        #endregion
    }

    public Sprite[] mySprites;
    public Field[,] fields;
    public GameObject[,] tiles;

    private int turn = 20;

    private PlayerController player;
    private SpriteRenderer[,] tileSprites;
    public UiController uiController;
    private AudioSource audioWave;
    private MapGenerator mapGenerator;
    [HideInInspector] public int vertical, horizontal, columns, rows;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        audioWave = GetComponent<AudioSource>();
        mapGenerator = GetComponent<MapGenerator>();
        vertical = (int)Camera.main.orthographicSize;
        horizontal = vertical * (Screen.width / Screen.height);
        /*
		columns = horizontal * 2;
		rows = vertical * 2;
		*/
        columns = 12;
        rows = 10;

        fields = new Field[columns, rows];
        tiles = new GameObject[columns, rows];
        tileSprites = new SpriteRenderer[columns, rows];

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                fields[i, j] = new Field(0);
                //groundheight
                fields[i, j].groundHeight = mapGenerator.field[i, j];
                //village
                if (fields[i, j].groundHeight == 9)
                {
                    fields[i, j].groundHeight = 0;
                    fields[i, j].hasVillage = true;
                    player.villages++;
                    player.villagesMax++;
                }
                //waterheight
                if (i == 0 && fields[i, j].groundHeight == -1)
                    fields[i, j].waterHeight = 1;

                SpawnTile(i, j, fields[i, j].getSpriteType());
                UpdateSprite(i, j);
            }
        }

        StartCoroutine("WaterflowLoop");
    }

    private void Update()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                UpdateSprite(i, j);
            }
        }
    }

    private void SpawnTile(int x, int y, SpriteType _type)
    {
        GameObject g = new GameObject("X: " + x + "Y: " + y);
        tiles[x, y] = g;
        g.transform.position = new Vector3(x - (horizontal - 0.5f), y - (vertical - 0.5f));
        SpriteRenderer s = g.AddComponent<SpriteRenderer>();
        tileSprites[x, y] = s;
        s.sprite = mySprites[(int)_type];
    }

    private void UpdateSprite(int x, int y)
    {
        if (0 < fields[x, y].waterHeight)
        {
            tileSprites[x, y].sprite = mySprites[(int)SpriteType.water_step1];
        }
        else if (fields[x, y].hasVillage)
        {
            tileSprites[x, y].sprite = mySprites[(int)SpriteType.village];
        }
        else
        {
            int temp = 1 + Mathf.Clamp(fields[x, y].groundHeight, -1, 1);
            tileSprites[x, y].sprite = mySprites[temp];
        }
    }

    public bool Dig(int x, int y)
    {
        if (fields[x, y].waterHeight == 0 && !fields[x, y].hasVillage && -1 < fields[x, y].groundHeight)
        {
            fields[x, y].groundHeight--;
            player.resources++;
            return true;
        }
        else
            return false;
    }

    public bool Build(int x, int y)
    {
        if (0 < player.resources && fields[x, y].waterHeight == 0 && !fields[x, y].hasVillage && fields[x, y].groundHeight < 1)
        {
            fields[x, y].groundHeight++;
            player.resources--;
            UpdateSprite(x, y);
            return true;
        }
        else
            return false;
    }

    #region Waterflow
    private int EvaluateFlowTarget(int _x, int _y, int groundHeight, int _xMax = 12, int _yMax = 10)
    {
        //nullcheck
        if (_x < 0 || _xMax <= _x || _y < 0 || _yMax <= _y)
            return 10;

        //determine height differance, how much lower is this target
        if (fields[_x, _y] != null && fields[_x, _y].waterHeight < 1)
        {
            int temp = fields[_x, _y].groundHeight - groundHeight;
            //Debug.Log(temp);
            return temp;
        }
        else
            return 10;
    }

    private void Splash(int[,] addedWater, int limit, int xMax = 12, int yMax = 10)
    {
        for (int i = 0; i < xMax; i++)
        {
            for (int j = 0; j < yMax; j++)
            {
                if (null != fields[i, j])
                {
                    if (0 < fields[i, j].waterHeight)
                    {
                        Vector3Int bestTarget = new Vector3Int(-2, -2, 5);
                        //go through the 4 possible targets and check for the best one with EvaluateFLowTarget()

                        int score = EvaluateFlowTarget(i + -1, j + 0, fields[i, j].groundHeight);
                        if (score < bestTarget.z)
                        {
                            bestTarget = new Vector3Int(i + -1, j + 0, score);
                        }

                        score = EvaluateFlowTarget(i + 1, j + 0, fields[i, j].groundHeight);
                        if (score < bestTarget.z)
                        {
                            bestTarget = new Vector3Int(i + 1, j + 0, score);
                        }

                        score = EvaluateFlowTarget(i + 0, j + -1, fields[i, j].groundHeight);
                        if (score < bestTarget.z)
                        {
                            bestTarget = new Vector3Int(i + 0, j + -1, score);
                        }

                        score = EvaluateFlowTarget(i + 0, j + 1, fields[i, j].groundHeight);
                        if (score < bestTarget.z)
                        {
                            bestTarget = new Vector3Int(i + 0, j + 1, score);
                        }

                        if (bestTarget.z < limit)
                        {
                            addedWater[bestTarget.x, bestTarget.y] = 1;
                        }
                    }
                }
            }
        }
    }

    private int AddWater(int[,] addedWater, int waterSet, int limit, int xMax = 12, int yMax = 10)
    {

        for (int i = 0; i < xMax; i++)
        {
            for (int j = 0; j < yMax; j++)
            {
                if (addedWater[i, j] == 1)
                {
                    waterSet--;
                    if (limit > 0 && waterSet < 0)
                    {
                        return waterSet;
                    }
                    if (fields[i, j].waterHeight < 5)
                    {
                        //fields[i, j].waterHeight++;
                        fields[i, j].waterHeight = 1;
                        fields[i, j].groundHeight = -1;
                    }

                    if (fields[i, j].hasVillage)
                    {
                        fields[i, j].hasVillage = false;
                        player.villages--;
                        if (player.villages == 0)
                            uiController.SetPanelLose();
                    }

                    UpdateSprite(i, j);
                }
            }
        }
        return waterSet;
    }

    private void CombinedWaterflow(int xMax = 12, int yMax = 10)
    {
        //16, 10
        int[,] addedWater = new int[xMax, yMax];

        for (int i = 0; i < xMax; i++)
        {
            for (int j = 0; j < yMax; j++)
            {
                addedWater[i, j] = 0;
            }
        }

        //actually adding the water
        int waterSet = 5;
        int limit = 0;
        while (waterSet > 0)
        {
            Splash(addedWater, limit);
            waterSet = AddWater(addedWater, waterSet, limit);
            limit++;
        }

        turn--;

        if (turn <= 0)
        {
            IntroController.gameWon = true;
            uiController.SetPanelWin();
        }


    }

    private IEnumerator WaterflowLoop()
    {
        while (player.villages > 0 && !IntroController.gameWon)
        {

            yield return new WaitForSeconds(5f);

            CombinedWaterflow();
            audioWave.PlayOneShot(audioWave.clip);
        }
    }
    #endregion
}
