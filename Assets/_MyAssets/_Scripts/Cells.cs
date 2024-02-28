using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

// y = row, x = column
using Cell = UnityEngine.Vector2Int;
public class CellGrid
{
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
        //if (bot && left) neighbours.Add(new Cell(cell.x - 1, cell.y - 1));
        //if (top && left) neighbours.Add(new Cell(cell.x - 1, cell.y + 1));
        //if (bot && right) neighbours.Add(new Cell(cell.x + 1, cell.y - 1));
        //if (top && right) neighbours.Add(new Cell(cell.x + 1, cell.y + 1));

        return neighbours;
    }

    // Adjacent
    public static float Manhattan(Cell cell1, Cell cell2)
    {
        return Mathf.Abs(cell1.x - cell2.x) + Mathf.Abs(cell1.y - cell2.y);
    }

    // Diagonal
    public static float Euclidean(Cell cell1, Cell cell2)
    {
        return Cell.Distance(cell1, cell2);
    }
}

// let c = current node (top of the queue / head of the frontier)
// let n = next node (neighbour)
// let e = end node (goal)
// let g(n) = Distance(c, n)
// let h(n) = Distance(n, e) + Terrain(n)

// g(n) is needed so that we can distinguish between adjacent & diagonal
// h(n) is needed so that we move towards the goal (shortest distance)
// h(n) also includes additional heuristic evaluations such as terrain cost

// A* minimizes f(n) = g(n) + h(n)
public class PathingGrid
{
    public delegate float Distance(Cell cell1, Cell cell2);
    public delegate float Heuristic(int type);

    public Distance distance;
    public Heuristic heuristic;

    public List<Cell> FindPath(Cell start, Cell end, int[,] tiles)
    {
        int rows = tiles.GetLength(0);
        int cols = tiles.GetLength(1);
        Node[,] graph = new Node[rows, cols];
        bool[,] closedList = new bool[rows, cols];
        PriorityQueue<Node, float> openList = new PriorityQueue<Node, float>();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                graph[i, j] = new Node();
            }
        }

        graph[start.y, start.x].parent = start;
        openList.Enqueue(new Node { cell = start }, 0.0f);

        // Explore all nodes
        while ((openList.Count > 0))
        {
            Cell current = openList.Dequeue().cell;
            if (current == end) break;

            float gNew, hNew;
            closedList[current.y, current.x] = true;
            foreach (Cell neighbour in CellGrid.Neighbours(current, rows, cols))
            {
                if (closedList[neighbour.y, neighbour.x]) continue;

                gNew = distance(current, neighbour);
                hNew = heuristic(tiles[neighbour.y, neighbour.x]);

                // if neighbour is unexplored or has best score so far:
                if (graph[neighbour.y, neighbour.x].F() <= Mathf.Epsilon ||
                    graph[neighbour.y, neighbour.x].F() > (gNew + hNew))
                {
                    Node node = new Node();
                    node.cell = neighbour;
                    node.parent = current;
                    node.g = gNew;
                    node.h = hNew;

                    // Add node to frontier and update connection in graph
                    openList.Enqueue(node, node.F());
                    graph[neighbour.y, neighbour.x] = node;
                }
            }
        }

        // Form a path by walking the list
        List<Cell> path = new List<Cell>();
        {
            Cell current = end;
            Node node = graph[current.y, current.x];
            while (node.parent != current)
            {
                path.Add(current);
                current = node.parent;
                node = graph[current.y, current.x];
            }
            path.Reverse();
        }
        return path;
    }
}

public class Node
{
    public Cell cell = new Cell(-1, -1);
    public Cell parent = new Cell(-1, -1);

    public float g, h;
    public float F()
    {
        return g + h;
    }
}
