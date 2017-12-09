namespace AStar{

	using System;
	using UnityEngine;

	public enum TileType {
		Empty,
		Block,
	}

	[Serializable]
	public class MapData
	{
		public string mapName;
		public int mapWidth;
		public int mapHeight;
		public TileType[] tileTypes;
		public Vector2[] positions;
	}
}