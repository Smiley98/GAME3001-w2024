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
    public float cost = 0.0f;
}
