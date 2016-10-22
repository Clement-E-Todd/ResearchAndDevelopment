/**
 * An enum representing the cardinal direction that the corner of a hex is pointing.
 */
public enum HexCornerDirection
{
    INVALID = -1,
    East,
    SouthEast,
    SouthWest,
    West,
    NorthWest,
    NorthEast,
    MAX
}

/**
 * An enum representing the cardinal direction that the edge of a hex is facing.
 */
public enum HexEdgeDirection
{
    INVALID = -1,
    SouthEast,
    South,
    SouthWest,
    NorthWest,
    North,
    NorthEast,
    MAX
}