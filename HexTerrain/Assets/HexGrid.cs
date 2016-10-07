using System;
using System.Collections;
using System.Collections.Generic;

#warning HexGrid<T> should be reinstated once Unity supports current Mono / .Net versions. (Should be available with Unity 5.5)

/*
public class HexGrid<T> : IEnumerable<T>
{
	public T this[int x, int y]
	{
		get { return items[new Coord(x, y)]; }
		set { items[new Coord(x, y)] = value; }
	}

	private Dictionary<Coord, T> items = new Dictionary<Coord, T>();

	private struct Coord
	{
		public int x, y;
		public Coord(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	private Coord GetNeighbourCoordOffset(HexEdge direction)
	{
		switch (direction)
		{
			case HexEdge.NorthWest:
				return new Coord(-1, 0);
			case HexEdge.North:
				return new Coord(-1, 1);
			case HexEdge.NorthEast:
				return new Coord(0, 1);
			case HexEdge.SouthEast:
				return new Coord(1, 0);
			case HexEdge.South:
				return new Coord(1, -1);
			case HexEdge.SouthWest:
				return new Coord(0, -1);
			default:
				return new Coord(0, 0);
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		return items.Values.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
*/

#warning HexGrid<T> should replace these classes once it is reinstated.
public abstract class HexGrid
{
	protected struct Coord
	{
		public int x, y;
		public Coord(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	protected Coord GetNeighbourCoordOffset(HexEdge direction)
	{
		switch (direction)
		{
			case HexEdge.NorthWest:
				return new Coord(-1, 0);
			case HexEdge.North:
				return new Coord(-1, 1);
			case HexEdge.NorthEast:
				return new Coord(0, 1);
			case HexEdge.SouthEast:
				return new Coord(1, 0);
			case HexEdge.South:
				return new Coord(1, -1);
			case HexEdge.SouthWest:
				return new Coord(0, -1);
			default:
				return new Coord(0, 0);
		}
	}
}

public class HexTerrainTileGrid : HexGrid, IEnumerable<HexTerrain.Tile>
{
	public HexTerrain.Tile this[int x, int y]
	{
		get { return items[new Coord(x, y)]; }
		set { items[new Coord(x, y)] = value; }
	}

	private Dictionary<Coord, HexTerrain.Tile> items = new Dictionary<Coord, HexTerrain.Tile>();

	public IEnumerator<HexTerrain.Tile> GetEnumerator()
	{
		return items.Values.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}

public class HexTerrainTileGridGrid : HexGrid, IEnumerable<HexTerrainTileGrid>
{
	public HexTerrainTileGrid this[int x, int y]
	{
		get { return items[new Coord(x, y)]; }
		set { items[new Coord(x, y)] = value; }
	}

	private Dictionary<Coord, HexTerrainTileGrid> items = new Dictionary<Coord, HexTerrainTileGrid>();

	public IEnumerator<HexTerrainTileGrid> GetEnumerator()
	{
		return items.Values.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}