using UnityEngine;

public class ObstacleLayer
{
    public Vector2 Origin { get; private set; }
    public Vector2 Step { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public bool[,] IsObstacle { get; private set; }
    public Vector3[,] Positions { get; private set; }

    public ObstacleLayer(Vector2 origin, Vector2 step, bool[,] isObstacle, Vector3[,] positions)
    {
        Origin = origin;
        Step = step;
        Width = isObstacle.GetLength(0);
        Height = isObstacle.GetLength(1);
        IsObstacle = isObstacle;
        Positions = positions;
    }

    public void SetBounds()
    {
        for (int x = 0; x < Width; ++x)
        {
            IsObstacle[x, 0] = true;
            IsObstacle[x, Height - 1] = true;
        }
        for (int y = 0; y < Height; ++y)
        {
            IsObstacle[0, y] = true;
            IsObstacle[Width - 1, y] = true;
        }
    }
}
