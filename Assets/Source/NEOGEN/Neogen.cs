using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Neogen : NavMeshGenerator
{
    [SerializeField, Min(0.001f)] private float _rasterCellSize;
    [SerializeField] private ObstacleDetectorSettings _obstacleDetectorSettings;
    [SerializeField, Min(0.001f)] private float _douglasPeuckerDistanceThreshold;
    public UnityEvent<List<Polygon>> OnPolygonsReceived;
    public UnityEvent<List<Polygon>> OnSimplifiedPolygonsReceived;

    public override CellAndPortalGraph Generate(GameObject[] gameObjects)
    {
        CellAndPortalGraph cellAndPortalGraph;
        DateTime startTime = DateTime.Now;
        {
            GridData gridData = Rasterizer.Rasterize(gameObjects, _rasterCellSize);
            ObstacleLayer obstacleLayer = ObstacleDetector.DetectObstacles(gridData, _obstacleDetectorSettings);

            List<ObstacleLayer> reducedLayers = ReducedLayersDataReceiver.GetReducedLayers(obstacleLayer);

            List<NavMeshCell> cells = new List<NavMeshCell>();
            foreach (var layer in reducedLayers)
            {
                LayerRefiner.RefineLayer(layer);
                List<Polygon> polygons = ANavMGPolygonExtractor.GetPolygons(layer);
                foreach (Polygon polygon in polygons)
                {
                    polygon.Simplify(_douglasPeuckerDistanceThreshold);
                }
                OnSimplifiedPolygonsReceived.Invoke(polygons);
                cells.AddRange(ANavMG.GetNavMesh(polygons));
            }
            cellAndPortalGraph = new CellAndPortalGraph(cells);
        }
        DateTime endTime = DateTime.Now;
        Debug.Log($"(NEOGEN) Time for generation - {(endTime - startTime).TotalSeconds}");
        // Callback
        return cellAndPortalGraph;
    }
}
