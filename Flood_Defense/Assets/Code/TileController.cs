using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FloodDefense
{
	public enum TileTypes { Low, Mid, High };

	public class Feld
	{
		int groundHeigth = 0;
		int waterHeight = 0;
	}

	public class TileController : MonoBehaviour
	{
		[SerializeField] private Tilemap tilemap;

		public Feld[,] felder = new Feld[2,2];

		// Start is called before the first frame update
		void Start()
		{
			//tilemap.WorldToCell
			//tilemap.SetTile(new Vector3Int(1,1,1), TileBase.Instantiate<>)
			//tilemap.GetTile
		}

		// Update is called once per frame
		void Update()
		{

		}

		//Lets the water flow one tick
		private void DoWaterTick()
		{

		}

		private void SetTile(int x, int y, TileTypes tileType)
		{

		}
	}
}
