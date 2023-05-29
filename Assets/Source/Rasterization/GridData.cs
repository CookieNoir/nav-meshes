using UnityEngine;

public class GridData
{
    public Vector2 Origin { get; private set; }
    public Vector2 Step { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    private CellData[,] _cells;

    public GridData(Vector2 origin, Vector2 step, int width, int height, float startDepth)
    {
        Origin = origin;
        Step = step;
        Width = width;
        Height = height;

        _cells = new CellData[Width, Height];
        Vector2 cellOffset = Origin + (step / 2f);
        for (int x = 0; x < Width; ++x)
        {
            for (int y = 0; y < Height; ++y)
            {
                Vector2 position = new Vector2(cellOffset.x + x * Step.x, cellOffset.y + y * Step.y);
                _cells[x, y] = new CellData(position, startDepth, 1f);
            }
        }
    }

    public void TrySetData(int width, int height, float depth, float slope)
    {
        if (_cells[width, height].Depth < depth)
        {
            _cells[width, height].SetDepthAndSlope(depth, slope);
        }
    }

    public Vector2 GetPosition2D(int width, int height)
    {
        return _cells[width, height].Position;
    }

    public Vector2 GetPosition3D(int width, int height)
    {
        return _cells[width, height].GetPosition3D();
    }
}
