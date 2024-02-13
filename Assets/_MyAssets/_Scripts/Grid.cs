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

public class Grid : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;
    List<List<GameObject>> grid = new List<List<GameObject>>();
    int rowCount = 10;      // vertical tile count
    int colCount = 20;      // horizontal tile count

    TileType tileState = TileType.INVALID;

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
                tile.transform.position = new Vector3(x, y);
                x += 1.0f;              // Step 1 unit right each iteration
                grid[row].Add(tile);    // Store the resultant tile in the 2D grid list
            }
            // Reset column position and increment row position when current row finishes
            x = xStart;
            y += 1.0f;
        }

        Debug.Log("Tile state: " + tileState);
    }

    void ColorGrid()
    {
        // Make increasingly red as we move right, make increasingly green as we move up
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                //Vector2 position = tile.transform.position;
                //position = new Vector2(position.x / colCount, position.y / rowCount);
                //tile.GetComponent<SpriteRenderer>().color = new Color(position.x, position.y, 0.0f, 1.0f);
                
                GameObject tile = grid[row][col];
                TileType type = (TileType)tiles[row, col];
                tile.GetComponent<SpriteRenderer>().color = TileColor(type);
            }
        }

        // Quantization test
        //Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector2Int cell = WorldToGrid(mouse);
        //grid[cell.y][cell.x].GetComponent<SpriteRenderer>().color = Color.magenta;

        // Localization test
        //Vector2 world = GridToWorld(cell);
        //GameObject test = Instantiate(tilePrefab);
        //test.transform.position = world;
        //test.GetComponent<SpriteRenderer>().color = Color.magenta;
    }

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

    void Update()
    {
        ColorGrid();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            int state = (int)tileState;
            ++state;
            state %= (int)TileType.INVALID;
            tileState = (TileType)state;
            Debug.Log("Tile state: " + tileState);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            tileState = TileType.INVALID;
            Debug.Log("Tile state: " + tileState);
        }

        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int cell = WorldToGrid(mouse);
        if (Input.GetKeyDown(KeyCode.Space) && tileState != TileType.INVALID)
        {
            tiles[cell.y, cell.x] = (int)tileState;
        }

        grid[cell.y][cell.x].GetComponent<SpriteRenderer>().color = TileColor(tileState);
    }
}
