using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// y = row, x = column
using Cell = UnityEngine.Vector2Int;

public static class Pathing
{
    public static List<Cell> FloodFill(Cell start, int[,] tiles, int stepCount)
    {
        int rows = tiles.GetLength(0);
        int cols = tiles.GetLength(1);
        bool[,] closedList = new bool[rows, cols];  // "visited"
        Queue<Cell> openList = new Queue<Cell>();   // "frontier"
        openList.Enqueue(start);

        // Explore the frontier, add each unique cell to the list
        List<Cell> cells = new List<Cell>();
        for (int i = 0; i < stepCount; i++)
        {
            // Stop if there's nothing left to explore
            if (openList.Count == 0) break;

            // Get cell at the top of the frontier
            Cell current = openList.Dequeue();

            // Prevent re-exploration of same cells (otherwise infinite loop)
            closedList[current.y, current.x] = true;

            // Add top cell to results list
            cells.Add(current);

            // Search adjacent cells, add them to the frontier if they haven't been explored
            foreach (Cell adjacent in Adjacents(current, rows, cols))
            {
                if (!closedList[adjacent.y, adjacent.x])
                    openList.Enqueue(adjacent);
            }
        }

        return cells;
    }

    public static List<Cell> Adjacents(Cell cell, int rows, int cols)
    {
        List<Cell> cells = new List<Cell>();

        bool bot = cell.y - 1 >= 0;
        bool top = cell.y + 1 < rows;
        bool left = cell.x - 1 >= 0;
        bool right = cell.x + 1 < cols;

        if (bot) cells.Add(new Cell(cell.x, cell.y - 1));
        if (top) cells.Add(new Cell(cell.x, cell.y + 1));
        if (left) cells.Add(new Cell(cell.x - 1, cell.y));
        if (right) cells.Add(new Cell(cell.x + 1, cell.y));

        return cells;
    }
}