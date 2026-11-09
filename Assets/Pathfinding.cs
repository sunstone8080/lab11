using System.Collections.Generic;
using UnityEngine;

// "partial" so the grid-editing code (steps 2 and 3) can live in Pathfinding.GridEditing.cs
public partial class Pathfinding : MonoBehaviour
{
    // Editable in the Inspector, and changeable at runtime via SetStart / SetGoal / SetStartAndGoal
    [SerializeField] private Vector2Int startPosition = new Vector2Int(0, 1);
    [SerializeField] private Vector2Int goalPosition = new Vector2Int(4, 4);

    private readonly List<Vector2Int> path = new List<Vector2Int>();

    private readonly Vector2Int[] directions =
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    // grid[y, x]: 0 = walkable, 1 = obstacle
    private int[,] grid = new int[,]
    {
        { 0, 1, 0, 0, 0 },
        { 0, 1, 0, 1, 0 },
        { 0, 0, 0, 1, 0 },
        { 0, 1, 1, 1, 0 },
        { 0, 0, 0, 0, 0 }
    };

    public Vector2Int StartPosition => startPosition;
    public Vector2Int GoalPosition => goalPosition;
    public IReadOnlyList<Vector2Int> CurrentPath => path;

    private void Start()
    {
        Recalculate();
    }

    // ---------------------------------------------------------------
    // Step 1: set start / goal dynamically
    // ---------------------------------------------------------------

    /// <summary>Sets a new start cell. Returns false if the cell is invalid.</summary>
    public bool SetStart(Vector2Int position)
    {
        if (!IsWalkable(position))
        {
            Debug.LogWarning($"Cannot set start to {position}: out of bounds or an obstacle.");
            return false;
        }

        startPosition = position;
        Recalculate();
        return true;
    }

    /// <summary>Sets a new goal cell. Returns false if the cell is invalid.</summary>
    public bool SetGoal(Vector2Int position)
    {
        if (!IsWalkable(position))
        {
            Debug.LogWarning($"Cannot set goal to {position}: out of bounds or an obstacle.");
            return false;
        }

        goalPosition = position;
        Recalculate();
        return true;
    }

    /// <summary>Sets both cells at once (only one path recalculation).</summary>
    public bool SetStartAndGoal(Vector2Int newStart, Vector2Int newGoal)
    {
        if (!IsWalkable(newStart) || !IsWalkable(newGoal))
        {
            Debug.LogWarning($"Invalid start {newStart} or goal {newGoal}: out of bounds or an obstacle.");
            return false;
        }

        startPosition = newStart;
        goalPosition = newGoal;
        Recalculate();
        return true;
    }

    /// <summary>Re-runs BFS with the current grid, start and goal.</summary>
    public void Recalculate()
    {
        FindPath(startPosition, goalPosition);
    }

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    private bool IsInBounds(Vector2Int point)
    {
        return point.x >= 0 && point.x < grid.GetLength(1) &&
               point.y >= 0 && point.y < grid.GetLength(0);
    }

    private bool IsWalkable(Vector2Int point)
    {
        return IsInBounds(point) && grid[point.y, point.x] == 0;
    }

    // ---------------------------------------------------------------
    // BFS
    // ---------------------------------------------------------------

    private void FindPath(Vector2Int from, Vector2Int to)
    {
        path.Clear(); // otherwise old paths pile up when recalculating

        if (!IsWalkable(from) || !IsWalkable(to))
        {
            Debug.Log("Path not found (start or goal is blocked or out of bounds).");
            return;
        }

        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(from);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[from] = from;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current == to)
            {
                break;
            }

            foreach (Vector2Int direction in directions)
            {
                Vector2Int next = current + direction;

                if (IsWalkable(next) && !cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(to))
        {
            Debug.Log("Path not found.");
            return;
        }

        // Trace path from goal back to start
        Vector2Int step = to;
        while (step != from)
        {
            path.Add(step);
            step = cameFrom[step];
        }
        path.Add(from);
        path.Reverse();
    }

    // ---------------------------------------------------------------
    // Visualization
    // ---------------------------------------------------------------

    private void OnDrawGizmos()
    {
        float cellSize = 1f;
        Vector3 size = new Vector3(cellSize, 0.1f, cellSize);

        // Grid cells
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Gizmos.color = grid[y, x] == 1 ? Color.black : Color.white;
                Gizmos.DrawCube(new Vector3(x * cellSize, 0, y * cellSize), size);
            }
        }

        // Path
        Gizmos.color = Color.blue;
        foreach (var step in path)
        {
            Gizmos.DrawCube(new Vector3(step.x * cellSize, 0, step.y * cellSize), size);
        }

        // Start and goal
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(startPosition.x * cellSize, 0, startPosition.y * cellSize), size);

        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(goalPosition.x * cellSize, 0, goalPosition.y * cellSize), size);
    }
}
