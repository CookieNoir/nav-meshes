using System;
using System.Collections.Generic;
using UnityEngine;

public class QuadrilateralGridNavMeshGenerator : NavMeshGenerator
{
    [SerializeField] private HexagonMapBuilder _hexagonMapBuilder;
    [SerializeField, Min(0.001f)] private float _rasterCellSize;
    [SerializeField] private ObstacleDetectorSettings _obstacleDetectorSettings;
    [SerializeField, Min(0.05f)] private float _hexagonSize;

    public override CellAndPortalGraph Generate(GameObject[] gameObjects)
    {
        CellAndPortalGraph cellAndPortalGraph;
        DateTime startTime = DateTime.Now;
        {
            GridData gridData = Rasterizer.Rasterize(gameObjects, _rasterCellSize);

            ObstacleLayer obstacleLayer = ObstacleDetector.DetectObstacles(gridData, _obstacleDetectorSettings);

            Vector3 centerVector3 = (obstacleLayer.Positions[0, 0] + obstacleLayer.Positions[obstacleLayer.Width - 1, obstacleLayer.Height - 1]) / 2f;
            Vector3 sizeVector3 = obstacleLayer.Positions[0, 0] - obstacleLayer.Positions[obstacleLayer.Width - 1, obstacleLayer.Height - 1];
            Vector2 center = new Vector2(centerVector3.x, centerVector3.z);
            Vector2 size = new Vector2(Mathf.Abs(sizeVector3.x), Mathf.Abs(sizeVector3.z));
            List<HexagonModel> hexagons = _hexagonMapBuilder.BuildMap(center, size, _hexagonSize);

            List<NavMeshCell> cells = CellsFromHexagonsReceiver.GetCells(hexagons, obstacleLayer);
            cellAndPortalGraph = new CellAndPortalGraph(cells);
        }
        DateTime endTime = DateTime.Now;
        Debug.Log($"(Quadrilateral Grid Nav Mesh) Time for generation - {(endTime - startTime).TotalSeconds}");
        // Callback
        return cellAndPortalGraph;
    }
}
