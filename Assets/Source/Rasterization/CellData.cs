using UnityEngine;

public class CellData
{
    public Vector2 Position { get; private set; }
    public float Depth { get; private set; }
    public float Slope { get; private set; } // in range [-1, 1]

    public CellData(Vector2 position, float depth, float slope)
    {
        Position = position;
        SetDepthAndSlope(depth, slope);
    }

    public void SetDepthAndSlope(float depth, float slope)
    {
        Depth = depth;
        Slope = slope;
    }

    public Vector3 GetPosition3D()
    {
        return new Vector3(Position.x, Depth, Position.y);
    }
}
