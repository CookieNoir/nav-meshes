using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Neogen : NavMeshGenerator
{
    [SerializeField, Min(0.001f)] private float _douglasPeuckerDistanceThreshold;
    //[SerializeField] private PolygonsDrawer _polygonsDrawer;
    public UnityEvent<List<ObstacleLayer>> OnReducedLayersReceived;
    public UnityEvent<List<ANavMGPolygon>> OnPolygonsReceived;

    public override CellAndPortalGraph Generate(GameObject[] gameObjects)
    {
        //ANavMG.PolygonsDrawer = _polygonsDrawer;  
        DateTime startTime = DateTime.Now;

        GridData gridData = Rasterizer.Rasterize(gameObjects, RasterCellSize);
        ObstacleLayer obstacleLayer = ObstacleDetector.DetectObstacles(gridData, ObstacleDetectorSettings);

        List<ObstacleLayer> reducedLayers = ReducedLayersDataReceiver.GetReducedLayers(obstacleLayer);

        List<NavMeshCell> cells = new List<NavMeshCell>();
        List<ANavMGPolygon> polygons = new List<ANavMGPolygon>();
        foreach (var layer in reducedLayers)
        {
            LayerRefiner.RefineLayer(layer);
            List<ANavMGPolygon> layerPolygons = ANavMGPolygonExtractor.GetPolygons(layer);
            polygons.AddRange(layerPolygons);
            foreach (ANavMGPolygon polygon in layerPolygons)
            {
                polygon.Simplify(_douglasPeuckerDistanceThreshold);
            }
            cells.AddRange(ANavMG.GetNavMesh(layerPolygons));
        }
        CellAndPortalGraph cellAndPortalGraph = new CellAndPortalGraph(cells);

        DateTime endTime = DateTime.Now;
        Debug.Log($"(NEOGEN) Time for generation - {(endTime - startTime).TotalSeconds}");

        OnReducedLayersReceived.Invoke(reducedLayers);
        OnPolygonsReceived.Invoke(polygons);

        return cellAndPortalGraph;
    }
}
