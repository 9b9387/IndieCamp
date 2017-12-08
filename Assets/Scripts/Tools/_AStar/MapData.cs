namespace AStar{

	using System;

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
	}
}