using System;
using System.Collections.Generic;
using UnityEngine;

public class HexagonMapBuilder : MonoBehaviour
{
    [SerializeField] private UniversalHexagon _singleHexagonGrid;
    [SerializeField] private RelaxationProperties _relaxationProperties;

    public List<HexagonModel> BuildMap(Vector2 center, Vector2 size, float hexagonSize)
    {
        var (hexagons, neighborRadius) = CreateRectangularGrid(center, size, hexagonSize);
        RelaxGrid(hexagons, neighborRadius);
        return GetOnlyRelaxedHexagons(hexagons);
    }

    private List<HexagonModel> GetOnlyRelaxedHexagons(HexagonModel[,] hexagons)
    {
        List<HexagonModel> result = new List<HexagonModel>();
        for (int x = 0; x < hexagons.GetLength(0); ++x)
        {
            for (int y = 0; y < hexagons.GetLength(1); ++y)
            {
                if (hexagons[x, y] != null && hexagons[x, y].MeshCreator.IsRelaxed)
                {
                    result.Add(hexagons[x, y]);
                }
            }
        }
        return result;
    }

    private void RelaxGrid(HexagonModel[,] hexagons, int neighborRadius)
    {
        _relaxationProperties.ApplyProperties();
        if (RelaxationProperties.iterationsCount > 0)
        {
            Vector3Int cubeCoordinate;
            for (int i = 1; i < neighborRadius; ++i)
            {
                cubeCoordinate = i * CubeCoordinates.neighbors[4];
                for (int j = 0; j < 6; ++j)
                {
                    for (int k = 0; k < i; ++k)
                    {
                        cubeCoordinate = CubeCoordinates.GetNeighbor(cubeCoordinate, j);
                        for (int d = 0; d < 6; ++d)
                        {
                            HexagonModel targetModel = hexagons[neighborRadius + cubeCoordinate.x, neighborRadius + cubeCoordinate.z];
                            Vector3Int direction = CubeCoordinates.GetNeighbor(cubeCoordinate, d);
                            HexagonModel directionModel = hexagons[neighborRadius + direction.x, neighborRadius + direction.z];
                            Vector3Int directionMinusOne = CubeCoordinates.GetNeighbor(cubeCoordinate, (d + 5) % 6);
                            HexagonModel directionMinusOneModel = hexagons[neighborRadius + directionMinusOne.x, neighborRadius + directionMinusOne.z];
                            TryRelaxTriplet(d, targetModel, directionModel, directionMinusOneModel);
                        }
                    }
                }
            }
        }
    }

    private void TryRelaxTriplet(int direction, HexagonModel central, HexagonModel directionNeighbor, HexagonModel directionMinusOneNeighbor)
    {
        if (central == null || directionNeighbor == null || directionMinusOneNeighbor == null) { return; }
        central.Neighbors[direction] = directionNeighbor;
        central.Neighbors[(direction + 5) % 6] = directionMinusOneNeighbor;
        TripletRelaxator.RelaxTriplet(direction, central.RelaxationData, directionNeighbor.RelaxationData, directionMinusOneNeighbor.RelaxationData);
    }

    private (HexagonModel[,], int) CreateRectangularGrid(Vector2 center, Vector2 size, float hexagonSize)
    {
        float unitFactor = hexagonSize / (6 * _singleHexagonGrid.radius);
        _singleHexagonGrid.unitFactor = new Vector2(unitFactor, unitFactor);
        _singleHexagonGrid.FillStaticFields();

        float halfAreaRightSize = size.x / 2f;
        int halfHexCountRight = Mathf.RoundToInt((halfAreaRightSize - hexagonSize / 2f) / hexagonSize) + 1;

        float halfHexUpSize = UniversalHexagon.hexagonSize.y / 2f;
        float halfAreaUpSize = size.y / 2f;
        int halves = Mathf.CeilToInt(halfAreaUpSize / halfHexUpSize) - 1;
        int halfHexCountUp = 1 + halves / 3;
        if (halves % 3 != 0) { halfHexCountUp++; }

        halfHexCountRight += halfHexCountUp / 2;
        int neighborRadius = Math.Max(halfHexCountRight, halfHexCountUp);
        Vector2 extendedSize = new Vector2(size.x + 3 * hexagonSize, 2f * (1 + 3 * halfHexCountUp) * halfHexUpSize);
        Vector2 halfSize = extendedSize / 2f;

        int neighborRadiusBidirectional = neighborRadius + neighborRadius + 1;
        HexagonModel[,] hexagons = new HexagonModel[neighborRadiusBidirectional, neighborRadiusBidirectional];
        Vector3Int cubeCoordinate;

        Vector3 centerVector3 = new Vector3(center.x, 0f, center.y);
        hexagons[neighborRadius, neighborRadius] = new HexagonModel(centerVector3);
        for (int i = 1; i <= neighborRadius; ++i)
        {
            cubeCoordinate = i * CubeCoordinates.neighbors[4];
            for (int j = 0; j < 6; ++j)
            {
                for (int k = 0; k < i; ++k)
                {
                    cubeCoordinate = CubeCoordinates.GetNeighbor(cubeCoordinate, j);
                    Vector3 position = CubeCoordinates.CubePointyTopToCartesianXZ(cubeCoordinate, UniversalHexagon.hexagonSize);
                    if (IsInside(position, halfSize))
                    {                     
                        int x = neighborRadius + cubeCoordinate.x;
                        int y = neighborRadius + cubeCoordinate.z;
                        hexagons[x, y] = new HexagonModel(centerVector3 + position);
                    }
                }
            }
        }
        return (hexagons, neighborRadius);
    }

    private static bool IsInside(Vector3 position, Vector2 halfSize)
    {
        return (position.x >= -halfSize.x && position.x <= halfSize.x &&
            position.z >= -halfSize.y && position.z <= halfSize.y);
    }
}
