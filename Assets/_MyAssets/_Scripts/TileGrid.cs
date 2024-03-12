using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using Utils;

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
    [SerializeField] Vector2Int goal;
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

        //List<Vector2Int> path = Pathing.FloodFill(start, goal, tiles, 5);
        //foreach (Vector2Int pathItem in path)
        //    Debug.Log(pathItem);

        PriorityQueue<Vector2Int, float> pq = new PriorityQueue<Vector2Int, float>();
        pq.Enqueue(new Vector2Int(1, 1), 3.0f);
        pq.Enqueue(new Vector2Int(2, 2), 2.0f);
        pq.Enqueue(new Vector2Int(3, 3), 1.0f);

        while (pq.Count > 0)
        {
            Vector2Int cell = pq.Dequeue();
            Debug.Log(cell);
        }
    }

    // Type-based colour
    Color TileColor(TileType type)
    {
        Color invalid = Color.magenta;
        if (type != TileType.INVALID)
        {
            Color[] colors = { Color.green, Color.blue, Color.red, Color.grey };
            return colors[(int)type];
        }
        return invalid;
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

        List<Vector2Int> path = Pathing.FloodFill(start, goal, tiles, stepCount);
        // TODO -- write code to move an object along this path.
        // See Connor's Lab 5.docx for additional details.

        // Render floodfill/path in purple
        foreach (Vector2Int cell in path)
        {
            GameObject tile = grid[cell.y][cell.x];
            tile.GetComponent<SpriteRenderer>().color = TileColor(TileType.INVALID);
        }

        GameObject startTile = grid[start.y][start.x];
        GameObject goalTile = grid[goal.y][goal.x];
        startTile.GetComponent<SpriteRenderer>().color = Color.red;
        goalTile.GetComponent<SpriteRenderer>().color = Color.cyan;
    }
}
