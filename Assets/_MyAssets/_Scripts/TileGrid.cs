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

    [SerializeField] GameObject playerOutline;
    [SerializeField] Transform player;
    [SerializeField] Transform viewer;
    int currentIndex = 0;
    int nextIndex = 1;
    float t = 0.0f;

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

    void FollowPath()
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

        List<Vector2Int> path = Pathing.FloodFill(start, goal, tiles, 16);
        Vector2Int current = path[currentIndex];
        Vector2Int next = path[nextIndex];

        Vector3 currentWorld = Pathing.GridToWorld(current, tiles);
        Vector3 nextWorld = Pathing.GridToWorld(next, tiles);
        player.position = Vector3.Lerp(currentWorld, nextWorld, t);
        t += Time.deltaTime;

        if (Vector3.Distance(player.position, nextWorld) <= 0.01f && next != goal)
        {
            currentIndex++;
            nextIndex++;
            t = 0.0f;
        }

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

    void Update()
    {
        // Revert each tile to white
        int rowCount = tiles.GetLength(0);
        int colCount = tiles.GetLength(1);
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                grid[row][col].GetComponent<SpriteRenderer>().color = Color.white;
            }
        }

        Vector3 toPlayer = (Vector3.zero - viewer.position).normalized;
        bool hit = Physics2D.Raycast(viewer.position, toPlayer, 1000.0f);
        Color hitColor = hit ? Color.green : Color.red;
        playerOutline.GetComponent<SpriteRenderer>().color = hitColor;
        Debug.DrawLine(viewer.position, viewer.position + toPlayer * 1000.0f, hitColor);

        Vector2Int playerCell = Pathing.WorldToGrid(player.position, tiles);
        Vector2Int viewerCell = Pathing.WorldToGrid(viewer.position, tiles);
        grid[playerCell.y][playerCell.x].GetComponent<SpriteRenderer>().color = Color.magenta;
        grid[viewerCell.y][viewerCell.x].GetComponent<SpriteRenderer>().color = Color.cyan;
    }
}
