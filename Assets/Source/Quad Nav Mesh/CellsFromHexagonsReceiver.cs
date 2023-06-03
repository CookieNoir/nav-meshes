using System;
using System.Collections.Generic;
using UnityEngine;

public static class CellsFromHexagonsReceiver
{
    private const float _distanceSquaredThreshold = 1e-6f;

    public struct HexagonCellData
    {
        public int FirstQuadIndex;
        public NavMeshCell Cell;

        public HexagonCellData(int firstQuadIndex, NavMeshCell cell)
        {
            FirstQuadIndex = firstQuadIndex;
            Cell = cell;
        }
    }

    public struct BorderCellData
    {
        public HexagonModel HexagonModel;
        public List<NavMeshCell> Cells;

        public BorderCellData(HexagonModel hexagonModel, List<NavMeshCell> cells)
        {
            HexagonModel = hexagonModel;
            Cells = cells;
        }
    }

    public static List<NavMeshCell> GetCells(List<HexagonModel> hexagonModels, ObstacleLayer obstacleLayer)
    {
        List<NavMeshCell> cells = new List<NavMeshCell>();
        List<BorderCellData> borderCellDatas = new List<BorderCellData>();
        List<HexagonCellData> hexagonCells = new List<HexagonCellData>();
        foreach (var hexagonModel in hexagonModels)
        {
            hexagonCells.Clear();
            List<NavMeshCell> borderCells = new List<NavMeshCell>();
            Vector3[] vertices = hexagonModel.MeshCreator.Vertices;
            int[] quads = hexagonModel.MeshCreator.Quads;
            int quadsCount = quads.Length / 4;
            for (int i = 0; i < quadsCount; ++i)
            {
                int firstQuadIndex = i * 4;
                int v0 = quads[firstQuadIndex];
                int v1 = quads[firstQuadIndex + 1];
                int v2 = quads[firstQuadIndex + 2];
                Vector3 point0 = hexagonModel.Position + vertices[v0];
                Vector3 point1 = hexagonModel.Position + vertices[v1];
                Vector3 point2 = hexagonModel.Position + vertices[v2];
                if (!IsTriangleContainingObstacle(obstacleLayer, point0, point1, point2))
                {
                    int v3 = quads[firstQuadIndex + 3];
                    Vector3 point3 = hexagonModel.Position + vertices[v3];
                    if (!IsTriangleContainingObstacle(obstacleLayer, point0, point2, point3))
                    {
                        NavMeshCell cell = new NavMeshCell(new Vector3[] { point0, point1, point2, point3 });
                        cells.Add(cell);
                        hexagonCells.Add(new HexagonCellData(firstQuadIndex, cell));

                        int borderVerticesCount = 0;
                        if (UniversalHexagon.IsBorderVertex(v0)) { borderVerticesCount++; }
                        if (UniversalHexagon.IsBorderVertex(v1)) { borderVerticesCount++; }
                        if (UniversalHexagon.IsBorderVertex(v2)) { borderVerticesCount++; }
                        if (UniversalHexagon.IsBorderVertex(v3)) { borderVerticesCount++; }
                        if (borderVerticesCount >= 2) { borderCells.Add(cell); }
                    }
                }
            }
            ConnectHexagonCells(hexagonCells, quads);
            borderCellDatas.Add(new BorderCellData(hexagonModel, borderCells));
        }
        ConnectBorderCells(borderCellDatas);
        return cells;
    }

    private static void ConnectHexagonCells(List<HexagonCellData> hexagonCells, int[] quads)
    {
        for (int i = 0; i < hexagonCells.Count; ++i)
        {
            for (int j = i + 1; j < hexagonCells.Count; ++j)
            {
                for (int q1 = 0; q1 < 4; ++q1)
                {
                    int v1 = quads[hexagonCells[i].FirstQuadIndex + q1];
                    for (int q2 = 0; q2 < 4; ++q2)
                    {
                        int v2 = quads[hexagonCells[j].FirstQuadIndex + q2];
                        if (v1 == v2)
                        {
                            int v1Neighbor1 = quads[hexagonCells[i].FirstQuadIndex + (q1 + 1) % 4];
                            int v1Neighbor2 = quads[hexagonCells[i].FirstQuadIndex + (q1 + 3) % 4];
                            int v2Neighbor1 = quads[hexagonCells[j].FirstQuadIndex + (q2 + 1) % 4];
                            int v2Neighbor2 = quads[hexagonCells[j].FirstQuadIndex + (q2 + 3) % 4];
                            if (v1Neighbor1 == v2Neighbor1 || v1Neighbor1 == v2Neighbor2 ||
                                v1Neighbor2 == v2Neighbor1 || v1Neighbor2 == v2Neighbor2)
                            {
                                hexagonCells[i].Cell.Neighbors.Add(hexagonCells[j].Cell);
                                hexagonCells[j].Cell.Neighbors.Add(hexagonCells[i].Cell);
                            }
                        }
                    }
                }
            }
        }
    }

    private static void ConnectBorderCells(List<BorderCellData> borderCellDatas)
    {
        for (int i = 0; i < borderCellDatas.Count; ++i)
        {
            for (int j = i + 1; j < borderCellDatas.Count; ++j)
            {
                if (borderCellDatas[i].HexagonModel.IsNeighbor(borderCellDatas[j].HexagonModel))
                {
                    ConnectBorderCellsFromNeighbors(borderCellDatas[i], borderCellDatas[j]);
                }
            }
        }
    }

