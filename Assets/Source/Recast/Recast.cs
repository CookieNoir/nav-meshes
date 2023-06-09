using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Recast : NavMeshGenerator
{
    [SerializeField, Min(0.001f)] private float _douglasPeuckerDistanceThreshold;
    public UnityEvent<ObstacleLayer, int[,]> OnDistanceTransformReceived;
    public UnityEvent<ObstacleLayer, int[,]> OnWatershedPartitionReceived;
    public UnityEvent<ObstacleLayer, bool[,]> OnExtendedObstacleLayerReceived;
    public UnityEvent<List<RecastPolygon>> OnPolygonsReceived;

    public override CellAndPortalGraph Generate(GameObject[] gameObjects)
    {
        DateTime startTime = DateTime.Now;

        GridData gridData = Rasterizer.Rasterize(gameObjects, RasterCellSize);
        ObstacleLayer obstacleLayer = ObstacleDetector.DetectObstacles(gridData, ObstacleDetectorSettings);
        LayerRefiner.RefineLayer(obstacleLayer);
        int[,] distanceTransform = DistanceTransform.GetDistancesManhattan(obstacleLayer);
        int[,] watershedPartition = Watershed.GetPartition(distanceTransform);

        bool[,] isObstacle;
        List<RecastPolygon> polygons = RecastPolygonExtractor.GetPolygons(obstacleLayer, watershedPartition, out isObstacle);
        List<NavMeshCell> cells = new List<NavMeshCell>();
        foreach (RecastPolygon polygon in polygons)
        {
            polygon.Simplify(_douglasPeuckerDistanceThreshold);
            cells.AddRange(EarClippingTriangulation.GetTriangles(polygon));
        }
        CellAndPortalGraph cellAndPortalGraph = new CellAndPortalGraph(cells);

        DateTime endTime = DateTime.Now;
        Debug.Log($"(Recast) Time for generation - {(endTime - startTime).TotalSeconds}");

        OnDistanceTransformReceived.Invoke(obstacleLayer, distanceTransform);
        OnWatershedPartitionReceived.Invoke(obstacleLayer, watershedPartition);
        OnExtendedObstacleLayerReceived.Invoke(obstacleLayer, isObstacle);
        OnPolygonsReceived.Invoke(polygons);
        return cellAndPortalGraph;
    }
}
