using System.Collections.Generic;
using UnityEngine;

public static class ReducedLayersDataReceiver
{
    public static ReducedLayersData GetReducedLayers(ObstacleLayer obstacleLayer)
    {
        int[,] layerMasks = new int[obstacleLayer.Width, obstacleLayer.Height];
        for (int x = 0; x < obstacleLayer.Width; ++x)
        {
            for (int y = 0; y < obstacleLayer.Height; ++y)
            {
                if (obstacleLayer.IsObstacle[x, y]) { layerMasks[x, y] = -2; }
                else { layerMasks[x, y] = -1; }
            }
        }

        int targetColor = 0;
        List<ReducedLayer> reducedLayers = new List<ReducedLayer>();
        for (int x = 0; x < obstacleLayer.Width; ++x)
        {
            for (int y = 0; y < obstacleLayer.Height; ++y)
            {
                if (layerMasks[x, y] == -1)
                {
                    ReducedLayerBounds reducedLayerBounds = new ReducedLayerBounds(x, y);
                    FloodFill.Properties properties = new FloodFill.Properties(layerMasks, obstacleLayer.IsObstacle,
                        reducedLayerBounds, targetColor);
                    FloodFill.FloodFillDFS(properties, new Vector2Int(x, y));
                    reducedLayers.Add(CreateReducedLayer(obstacleLayer, reducedLayerBounds, layerMasks, targetColor));
                    targetColor++;
                }
            }
        }
        return new ReducedLayersData(obstacleLayer.Positions, reducedLayers);
    }

    private static ReducedLayer CreateReducedLayer(ObstacleLayer obstacleLayer, ReducedLayerBounds reducedLayerBounds, 
        int[,] layerMasks, int targetColor)
    {
        int reducedWidth = reducedLayerBounds.MaxWidth - reducedLayerBounds.MinWidth + 1;
        int reducedHeight = reducedLayerBounds.MaxHeight - reducedLayerBounds.MinHeight + 1;
        bool[,] isObstacle = new bool[reducedWidth, reducedHeight];
        for (int x = 0; x < reducedWidth; ++x)
        {
            for (int y = 0; y < reducedHeight; ++y)
            {
                int globalWidth = x + reducedLayerBounds.MinWidth;
                int globalHeight = y + reducedLayerBounds.MinHeight;
                isObstacle[x, y] = layerMasks[globalWidth, globalHeight] != targetColor;
            }
        }
        return new ReducedLayer(isObstacle, reducedLayerBounds.MinWidth, reducedLayerBounds.MinHeight);
    }
}
