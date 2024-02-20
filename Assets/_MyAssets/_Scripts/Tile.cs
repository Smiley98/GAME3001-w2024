using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType : int
{
    GRASS,
    WATER,
    MUD,
    STONE,
    INVALID,
}

public class Tile : MonoBehaviour
{
    public TileType type = TileType.INVALID;
}
