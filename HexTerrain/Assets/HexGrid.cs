using System;
using System.Collections;
using System.Collections.Generic;

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