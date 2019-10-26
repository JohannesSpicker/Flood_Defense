using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	private GridManager gridManager;
	public Vector2Int activeTile;

	public GameObject cursor;

	// Start is called before the first frame update
	void Start()
    {
		gridManager = GameObject.FindGameObjectWithTag("GridManager")?.GetComponent<GridManager>();
		activeTile = new Vector2Int(0, 0);
	}

    // Update is called once per frame
    void Update()
	{
		if (Input.GetKeyDown(KeyCode.W))
			ChangeActiveTile(activeTile.x, activeTile.y + 1);
		if (Input.GetKeyDown(KeyCode.A))
			ChangeActiveTile(activeTile.x - 1, activeTile.y);
		if (Input.GetKeyDown(KeyCode.S))
			ChangeActiveTile(activeTile.x, activeTile.y - 1);
		if (Input.GetKeyDown(KeyCode.D))
			ChangeActiveTile(activeTile.x + 1, activeTile.y);


		if (Input.GetKeyDown(KeyCode.UpArrow))
			gridManager.Build(activeTile.x, activeTile.y);
		if (Input.GetKeyDown(KeyCode.DownArrow))
			gridManager.Dig(activeTile.x, activeTile.y);

		cursor.transform.position = new Vector3(activeTile.x - (gridManager.horizontal - 0.5f), activeTile.y - (gridManager.vertical - 0.5f));
	}

	private void ChangeActiveTile(int x, int y)
	{
		if (x < 0 || y < 0 || gridManager.columns < x || gridManager.rows < y)
			return;

		activeTile = new Vector2Int(x, y);
	}
}
