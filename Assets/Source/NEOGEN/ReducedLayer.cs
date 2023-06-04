using UnityEngine;

public class ReducedLayer
{
    public int OffsetX { get; private set; }
    public int OffsetY { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public bool[,] IsObstacle { get; private set; }

    public ReducedLayer(bool[,] isObstacle, int offsetX, int offsetY)
    {
        OffsetX = offsetX;
        OffsetY = offsetY;
        Width = isObstacle.GetLength(0);
        Height = isObstacle.GetLength(1);
        IsObstacle = isObstacle;
    }
}
