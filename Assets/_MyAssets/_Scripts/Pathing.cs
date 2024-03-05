using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// y = row, x = column
using Cell = UnityEngine.Vector2Int;

public static class Pathing
{
    public static List<Cell> FloodFill(Cell start, Cell goal, int[,] tiles, int stepCount)
    {
        int rows = tiles.GetLength(0);
        int cols = tiles.GetLength(1);
        bool[,] closedList = new bool[rows, cols];  // "visited"
        Queue<Cell> openList = new Queue<Cell>();   // "frontier"
        openList.Enqueue(start);

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
                // Impassible terrain solution 2: add impassible check directly into frontier condition
                // "Only add to frontier if tile is unvisited & tile is not impassible (stone is impassible)"
                if (!closedList[adjacent.y, adjacent.x]/* && tiles[adjacent.y, adjacent.x] != (int)TileType.STONE*/)
                {
                    openList.Enqueue(adjacent);
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

        if (bot) cells.Add(new Cell(cell.x, cell.y - 1));
        if (top) cells.Add(new Cell(cell.x, cell.y + 1));
        if (left) cells.Add(new Cell(cell.x - 1, cell.y));
        if (right) cells.Add(new Cell(cell.x + 1, cell.y));

        return cells;
    }
}

public class Node
{
    public Cell cell;   // current cell
    public Cell parent; // previous cell
    
    // Assign negative indices to indicate a cell is invalid
    public static Cell Invalid() { return new Cell { x = -1, y = -1 }; }
}