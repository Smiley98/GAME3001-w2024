using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;
    List<List<GameObject>> grid = new List<List<GameObject>>();
    int rowCount = 10;      // vertical tile count
    int colCount = 20;      // horizontal tile count

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

                // We know colors are represented as RGBA values between 0 and 1
                // Hence, we can convert our positions to the range [0, 1] to render them as colours!
                position = new Vector2(position.x / colCount, position.y / rowCount);
                tile.GetComponent<SpriteRenderer>().color = new Color(position.x, position.y, 0.0f, 1.0f);
            }
        }

        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int cell = WorldToGrid(mouse);
        grid[cell.y][cell.x].GetComponent<SpriteRenderer>().color = Color.magenta;
    }

    // Quantization
    Vector2Int WorldToGrid(Vector2 position)
    {
        Vector2Int cell = new Vector2Int((int)position.x, (int)position.y);
        cell.x = Mathf.Clamp(cell.x, 0, colCount - 1);
        cell.y = Mathf.Clamp(cell.y, 0, rowCount - 1);
        return cell;
    }

    void Update()
    {
        ColorGrid();
    }
}
