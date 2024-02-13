using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;
    List<List<GameObject>> grid = new List<List<GameObject>>();
    int rowCount = 10;      // vertical tile count
    int colCount = 20;      // horizontal tile count
    //int tileWidth = 1;
    //int tileHeight = 1;

    void Start()
    {
        //float xStart = -colCount / 2.0f + 0.5f;    // left (-x)
        //float yStart = -rowCount / 2.0f + 0.5f;    // bottom (-y)
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
    }

    void ColorGrid()
    {
        // Make increasingly red as we move right, make increasingly green as we move up
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                GameObject tile = grid[row][col];
                Vector2 position = tile.transform.position;

                // Normalize position to render as color
                position = new Vector2(position.x / 20.0f, position.y / 10.0f);
                tile.GetComponent<SpriteRenderer>().color = new Color(position.x, position.y, 0.0f, 1.0f);
            }
        }

        // World to grid (quantization) test
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int cell = WorldToGrid(mouse);
        grid[cell.y][cell.x].GetComponent<SpriteRenderer>().color = Color.magenta;

        // Grid to world (localization) test
        //GameObject selected = Instantiate(tilePrefab);
        //selected.transform.position = GridToWorld(cell);
        //selected.GetComponent<SpriteRenderer>().color = Color.magenta;
    }

    Vector2Int WorldToGrid(Vector2 position)
    {
        Vector2Int cell = new Vector2Int((int)position.x, (int)position.y);
        cell.x = Mathf.Clamp(cell.x, 0, colCount - 1);
        cell.y = Mathf.Clamp(cell.y, 0, rowCount - 1);
        return cell;
    }

    Vector2 GridToWorld(Vector2Int cell)
    {
        cell.x = Mathf.Clamp(cell.x, 0, colCount - 1);
        cell.y = Mathf.Clamp(cell.y, 0, rowCount - 1);
        return new Vector2(cell.x + 0.5f, cell.y + 0.5f);
    }

    void Update()
    {
        ColorGrid();
    }
}
