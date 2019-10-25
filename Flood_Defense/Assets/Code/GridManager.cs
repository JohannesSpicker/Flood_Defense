using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
	public enum SpriteType { dirt, grass, stone, water_step1, water_step2 }

	public class Field
	{
		public SpriteType _spriteType = SpriteType.water_step1;
		public int groundHeigth = 0;
		public int waterHeight = 0;

		public int combinedHeight { get => groundHeigth + waterHeight; }
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
		//grid = new Field[columns, rows];

		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				fields[i, j] = new Field();
				//grid[i, j] =  Random.Range(0f, 10f);//unsere textur hier rein
				SpawnTile(i, j, fields[i, j]._spriteType);
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
}
