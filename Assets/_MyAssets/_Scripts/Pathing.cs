using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

// y = row, x = column
using Cell = UnityEngine.Vector2Int;
public delegate float DistanceFunction(Cell cell1, Cell cell2);
public delegate float HeuristicFunction(int type);

// let c = current node
// let n = next node (neighbour)
// let e = end node (goal)
// let g(n) = g(n - 1) + Distance(c, n)
// let h(n) = Distance(n, e) + Terrain(n)

// g(n) is needed so that we can distinguish between adjacent & diagonal
// g(n) also allows us to improve upon existing paths ie:
// if we move [down, left, left] g = 1 + 1 + 1 = 3
// if we move [down-left, left] g = 1.4 + 1 = 2.4
// Hence, we must update g(n) to be 2.4!

// h(n) is needed so that we move towards the goal
// h(n) also includes additional information such as terrain cost
// This allows us to decentivise costly tiles from being explored

// A* minimizes f(n) = g(n) + h(n)
public static class Pathing
{
    public static List<Cell> Find(Cell start, Cell end, int[,] tiles,
        DistanceFunction distance, HeuristicFunction heuristic)
    {
        int rows = tiles.GetLength(0);  // Dimension 0 (height) = rows
        int cols = tiles.GetLength(1);  // Dimension 1 (width)  = columns

        // Score-based frontier
        PriorityQueue<Cell, float> openList = new PriorityQueue<Cell, float>();

        // List of explored nodes (true if explored, indices map 1:1 to tiles)
        bool[,] closedList = new bool[rows, cols];

        // Create 1:1 grid-based weighted graph
        // ("connection" = node.parent, "weight" = node.f)
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

        // Explore the frontier, beginning with the starting cell
        openList.Enqueue(start, 0.0f);
        while ((openList.Count > 0))
        {
            // Fetch the cell at the front of the queue (best f-score)
            Cell current = openList.Dequeue();

            // End exploration if we've reached our destination!
            if (current == end) break;

            // Otherwise, mark as explored and evaluate neighbours
            closedList[current.y, current.x] = true;

            float gNew, hNew;
            foreach (Cell neighbour in Neighbours(current, rows, cols))
            {
                // Don't re-explore neighbours (otherwise infinite loop)
                if (closedList[neighbour.y, neighbour.x]) continue;

                // Compute neighbour's f-score; f(n) = g(n) + h(n)
                gNew = graph[current.y, current.x].g;
                gNew += distance(current, neighbour);
                hNew = distance(neighbour, end);
                hNew += heuristic(tiles[neighbour.y, neighbour.x]);

                // If the neighbour is unexplored or has the best score so far:
                Node node = graph[neighbour.y, neighbour.x];
                if (node.F() <= Mathf.Epsilon || node.F() > (gNew + hNew))
                {
                    // Update graph connections
                    node.g = gNew;
                    node.h = hNew;
                    node.parent = current;
                    graph[neighbour.y, neighbour.x] = node;

                    // Add the node to the frontier (based on f-score)
                    openList.Enqueue(node.cell, node.F());
                }
            }
        }

        // Form a path by walking the list (stop when the parent is invalid)
        List<Cell> path = new List<Cell>();
        {
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
        }
        return path;
    }

    public static List<Cell> Neighbours(Cell cell, int rows, int cols)
    {
        List<Cell> neighbours = new List<Cell>();

        // Bounds
        bool bot = cell.y - 1 >= 0;
        bool top = cell.y + 1 < rows;
        bool left = cell.x - 1 >= 0;
        bool right = cell.x + 1 < cols;

        // Adjacent
        if (bot) neighbours.Add(new Cell(cell.x, cell.y - 1));
        if (top) neighbours.Add(new Cell(cell.x, cell.y + 1));
        if (left) neighbours.Add(new Cell(cell.x - 1, cell.y));
        if (right) neighbours.Add(new Cell(cell.x + 1, cell.y));

        // Diagonal
        if (bot && left) neighbours.Add(new Cell(cell.x - 1, cell.y - 1));
        if (top && left) neighbours.Add(new Cell(cell.x - 1, cell.y + 1));
        if (bot && right) neighbours.Add(new Cell(cell.x + 1, cell.y - 1));
        if (top && right) neighbours.Add(new Cell(cell.x + 1, cell.y + 1));

        return neighbours;
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

    public enum DistanceType
    {
        MANHATTAN,
        EUCLIDEAN
    }

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
