using UnityEngine;


public partial class Pathfinding
{
 
    public void GenerateRandomGrid(int width, int height, float obstacleProbability)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.LogWarning("GenerateRandomGrid: width and height must be greater than 0.");
            return;
        }

        obstacleProbability = Mathf.Clamp01(obstacleProbability);

        int[,] newGrid = new int[height, width]; 
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                newGrid[y, x] = Random.value < obstacleProbability ? 1 : 0;
            }
        }

        grid = newGrid;

     
        startPosition = ClampToGrid(startPosition);
        goalPosition = ClampToGrid(goalPosition);

       
        grid[startPosition.y, startPosition.x] = 0;
        grid[goalPosition.y, goalPosition.x] = 0;

        Recalculate();
    }

    public bool AddObstacle(Vector2Int position)
    {
        if (!IsInBounds(position))
        {
            Debug.LogWarning($"AddObstacle: {position} is out of bounds.");
            return false;
        }

        if (position == startPosition || position == goalPosition)
        {
            Debug.LogWarning($"AddObstacle: {position} is the start or goal cell.");
            return false;
        }

        if (grid[position.y, position.x] == 1)
        {
            return false; 
        }

        grid[position.y, position.x] = 1;
        Recalculate();
        return true;
    }

    private Vector2Int ClampToGrid(Vector2Int point)
    {
        return new Vector2Int(
            Mathf.Clamp(point.x, 0, grid.GetLength(1) - 1),
            Mathf.Clamp(point.y, 0, grid.GetLength(0) - 1));
    }

    [ContextMenu("Test: Random 10x10 grid (30% obstacles)")]
    private void TestRandomGrid() => GenerateRandomGrid(10, 10, 0.3f);

    [ContextMenu("Test: Add obstacle at (2,2)")]
    private void TestAddObstacle() => AddObstacle(new Vector2Int(2, 2));
}
