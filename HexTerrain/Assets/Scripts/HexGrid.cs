using UnityEngine;
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
    public struct Coord
    {
        public int x, y;
        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public static Coord GetNeighbourCoordOffset(HexEdge direction)
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

public class HexTerrainPillarGrid : HexGrid, IEnumerable<List<HexPillarInfo>>
{
    public List<HexPillarInfo> this[Coord coord]
    {
        get { return items[coord]; }
        set { items[coord] = value; }
    }

    public List<HexPillarInfo> this[int x, int y]
    {
        get { return items[new Coord(x, y)]; }
        set { items[new Coord(x, y)] = value; }
    }

    private Dictionary<Coord, List<HexPillarInfo>> items = new Dictionary<Coord, List<HexPillarInfo>>();

    public bool Add(Coord coord, List<HexPillarInfo> item)
    {
        if (items.ContainsKey(coord))
        {
            Debug.LogWarningFormat("Cannot add item {0} to hex grid at coord {1}; coord already contains an item.", item, coord);
            return false;
        }

        items.Add(coord, item);
        return true;
    }

    public bool TryGetCoordForItem(List<HexPillarInfo> item, out Coord coord)
    {
        foreach (KeyValuePair<Coord, List<HexPillarInfo>> itemPair in items)
        {
            if (itemPair.Value == item)
            {
                coord = itemPair.Key;
                return true;
            }
        }

        coord = new Coord();
        return false;
    }

    public bool ContainsItemAtCoord(Coord coord)
    {
        return items.ContainsKey(coord);
    }

    public IEnumerator<List<HexPillarInfo>> GetEnumerator()
    {
        return items.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}