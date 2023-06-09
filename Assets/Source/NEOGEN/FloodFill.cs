using System.Collections.Generic;
using UnityEngine;

public static class FloodFill
{
    public struct Properties
    {
        public int[,] LayerMasks;
        public bool[,] IsObstacle;
        public ReducedLayerBounds ReducedLayerBounds;
        public int TargetColor;

        public Properties(int[,] layerMasks, bool[,] isObstacle, ReducedLayerBounds reducedLayerBounds, int targetColor)
        {
            LayerMasks = layerMasks;
            IsObstacle = isObstacle;
            ReducedLayerBounds = reducedLayerBounds;
            TargetColor = targetColor;
        }
    }

    private static readonly Vector2Int[] _delta = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };

    public static void FloodFillDFS(Properties properties, Vector2Int point)
    {
        properties.LayerMasks[point.x, point.y] = properties.TargetColor;
        properties.ReducedLayerBounds.UpdateBounds(point.x, point.y);

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(point);

        while (stack.TryPop(out Vector2Int item))
        {
            properties.LayerMasks[item.x, item.y] = properties.TargetColor;
            properties.ReducedLayerBounds.UpdateBounds(item.x, item.y);
            for (int i = 0; i < 4; ++i)
            {
                Vector2Int nextPoint = item + _delta[i];
                if (nextPoint.x >= 0 && nextPoint.x < properties.LayerMasks.GetLength(0) &&
                    nextPoint.y >= 0 && nextPoint.y < properties.LayerMasks.GetLength(1) &&
                    properties.LayerMasks[nextPoint.x, nextPoint.y] == -1)
                {
                    stack.Push(nextPoint);
                }
            }
        }
    }
}
