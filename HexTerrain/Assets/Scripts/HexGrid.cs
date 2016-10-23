using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * A generic collection which stores data in a hexagonal grid. Where in a normal grid X would
 * represent horizontal position and Y its vertical position, a HexGrid instead uses X to
 * represent south-east distance from the origin and Y to represent north-east distance.
 * 
 * TODO: Reintroduce this class once Unity supports newer versions of Mono / .Net
 * (Should supposedly be supported in Unity 5.5)
 */

/*
[System.Serializable]
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

/**
 * Positional information for a collection which stores data in a hexagonal grid. Where in
 * a normal grid X would represent horizontal position and Y its vertical position, a
 * HexGrid instead uses X to represent south-east distance from the origin and Y to
 * represent north-east distance.
 * 
 * TODO: Replace this with HexGrid<T> once Unity supports newer versions of Mono / .Net
 * (Should supposedly be supported in Unity 5.5)
 */
[System.Serializable]
public abstract class HexGrid
{
    [System.Serializable]
    public struct Coord
    {
        [SerializeField] public int x;
        [SerializeField] public int y;
        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Coord operator +(Coord a, Coord b)
        {
            return new Coord(a.x + b.x, a.y + b.y);
        }

        public static Coord operator -(Coord a, Coord b)
        {
            return new Coord(a.x - b.x, a.y - b.y);
        }
    }

    public static Coord GetNeighbourCoordOffset(HexEdgeDirection direction)
    {
        switch (direction)
        {
            case HexEdgeDirection.NorthWest:
                return new Coord(-1, 0);
            case HexEdgeDirection.North:
                return new Coord(-1, 1);
            case HexEdgeDirection.NorthEast:
                return new Coord(0, 1);
            case HexEdgeDirection.SouthEast:
                return new Coord(1, 0);
            case HexEdgeDirection.South:
                return new Coord(1, -1);
            case HexEdgeDirection.SouthWest:
                return new Coord(0, -1);
            default:
                return new Coord(0, 0);
        }
    }
}

/**
 * A collection of HexPillars stored in a hexagonal grid. Where in a normal grid X would
 * represent horizontal position and Y its vertical position, a HexGrid instead uses X to
 * represent south-east distance from the origin and Y to represent north-east distance.
 * 
 * TODO: Replace this with HexGrid<T> once Unity supports newer versions of Mono / .Net
 * (Should supposedly be supported in Unity 5.5)
 */
[System.Serializable]
public class HexTerrainPillarGrid : HexGrid, IEnumerable<List<HexPillar>>
{
    public List<HexPillar> this[Coord coord]
    {
        get { return items[coord]; }
        set { items[coord] = value; }
    }

    public List<HexPillar> this[int x, int y]
    {
        get { return items[new Coord(x, y)]; }
        set { items[new Coord(x, y)] = value; }
    }

    public int Count
    {
        get { return items.Count; }
    }

    private Dictionary<Coord, List<HexPillar>> items = new Dictionary<Coord, List<HexPillar>>();

    public bool Add(Coord coord, List<HexPillar> item)
    {
        if (items.ContainsKey(coord))
        {
            Debug.LogWarningFormat("Cannot add item {0} to hex grid at coord {1}; coord already contains an item.", item, coord);
            return false;
        }

        items.Add(coord, item);
        return true;
    }

    public void Clear()
    {
        items.Clear();
    }

    public bool TryGetCoordForItem(List<HexPillar> item, out Coord coord)
    {
        foreach (KeyValuePair<Coord, List<HexPillar>> itemPair in items)
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

    public IEnumerator<List<HexPillar>> GetEnumerator()
    {
        return items.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}