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

	private PlayerController player;
	private SpriteRenderer[,] tileSprites;
	private int vertical, horizontal, columns, rows;

	// Start is called before the first frame update
	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
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
				fields[i, j] = new Field(createSpriteType(i, j));
				fields[i, j].groundHeight = Mathf.Clamp(fields[i, j].groundHeight, -1, 1);
				fields[i, j].waterHeight = Random.Range(0, 2);

				if (fields[i, j].waterHeight == 0 && fields[i, j].groundHeight == 0)
				{
					fields[i, j].hasVillage = true;
					player.villages++;
					player.villagesMax++;
				}

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
	private int EvaluateFlowTarget(int _x, int _y, int waterFountain, int _xMax = 12, int _yMax = 10)
	{
		//nullcheck
		if (_x < 0 || _xMax <= _x || _y < 0 || _yMax <= _y)
			return 10;

		//determine height differance, how much lower is this target
		if (fields[_x, _y] != null)
		{
			int temp = fields[_x, _y].combinedHeight - waterFountain;
			//Debug.Log(temp);
			return temp;
		}
		else
			return 10;
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

						int score = EvaluateFlowTarget(-1, 0, fields[i, j].combinedHeight);
						if (score < bestTarget.z)
						{
							bestTarget = new Vector3Int(-1, 0, score);
						}

						score = EvaluateFlowTarget(1, 0, fields[i, j].combinedHeight);
						if (score < bestTarget.z)
						{
							bestTarget = new Vector3Int(1, 0, score);
						}

						score = EvaluateFlowTarget(0, -1, fields[i, j].combinedHeight);
						if (score < bestTarget.z)
						{
							bestTarget = new Vector3Int(0, -1, score);
						}

						score = EvaluateFlowTarget(0, 1, fields[i, j].combinedHeight);
						if (score < bestTarget.z)
						{
							bestTarget = new Vector3Int(0, 1, score);
						}

						if (bestTarget.z < 0)
						{
							addedWater[bestTarget.x, bestTarget.y] = 1;
						}
					}
				}
			}
		}

		//actually adding the water
		for (int i = 0; i < xMax; i++)
		{
			for (int j = 0; j < yMax; j++)
			{
				if (addedWater[i, j] == 1)
				{
					if (fields[i, j].waterHeight < 5)
						fields[i, j].waterHeight++;

					if (fields[i, j].hasVillage)
					{
						fields[i, j].hasVillage = false;
						player.villages--;
					}

					UpdateSprite(i, j);
				}
			}
		}
	}

	private IEnumerator WaterflowLoop()
	{
		while (true)
		{
			CombinedWaterflow();
			yield return new WaitForSeconds(1f);
		}
	}
	#endregion
}
