using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;
    List<List<GameObject>> grid = new List<List<GameObject>>();
    int rowCount = 10;      // vertical tile count
    int colCount = 20;      // horizontal tile count

    // Begin & goal set in inspector
    [SerializeField] Vector2Int start;
    [SerializeField] Vector2Int end;

    Queue<Vector2Int> frontier = new Queue<Vector2Int>();
    HashSet<Vector2Int> reached = new HashSet<Vector2Int>();

    // Tile types (dictates the properties of each tile)
    int[,] tiles =
    {
        { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
        { 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
        { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 }
    };

    // Initialize 2D list of tile game objects based on tile integer 2D array 
    void Start()
    {
        float xStart = 0.0f + 0.5f;    // left (-x)
        float yStart = 0.0f + 0.5f;    // bottom (-y)
        float x = xStart;
        float y = yStart;

        for (int row = 0; row < rowCount; row++)
        {
            // Allocate space for each incoming row (a row is a collection of columns)
            grid.Add(new List<GameObject>());
            for (int col = 0; col < colCount; col++)
            {
                // For every column in the row, we must create a tile and then store it!
                GameObject tile = Instantiate(tilePrefab);
                tile.GetComponent<Tile>().type = (TileType)tiles[row, col];
                tile.transform.position = new Vector3(x, y);
                grid[row].Add(tile);    // Store the resultant tile in the 2D grid list
                x += 1.0f;              // Step 1 unit right each iteration
            }
            // Reset column position and increment row position when current row finishes
            x = xStart;
            y += 1.0f;
        }

        // Must re-compute tile costs every time start or end is changed
        UpdateTileCosts(end);

        // Adds the bottom row of tiles to our queue:
        for (int col = 0; col < colCount; col++)
        {
            frontier.Enqueue(new Vector2Int(col, 0));
        }

        // Loops through all elements in the queue WITHOUT removing anything
        foreach (Vector2Int cell in frontier)
        {
            Debug.Log(cell);
        }

        // Loops throught the queue and deletes each element
        while (frontier.Count > 0)
        {
            Debug.Log(frontier.Dequeue());
        }

        // (This logs nothing because there's nothing left in the queue)
        foreach (Vector2Int cell in frontier)
        {
            Debug.Log(cell);
        }
    }

    // Set each tile's sprite colour based on its tile type
    void ColorGrid()
    {
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                GameObject tile = grid[row][col];
                TileType type = tile.GetComponent<Tile>().type;
                tile.GetComponent<SpriteRenderer>().color = TileColor(type);
            }
        }
    }

    // Color based on tile type
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

    // Quantization
    Vector2Int WorldToGrid(Vector2 position)
    {
        Vector2Int cell = new Vector2Int((int)position.x, (int)position.y);
        cell.x = Mathf.Clamp(cell.x, 0, colCount - 1);
        cell.y = Mathf.Clamp(cell.y, 0, rowCount - 1);
        return cell;
    }

    // Localization
    Vector2 GridToWorld(Vector2Int cell)
    {
        cell.x = Mathf.Clamp(cell.x, 0, colCount - 1);
        cell.y = Mathf.Clamp(cell.y, 0, rowCount - 1);
        return new Vector2(cell.x + 0.5f, cell.y + 0.5f);
    }

    // Adjacent & diagonal tiles
    List<GameObject> Neighbours(Vector2Int cell)
    {
        // Bounding checks
        bool bot = cell.y - 1 >= 0;
        bool top = cell.y + 1 < rowCount;
        bool left = cell.x - 1 >= 0;
        bool right = cell.x + 1 < colCount;

        List<GameObject> neighbours = new List<GameObject>();

        // Adjacent
        if (bot) neighbours.Add(grid[cell.y - 1][cell.x]);
        if (top) neighbours.Add(grid[cell.y + 1][cell.x]);
        if (left) neighbours.Add(grid[cell.y][cell.x - 1]);
        if (right) neighbours.Add(grid[cell.y][cell.x + 1]);

        // Diagonals
        if (bot && left) neighbours.Add(grid[cell.y - 1][cell.x - 1]);
        if (top && left) neighbours.Add(grid[cell.y + 1][cell.x - 1]);
        if (bot && right) neighbours.Add(grid[cell.y - 1][cell.x + 1]);
        if (top && right) neighbours.Add(grid[cell.y + 1][cell.x + 1]);

        return neighbours;
    }

    // Scores left-right-up-down only
    float Manhattan(Vector2Int cell1, Vector2Int cell2)
    {
        return Mathf.Abs(cell1.x - cell2.x) + Mathf.Abs(cell1.y - cell2.y);
    }

    // Prefers diagonals over left-right-up-down
    float Euclidean(Vector2Int cell1, Vector2Int cell2)
    {
        return Vector2Int.Distance(cell1, cell2);
    }

    // Cost of each tile with respect to the goal
    float Cost(Vector2Int current, Vector2Int goal)
    {
        Tile tile = grid[current.y][current.x].GetComponent<Tile>();

        float terrainCost = 0.0f;
        switch (tile.type)
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

        float distanceCost = Manhattan(current, goal);
        return terrainCost + distanceCost;
    }

    // Recompute cost of each tile with respect to the goal
    void UpdateTileCosts(Vector2Int goal)
    {
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                Tile tile = grid[row][col].GetComponent<Tile>();
                tile.cost = Cost(new Vector2Int(col, row), goal);
            }
        }
    }

    void FloodFill(Vector2Int cell)
    {

    }

    void Update()
    {
        // Reset every tile's sprite color to white every frame for testing
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                GameObject tile = grid[row][col];
                tile.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }

        // Color each tile based on its type (stored in the Tile script component)
        ColorGrid();

        // Convert cursor from world to grid space (quantization)
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int cell = WorldToGrid(mouse);

        // Neighbours test
        //List<GameObject> neighbours = Neighbours(cell);
        //for (int i = 0; i < neighbours.Count; i++)
        //{
        //    GameObject go = neighbours[i];
        //    Tile tile = go.GetComponent<Tile>();
        //    go.GetComponent<SpriteRenderer>().color = TileColor(tile.type);
        //}

        // I have no idea how to render per-object text in Unity...
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log($"Current tile [{cell.y}, {cell.x}] costs {Cost(cell, end)}");
        }

        // Render start and end
        grid[start.y][start.x].GetComponent<SpriteRenderer>().color = TileColor(TileType.INVALID);
        grid[end.y][end.x].GetComponent<SpriteRenderer>().color = TileColor(TileType.INVALID);
    }
}
