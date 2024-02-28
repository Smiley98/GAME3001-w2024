using System;
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

public class TileGrid : MonoBehaviour
{
    // Rendering only
    [SerializeField] GameObject tilePrefab;
    List<List<GameObject>> grid = new List<List<GameObject>>();

    // Begin & goal set in inspector
    [SerializeField] Vector2Int start;
    [SerializeField] Vector2Int end;

    // Tile types (dictates the properties of each tile)
    int[,] tiles =
    {
        { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
        { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 }
    };

    // Create 2D list of game objects to represent tile positions in the world
    void Start()
    {
        float xStart = 0.0f + 0.5f;    // left (-x)
        float yStart = 0.0f + 0.5f;    // bottom (-y)
        float x = xStart;
        float y = yStart;
        int rowCount = tiles.GetLength(0);
        int colCount = tiles.GetLength(1);
        for (int row = 0; row < rowCount; row++)
        {
            grid.Add(new List<GameObject>());
            for (int col = 0; col < colCount; col++)
            {
                GameObject tile = Instantiate(tilePrefab);
                tile.transform.position = new Vector3(x, y);
                grid[row].Add(tile);
                x += 1.0f;
            }
            // Reset column position and increment row position when current row finishes
            x = xStart;
            y += 1.0f;
        }
    }

    // Type-based colour
    Color TileColor(TileType type)
    {
        Color color = Color.white;
        switch (type)
        {
            case TileType.GRASS:
                color = Color.green;
                break;

            case TileType.WATER:
                color = Color.blue;
                break;

            case TileType.MUD:
                color = Color.red;
                break;

            case TileType.STONE:
                color = Color.grey;
                break;

            case TileType.INVALID:
                color = Color.magenta;
                break;
        }
        return color;
    }

    // Type-based movement cost
    float TerrainCost(int type)
    {
        float terrainCost = 0.0f;
        switch ((TileType)type)
        {
            case TileType.GRASS:
                terrainCost = 10.0f;
                break;

            case TileType.WATER:
                terrainCost = 20.0f;
                break;

            case TileType.MUD:
                terrainCost = 50.0f;
                break;

            case TileType.STONE:
                terrainCost = 100.0f;
                break;
        }
        return terrainCost;
    }

    void Update()
    {
        // A*
        List<Vector2Int> path = Pathing.Find(start, end, tiles,
            Pathing.Manhattan, TerrainCost);

        // Revert each tile to its type-based colour
        int rowCount = tiles.GetLength(0);
        int colCount = tiles.GetLength(1);
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                TileType type = (TileType)tiles[row, col];
                GameObject tile = grid[row][col];
                tile.GetComponent<SpriteRenderer>().color = TileColor(type);
            }
        }

        // Color path tiles purple
        foreach (Vector2Int cell in path)
        {
            GameObject tile = grid[cell.y][cell.x];
            tile.GetComponent<SpriteRenderer>().color = TileColor(TileType.INVALID);
        }
    }
}
