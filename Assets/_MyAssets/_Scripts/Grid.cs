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
        float xStart = -colCount / 2.0f + 0.5f;    // left (-x)
        float yStart = -rowCount / 2.0f + 0.5f;    // bottom (-y)
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

                // We know our grid positions are x = [-10, 10], y = [-5, 5]
                // We know colors are represented as RGBA values between 0 and 1
                // Hence, we can convert our positions to the range [0, 1] to render them as colours!
                position = new Vector2(position.x / 10.0f, position.y / 5.0f);
                position *= 0.5f;
                position += new Vector2(0.5f, 0.5f);
                tile.GetComponent<SpriteRenderer>().color = new Color(position.x, position.y, 0.0f, 1.0f);
            }
        }
    }

    void Update()
    {
        ColorGrid();

        // 1. Convert screen-space to world-space
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 2. Convert world-space to grid-space ("quantization/localization")
        //Vector2Int cell = 
        Debug.Log(mouse);
    }
}