    private static void ConnectBorderCellsFromNeighbors(BorderCellData a, BorderCellData b)
    {
        List<NavMeshCell> aCells = a.Cells;
        List<NavMeshCell> bCells = b.Cells;
        for (int i = 0; i < a.Cells.Count; ++i)
        {
            for (int j = 0; j < b.Cells.Count; ++j)
            {
                for (int v1 = 0; v1 < 4; ++v1)
                {
                    for (int v2 = 0; v2 < 4; ++v2)
                    {
                        if ((aCells[i].Vertices[v1] - bCells[j].Vertices[v2]).sqrMagnitude <= _distanceSquaredThreshold)
                        {
                            int v1Neighbor1 = (v1 + 1) % 4;
                            int v1Neighbor2 = (v1 + 3) % 4;
                            int v2Neighbor1 = (v2 + 1) % 4;
                            int v2Neighbor2 = (v2 + 3) % 4;
                            float d11 = (aCells[i].Vertices[v1Neighbor1] - bCells[j].Vertices[v2Neighbor1]).sqrMagnitude;
                            float d12 = (aCells[i].Vertices[v1Neighbor1] - bCells[j].Vertices[v2Neighbor2]).sqrMagnitude;
                            float d21 = (aCells[i].Vertices[v1Neighbor2] - bCells[j].Vertices[v2Neighbor1]).sqrMagnitude;
                            float d22 = (aCells[i].Vertices[v1Neighbor2] - bCells[j].Vertices[v2Neighbor2]).sqrMagnitude;
                            if (d11 <= _distanceSquaredThreshold || d12 <= _distanceSquaredThreshold ||
                                d21 <= _distanceSquaredThreshold || d22 <= _distanceSquaredThreshold)
                            {
                                aCells[i].Neighbors.Add(bCells[j]);
                                bCells[j].Neighbors.Add(aCells[i]);
                            }
                        }
                    }
                }
            }
        }
    }

    private static bool IsTriangleContainingObstacle(ObstacleLayer obstacleLayer, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float minX = Mathf.Min(Mathf.Min(p1.x, p2.x), p3.x),
              minZ = Mathf.Min(Mathf.Min(p1.z, p2.z), p3.z),
              maxX = Mathf.Max(Mathf.Max(p1.x, p2.x), p3.x),
              maxZ = Mathf.Max(Mathf.Max(p1.z, p2.z), p3.z);
        int minWidth = Mathf.FloorToInt((minX - obstacleLayer.Origin.x) / obstacleLayer.Step.x),
            maxWidth = Mathf.CeilToInt((maxX - obstacleLayer.Origin.x) / obstacleLayer.Step.x),
            minHeight = Mathf.FloorToInt((minZ - obstacleLayer.Origin.y) / obstacleLayer.Step.y),
            maxHeight = Mathf.CeilToInt((maxZ - obstacleLayer.Origin.y) / obstacleLayer.Step.y);
        if (minWidth >= obstacleLayer.Width || maxWidth < 0 || minHeight >= obstacleLayer.Height || maxHeight < 0)
        { return false; }
        else
        {
            if (minWidth < 0) { minWidth = 0; }
            if (minHeight < 0) { minHeight = 0; }
            if (maxWidth > obstacleLayer.Width) { maxWidth = obstacleLayer.Width; }
            if (maxHeight > obstacleLayer.Height) { maxHeight = obstacleLayer.Height; }
            Vector2Int p1Int = new Vector2Int(Mathf.FloorToInt((p1.x - obstacleLayer.Origin.x) / obstacleLayer.Step.x),
                Mathf.FloorToInt((p1.z - obstacleLayer.Origin.y) / obstacleLayer.Step.y));
            Vector2Int p2Int = new Vector2Int(Mathf.FloorToInt((p2.x - obstacleLayer.Origin.x) / obstacleLayer.Step.x),
                Mathf.FloorToInt((p2.z - obstacleLayer.Origin.y) / obstacleLayer.Step.y));
            Vector2Int p3Int = new Vector2Int(Mathf.FloorToInt((p3.x - obstacleLayer.Origin.x) / obstacleLayer.Step.x),
                Mathf.FloorToInt((p3.z - obstacleLayer.Origin.y) / obstacleLayer.Step.y));
            for (int x = minWidth; x < maxWidth; ++x)
            {
                for (int y = minHeight; y < maxHeight; ++y)
                {
                    if (IsPointInTriangle(x, y, p1Int, p2Int, p3Int) && obstacleLayer.IsObstacle[x, y]) { return true; }
                }
            }
        }
        return false;
    }

    private static bool IsPointInTriangle(int x, int y, Vector2Int p1, Vector2Int p2, Vector2Int p3)
    {
        float s1 = SignInt(x, y, p1, p2),
              s2 = SignInt(x, y, p2, p3),
              s3 = SignInt(x, y, p3, p1);
        bool hasNegative = (s1 < 0) || (s2 < 0) || (s3 < 0),
             hasPositive = (s1 > 0) || (s2 > 0) || (s3 > 0);
        return !(hasNegative && hasPositive);
    }

    private static int SignInt(int p1x, int p1y, Vector2Int p2, Vector2Int p3)
    {
        return (p1x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1y - p3.y);
    }
}
