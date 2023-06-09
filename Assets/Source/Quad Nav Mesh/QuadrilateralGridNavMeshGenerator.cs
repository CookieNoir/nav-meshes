using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuadrilateralGridNavMeshGenerator : NavMeshGenerator
{
    [SerializeField] private HexagonMapBuilder _hexagonMapBuilder;
    [SerializeField, Min(0.05f)] private float _hexagonSize;
    [SerializeField] private bool _isGeneratingHexagons;
    public UnityEvent<List<HexagonModel>> OnHexagonsReceived;
    private List<HexagonModel> _lastGeneratedHexagons;

    public override CellAndPortalGraph Generate(GameObject[] gameObjects)
    {
        DateTime startTime = DateTime.Now;

        GridData gridData = Rasterizer.Rasterize(gameObjects, RasterCellSize);

        ObstacleLayer obstacleLayer = ObstacleDetector.DetectObstacles(gridData, ObstacleDetectorSettings);

        DateTime beforeBuilding = DateTime.Now;
        if (_isGeneratingHexagons || _lastGeneratedHexagons == null)
        {
            Vector3 centerVector3 = (obstacleLayer.Positions[0, 0] + obstacleLayer.Positions[obstacleLayer.Width - 1, obstacleLayer.Height - 1]) / 2f;
            Vector3 sizeVector3 = obstacleLayer.Positions[0, 0] - obstacleLayer.Positions[obstacleLayer.Width - 1, obstacleLayer.Height - 1];
            Vector2 center = new Vector2(centerVector3.x, centerVector3.z);
            Vector2 size = new Vector2(Mathf.Abs(sizeVector3.x), Mathf.Abs(sizeVector3.z));
            _lastGeneratedHexagons = _hexagonMapBuilder.BuildMap(center, size, _hexagonSize);
        }

        DateTime afterBuilding = DateTime.Now;

        List<NavMeshCell> cells = CellsFromHexagonsReceiver.GetCells(_lastGeneratedHexagons, obstacleLayer);
        CellAndPortalGraph cellAndPortalGraph = new CellAndPortalGraph(cells);

        DateTime endTime = DateTime.Now;
        Debug.Log($"(Quadrilateral Grid Nav Mesh) Time for generation - {(endTime - startTime).TotalSeconds}");
        Debug.Log($"(Quadrilateral Grid Nav Mesh) Time for generation (NO BUILD) - {(endTime - afterBuilding).TotalSeconds + (beforeBuilding - startTime).TotalSeconds}");

        OnHexagonsReceived.Invoke(_lastGeneratedHexagons);

        return cellAndPortalGraph;
    }
}
