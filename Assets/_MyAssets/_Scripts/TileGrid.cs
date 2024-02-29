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

public enum TileEditMode
{
    NONE,
    START,
    END
}

public class TileGrid : MonoBehaviour
{
    // Rendering only
    [SerializeField] GameObject tilePrefab;
    List<List<GameObject>> grid = new List<List<GameObject>>();

    // Begin & goal set in inspector
    [SerializeField] Vector2Int start;
    [SerializeField] Vector2Int end;

    [SerializeField] Pathing.DistanceType distanceType;
    TileEditMode tileEditMode = TileEditMode.NONE;

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
        int rowCount = tiles.GetLength(0);
        int colCount = tiles.GetLength(1);
        float xStart = 0.0f + 0.5f;         // left (-x)
        float yStart = rowCount - 0.5f;     // top (+y)
        float x = xStart;
        float y = yStart;
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
        // Define A* evaluation functions
        HeuristicFunction heurFunc = TerrainCost;
        DistanceFunction distFunc =
            distanceType == Pathing.DistanceType.MANHATTAN ?
            Pathing.Manhattan : Pathing.Euclidean;

        // Compute best path between start & end based on eval functions (A*)
        List <Vector2Int> path = Pathing.Find(start, end, tiles, distFunc, heurFunc);

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

        if (Input.GetMouseButtonDown(0))
        {
            tileEditMode = TileEditMode.NONE;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            tileEditMode = TileEditMode.START;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            tileEditMode = TileEditMode.END;
        }

        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int mouseCell = Pathing.WorldToGrid(mouse, tiles);
        switch (tileEditMode)
        {
            case TileEditMode.START:
                start = mouseCell;
                break;

            case TileEditMode.END:
                end = mouseCell;
                break;
        }

        Color startColor = tileEditMode == TileEditMode.START ? Color.cyan : Color.white;
        Color endColor = tileEditMode == TileEditMode.END ? Color.cyan : Color.black;
        grid[start.y][start.x].GetComponent<SpriteRenderer>().color = startColor;
        grid[end.y][end.x].GetComponent<SpriteRenderer>().color = endColor;
    }
}
