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
    [SerializeField] GameObject obstacle;

    [SerializeField] [Range(0.0f, 25.0f)] float distance;

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

    bool IsVisbile(Vector3 viewer, Vector3 target, GameObject obstacle, float distance)
    {
        // AB = B - A
        Vector3 direction = (target - viewer).normalized;
        RaycastHit2D hit = Physics2D.Raycast(viewer, direction, distance);
        bool obstacleHit = hit.collider && hit.collider.CompareTag(obstacle.tag);
        return !obstacleHit;
        //return Physics2D.Raycast(viewer, direction, distance);
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
                GameObject tile = grid[row][col];
                bool visible = IsVisbile(tile.transform.position, player.position, obstacle, distance);
                tile.GetComponent<SpriteRenderer>().color = visible ? Color.green : Color.red;
            }
        }

        // Colors ray & player green if viewer can see player, otherwise red
        //Vector3 toPlayer = (player.position - viewer.position).normalized;
        //RaycastHit2D hit = Physics2D.Raycast(viewer.position, toPlayer, 1000.0f);
        //bool playerHit = hit && hit.collider.CompareTag("Player");
        //Color hitColor = playerHit ? Color.green : Color.red;
        //playerOutline.GetComponent<SpriteRenderer>().color = hitColor;
        //Debug.DrawLine(viewer.position, viewer.position + toPlayer * 1000.0f, hitColor);

        // Figure out the cells that the player & viewer are in
        //Vector2Int playerCell = Pathing.WorldToGrid(player.position, tiles);
        //Vector2Int viewerCell = Pathing.WorldToGrid(viewer.position, tiles);
        //grid[playerCell.y][playerCell.x].GetComponent<SpriteRenderer>().color = Color.magenta;
        //grid[viewerCell.y][viewerCell.x].GetComponent<SpriteRenderer>().color = Color.cyan;

        // Homework: colour tiles green if the player is visible from them, otherwise red
        // Hint: You need to loop through all tiles (similar to above where each tile is coloured white),
        // and then tailor the above line of sight test to your tiles loop!
    }
}
