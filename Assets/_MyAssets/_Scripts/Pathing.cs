using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// y = row, x = column
using Cell = UnityEngine.Vector2Int;

public static class Pathing
{
    public static List<Cell> Find(Cell start, Cell end, int[,] tiles, int stepCount)
    {
        int rows = tiles.GetLength(0);
        int cols = tiles.GetLength(1);
        bool[,] closedList = new bool[rows, cols];
        Queue<Cell> openList = new Queue<Cell>();
        openList.Enqueue(start);

        // Create a 1:1 grid-based graph to store connections along path
        Node[,] graph = new Node[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                graph[i, j] = new Node();
                graph[i, j].cell = new Cell(j, i);
                graph[i, j].parent = Node.Invalid();
            }
        }

        // Explore the frontier, add each unique cell to the list
        bool found = false;
        for (int i = 0; i < stepCount; i++)
        {
            // Stop if there's nothing left to explore
            if (openList.Count == 0) break;

            // Get cell at the top of the frontier
            Cell current = openList.Dequeue();

            // Exit if we've found the goal!
            if (current == end)
            {
                found = true;
                break;
            }

            // Prevent re-exploration of same cells (otherwise infinite loop)
            closedList[current.y, current.x] = true;

            // Search adjacent cells, add them to the frontier if they haven't been explored
            foreach (Cell adjacent in Adjacents(current, rows, cols))
            {
                if (!closedList[adjacent.y, adjacent.x] &&
                    tiles[adjacent.y, adjacent.x] != 3)
                {
                    openList.Enqueue(adjacent);
                    graph[adjacent.y, adjacent.x].parent = current;
                }
            }
        }
        
        // Walk list from end to start
        if (found) {
            List<Cell> path = new List<Cell>();
            Cell current = end;
            Cell next = graph[current.y, current.x].parent;
            while (next != Node.Invalid())
            {
                path.Add(current);
                current = next;
                next = graph[current.y, current.x].parent;
            }
            path.Add(start);
            path.Reverse();
            return path;
        }

        // Flood fill if no solution for visualization
        return FloodFill(start, tiles, stepCount);
    }

    public static List<Cell> FloodFill(Cell start, int[,] tiles, int stepCount)
    {
        int rows = tiles.GetLength(0);
        int cols = tiles.GetLength(1);
        bool[,] closedList = new bool[rows, cols];
        Queue<Cell> openList = new Queue<Cell>();
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

public class Node
{
    public Cell cell;
    public Cell parent;

    public static Cell Invalid()
    {
        return new Cell(-1, -1);
    }

    public float g, h;
    public float F()
    {
        return g + h;
    }
}