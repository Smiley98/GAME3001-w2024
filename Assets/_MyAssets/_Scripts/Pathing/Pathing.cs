using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

// y = row, x = column
using Cell = UnityEngine.Vector2Int;

public static class Pathing
{
    public static List<Cell> FloodFill(Cell start, Cell goal, int[,] tiles, int stepCount)
    {
        int rows = tiles.GetLength(0);
        int cols = tiles.GetLength(1);
        bool[,] closedList = new bool[rows, cols];  // "visited"
        PriorityQueue<Cell, float> openList = new PriorityQueue<Cell, float>();
        openList.Enqueue(start, 0.0f);
        // The closer to 0, the higher the priority

        Node[,] nodes = new Node[rows, cols];
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                nodes[row, col] = new Node();
                nodes[row, col].cell = new Cell { y = row, x = col };   // Position cell on grid
                nodes[row, col].parent = Node.Invalid();                // Indicate cell has no parent by default

                // Impassible terrain solution 1: pre-mark all impassible tiles as visited
                closedList[row, col] = tiles[row, col] == (int)TileType.STONE;
            }
        }

        bool found = false;

        // Explore the frontier, add each unique cell to the list
        List<Cell> cells = new List<Cell>();
        for (int i = 0; i < stepCount; i++)
        {
            // Stop if there's nothing left to explore
            if (openList.Count == 0) break;

            // Get cell at the top of the frontier
            Cell current = openList.Dequeue();

            // Stop exploring if we've reached our goal!
            if (current == goal)
            {
                found = true;
                break;
            }

            // Prevent re-exploration of same cells (otherwise infinite loop)
            closedList[current.y, current.x] = true;

            // Add top cell to results list
            cells.Add(current);

            // Search adjacent cells, add them to the frontier if they haven't been explored
            foreach (Cell adjacent in Adjacents(current, rows, cols))
            {
                if (!closedList[adjacent.y, adjacent.x])
                {
                    // For A*, you need to make your priority based on f(x) = g(x) + h(x)
                    // We've coded h(x) as manhattan distance from adjacent to goal.
                    // You may also want to associate terrain cost with h(x) [lab 4's homework]
                    // g(x) should be distance from current to adjacent.
                    openList.Enqueue(adjacent, Manhattan(adjacent, goal));
                    nodes[adjacent.y, adjacent.x].parent = current;
                }
            }
        }

        // Return the path found by retracing our steps if there's a solution
        if (found)
        {
            List<Cell> path = new List<Cell>();
            Cell current = goal;
            Cell next = nodes[current.y, current.x].parent;
            while (next != Node.Invalid())
            {
                path.Add(current);
                current = next;
                next = nodes[next.y, next.x].parent;
            }
            path.Reverse();
            return path;
        }

        // Otherwise return flood fill for visualization
        return cells;
    }

    public static List<Cell> Adjacents(Cell cell, int rows, int cols)
    {
        List<Cell> cells = new List<Cell>();

        bool bot = cell.y - 1 >= 0;
        bool top = cell.y + 1 < rows;
        bool left = cell.x - 1 >= 0;
        bool right = cell.x + 1 < cols;

        if (top) cells.Add(new Cell(cell.x, cell.y + 1));
        if (bot) cells.Add(new Cell(cell.x, cell.y - 1));
        if (left) cells.Add(new Cell(cell.x - 1, cell.y));
        if (right) cells.Add(new Cell(cell.x + 1, cell.y));

        return cells;
    }

    // Prefers adjacent
    public static float Manhattan(Cell cell1, Cell cell2)
    {
        return Mathf.Abs(cell1.x - cell2.x) + Mathf.Abs(cell1.y - cell2.y);
    }

    // Prefers diagonal
    public static float Euclidean(Cell cell1, Cell cell2)
    {
        return Cell.Distance(cell1, cell2);
    }

    // Quantization
    public static Cell WorldToGrid(Vector3 position, int[,] tiles)
    {
        int rows = tiles.GetLength(0);
        int cols = tiles.GetLength(1);
        Cell cell = new Cell((int)position.x, (rows - 1) - (int)position.y);
        cell.x = Mathf.Clamp(cell.x, 0, cols - 1);
        cell.y = Mathf.Clamp(cell.y, 0, rows - 1);
        return cell;
    }

    // Localization
    public static Vector3 GridToWorld(Cell cell, int[,] tiles)
    {
        int rows = tiles.GetLength(0);
        int cols = tiles.GetLength(1);
        cell.x = Mathf.Clamp(cell.x, 0, cols - 1);
        cell.y = Mathf.Clamp((rows - 1) - cell.y, 0, rows - 1);
        return new Vector3(cell.x + 0.5f, cell.y + 0.5f);
    }
}

public class Node
{
    // For A*, include g & h values
    public Cell cell;   // current cell
    public Cell parent; // previous cell

    // Assign negative indices to indicate a cell is invalid
    public static Cell Invalid() { return new Cell { x = -1, y = -1 }; }
}