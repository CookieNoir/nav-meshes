using System;
using System.Collections.Generic;
using UnityEngine;

public class Neogen : NavMeshGenerator
{
    [SerializeField, Min(0.001f)] private float _rasterCellSize;
    [SerializeField] private ObstacleDetectorSettings _obstacleDetectorSettings;

    public override CellAndPortalGraph Generate(GameObject[] gameObjects)
    {
        CellAndPortalGraph cellAndPortalGraph;
        DateTime startTime = DateTime.Now;
        {
            GridData gridData = Rasterizer.Rasterize(gameObjects, _rasterCellSize);
            ObstacleLayer obstacleLayer = ObstacleDetector.DetectObstacles(gridData, _obstacleDetectorSettings);

            ReducedLayersData reducedLayersData = ReducedLayersDataReceiver.GetReducedLayers(obstacleLayer);

            cellAndPortalGraph = new CellAndPortalGraph(null);
        }
        DateTime endTime = DateTime.Now;
        Debug.Log($"(Quadrilateral Grid Nav Mesh) Time for generation - {(endTime - startTime).TotalSeconds}");
        // Callback
        return cellAndPortalGraph;
    }
}
