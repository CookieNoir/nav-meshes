using System;
using UnityEngine;

public class HexagonRelaxationData
{
    public class DirectionRelaxationData
    {
        public Vector3[] vertices;
        public int[] counts;
        public float relaxationMultiplier;
        public bool isRelaxed;

        public DirectionRelaxationData()
        {
            vertices = null;
            counts = null;
            relaxationMultiplier = RelaxationProperties.baseOffsetMultiplier;
            isRelaxed = false;
        }

        public void Multiply()
        {
            relaxationMultiplier *= RelaxationProperties.offsetMultiplierPerIteration;
        }

        public void ResetData(Vector3[] newVertices)
        {
            vertices = new Vector3[newVertices.Length];
            counts = new int[newVertices.Length];
            Array.Copy(newVertices, vertices, newVertices.Length);
            relaxationMultiplier = RelaxationProperties.baseOffsetMultiplier;
        }

        public void ApplyOffsets(Vector3[] offsets, int[] counts)
        {
            for (int i = 0; i < vertices.Length; ++i)
            {
                vertices[i] += (offsets[i] / counts[i]) * relaxationMultiplier;
            }
        }

        public void GetOffsetsAndCounts(HexagonMeshCreator meshCreator, Vector3[] offsets, int[] counts)
        {
            // Counterclockwise bypass
            int i0, i1, i2, i3;
            Vector2 point0, point1, point2, point3;
            Vector2 center;

            for (int i = 0; i < meshCreator.Quads.Length; i += 4)
            {
                i0 = meshCreator.Quads[i];
                i1 = meshCreator.Quads[i + 1];
                i2 = meshCreator.Quads[i + 2];
                i3 = meshCreator.Quads[i + 3];
                point0 = new Vector2(vertices[i0].x, vertices[i0].z);
                point1 = new Vector2(vertices[i1].x, vertices[i1].z);
                point2 = new Vector2(vertices[i2].x, vertices[i2].z);
                point3 = new Vector2(vertices[i3].x, vertices[i3].z);
                center = (point0 + point1 + point2 + point3) * 0.25f;
                point0 -= center;
                point1 -= center;
                point2 -= center;
                point3 -= center;
                // point0 isn't rotated
                point1 = new Vector2(-point1.y, point1.x);
                point2 *= -1f;
                point3 = new Vector2(point3.y, -point3.x);
                center = (point0 + point1 + point2 + point3) * 0.25f;
                point0 -= center;
                point1 -= center;
                point2 -= center;
                point3 -= center;
                offsets[i0] += new Vector3(-point0.x, 0f, -point0.y);
                offsets[i1] += new Vector3(point1.x, 0f, point1.y);
                offsets[i2] += new Vector3(-point2.x, 0f, -point2.y);
                offsets[i3] += new Vector3(point3.x, 0f, point3.y);
                counts[i0]++;
                counts[i1]++;
                counts[i2]++;
                counts[i3]++;
            }
        }

        public void GetOffsets(HexagonMeshCreator meshCreator, Vector3[] offsets)
        {
            // Counterclockwise bypass
            int i0, i1, i2, i3;
            Vector2 point0, point1, point2, point3;
            Vector2 center;

            for (int i = 0; i < meshCreator.Quads.Length; i += 4)
            {
                i0 = meshCreator.Quads[i];
                i1 = meshCreator.Quads[i + 1];
                i2 = meshCreator.Quads[i + 2];
                i3 = meshCreator.Quads[i + 3];
                point0 = new Vector2(vertices[i0].x, vertices[i0].z);
                point1 = new Vector2(vertices[i1].x, vertices[i1].z);
                point2 = new Vector2(vertices[i2].x, vertices[i2].z);
                point3 = new Vector2(vertices[i3].x, vertices[i3].z);
                center = (point0 + point1 + point2 + point3) * 0.25f;
                point0 -= center;
                point1 -= center;
                point2 -= center;
                point3 -= center;
                // point0 isn't rotated
                point1 = new Vector2(-point1.y, point1.x);
                point2 *= -1f;
                point3 = new Vector2(point3.y, -point3.x);
                center = (point0 + point1 + point2 + point3) * 0.25f;
                point0 -= center;
                point1 -= center;
                point2 -= center;
                point3 -= center;
                offsets[i0] += new Vector3(-point0.x, 0f, -point0.y);
                offsets[i1] += new Vector3(point1.x, 0f, point1.y);
                offsets[i2] += new Vector3(-point2.x, 0f, -point2.y);
                offsets[i3] += new Vector3(point3.x, 0f, point3.y);
            }
        }
    }

