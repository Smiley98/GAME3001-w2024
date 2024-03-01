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
    [SerializeField] GameObject tilePrefab;
    List<List<GameObject>> grid = new List<List<GameObject>>();

    [SerializeField] Vector2Int start;
    [SerializeField] int stepCount;

    // Tile types (dictates the properties of each tile)
    int[,] tiles =
    {
        { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
        { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 3, 0, 0, 3, 3, 3, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
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
        int rows = tiles.GetLength(0);
        int cols = tiles.GetLength(1);
        float xStart = 0.0f + 0.5f;     // left (-x)
        float yStart = rows - 0.5f;     // top (+y)
        float x = xStart;
        float y = yStart;
        for (int row = 0; row < rows; row++)
        {
            grid.Add(new List<GameObject>());
            for (int col = 0; col < cols; col++)
            {
                GameObject tile = Instantiate(tilePrefab);
                tile.transform.position = new Vector3(x, y);
                grid[row].Add(tile);
                x += 1.0f;
            }
            // Reset column position and increment row position when current row finishes
            x = xStart;
            y -= 1.0f;
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

    void Update()
    {
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

        // Flood-fill in purple
        List<Vector2Int> cells = Pathing.FloodFill(start, tiles, stepCount);
        foreach (Vector2Int cell in cells)
        {
            GameObject tile = grid[cell.y][cell.x];
            tile.GetComponent<SpriteRenderer>().color = TileColor(TileType.INVALID);
        }
    }
}