    private HexagonMeshCreator meshCreator;
    private DirectionRelaxationData[] directionDatas;
    private int length;
    private Vector3[] offsets;
    private int completedDirections;

    public HexagonRelaxationData(HexagonMeshCreator hexagonMeshCreator)
    {
        meshCreator = hexagonMeshCreator;
        directionDatas = new DirectionRelaxationData[6];
        for (int i = 0; i < 6; ++i)
        {
            directionDatas[i] = new DirectionRelaxationData();
        }
        length = meshCreator.Vertices.Length;
        offsets = new Vector3[length];
    }

    public void ApplyOffsetInDirection(int direction, Vector3[] outerOffsets, int[] outerCounts)
    {
        directionDatas[direction].ApplyOffsets(outerOffsets, outerCounts);
    }

    public void SetRelaxedInDirection(int direction)
    {
        directionDatas[direction].isRelaxed = true;
        completedDirections++;
        if (completedDirections == 6)
        {
            SetMeshVertices();
        }
    }

    public bool GetOffsetsInDirectionFirst(int direction, out Vector3[] resultOffsets, out int[] resultCounts)
    {
        if (directionDatas[direction].isRelaxed)
        {
            resultOffsets = null;
            resultCounts = null;
            return false;
        }
        else
        {
            for (int i = 0; i < length; ++i)
            {
                offsets[i] = Vector3.zero;
            }
            directionDatas[direction].ResetData(meshCreator.Vertices);
            directionDatas[direction].GetOffsetsAndCounts(meshCreator, offsets, directionDatas[direction].counts);
            resultCounts = directionDatas[direction].counts;
            resultOffsets = offsets;
            return true;
        }
    }

    public void GetOffsetsInDirection(int direction, out Vector3[] resultOffsets, out int[] resultCounts)
    {
        for (int i = 0; i < length; ++i)
        {
            offsets[i] = Vector3.zero;
        }
        directionDatas[direction].Multiply();
        directionDatas[direction].GetOffsets(meshCreator, offsets);
        resultCounts = directionDatas[direction].counts;
        resultOffsets = offsets;
    }

    private void SetMeshVertices()
    {
        offsets[0] = (directionDatas[0].vertices[0] +
             directionDatas[1].vertices[0] +
             directionDatas[2].vertices[0] +
             directionDatas[3].vertices[0] +
             directionDatas[4].vertices[0] +
             directionDatas[5].vertices[0]) * 0.16666666666f;
        int sector;
        float value;
        for (int i = 1; i < UniversalHexagon.pointsCount; ++i)
        {
            sector = (UniversalHexagon.pointsSectors[i] + 5) % 6;
            value = UniversalHexagon.pointsInterpolationFactors[i];
            offsets[i] = directionDatas[sector].vertices[i] * value + (1f - value) * directionDatas[(sector + 5) % 6].vertices[i];
        }
        for (int i = UniversalHexagon.pointsCount; i < UniversalHexagon.pointsAndEdgesCount; ++i)
        {
            sector = (UniversalHexagon.edgesSectors[i - UniversalHexagon.pointsCount] + 5) % 6;
            value = UniversalHexagon.edgesInterpolationFactors[i - UniversalHexagon.pointsCount];
            offsets[i] = directionDatas[sector].vertices[i] * value + (1f - value) * directionDatas[(sector + 5) % 6].vertices[i];
        }
        int index;
        for (int i = UniversalHexagon.pointsAndEdgesCount; i < offsets.Length; ++i)
        {
            index = meshCreator.TrianglesCentersIndices[i - UniversalHexagon.pointsAndEdgesCount];
            sector = (UniversalHexagon.trianglesCentersSectors[index] + 5) % 6;
            value = UniversalHexagon.trianglesCentersInterpolationFactors[index];
            offsets[i] = directionDatas[sector].vertices[i] * value + (1f - value) * directionDatas[(sector + 5) % 6].vertices[i];
        }
        meshCreator.ApplyVerticesToMesh(offsets);
    }

    public void ApplyChangesByDirection(int direction)
    {
        meshCreator.ApplyVerticesToMesh(directionDatas[direction].vertices);
    }
}