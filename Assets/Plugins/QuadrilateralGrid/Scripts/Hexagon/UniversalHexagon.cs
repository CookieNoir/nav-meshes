using System;
using UnityEngine;

public class UniversalHexagon : MonoBehaviour
{
    public static Vector3Int[] pointsCube;
    public static Vector3[] points;
    public static Vector3Int[] edgesCentersCube;
    public static Vector3[] edgesCenters;
    public static int[] triangles;
    public static int[] trianglesEdges;
    public static int[] trianglesNeighbors;
    public static Vector3[] trianglesCenters;

    public static int[] pointsSectors;
    public static float[] pointsInterpolationFactors;
    public static int[] edgesSectors;
    public static float[] edgesInterpolationFactors;
    public static int[] trianglesCentersSectors;
    public static float[] trianglesCentersInterpolationFactors;

    public static int pointsCount;
    public static int edgesCount;
    public static int pointsAndEdgesCount;
    public static int trianglesCount;
    public static Vector3[] pointsAndEdgesCenters;

    public static int pointsBeforeLastRing;
    public static int pointsAtLastRing;
    public static int hexagonRadius;
    public static Vector2 hexagonSize;
    public static int edgesBeforeLastRing;
    public static int edgesSectorCountAtLastRing;

    public static int[] pointIndexArray;
    public static int[] pointIndexOppositeArray;
    public static int[] edgeIndexArray;
    public static int[] edgeIndexOppositeArray;

    [Range(1, 10)] public int radius = 3;
    public Vector2 unitFactor;
    public void FillStaticFields()
    {
        FormHexagon();
        FormAggregateArray();
        FormAuxiliaryArrays();
    }

    private void FormHexagon()
    {
        int radius3 = 3 * radius;
        pointsCount = 1 + radius3 * (radius + 1);
        edgesCount = radius3 * (radius3 + 1);
        pointsAndEdgesCount = pointsCount + edgesCount;
        trianglesCount = 6 * radius * radius;
        int trianglesPointsCount = 3 * trianglesCount;
        pointsCube = new Vector3Int[pointsCount];
        edgesCentersCube = new Vector3Int[edgesCount];
        points = new Vector3[pointsCount];
        edgesCenters = new Vector3[edgesCount];
        triangles = new int[trianglesPointsCount];
        trianglesEdges = new int[trianglesPointsCount];
        trianglesNeighbors = new int[trianglesPointsCount];
        trianglesCenters = new Vector3[trianglesCount];

        pointsSectors = new int[pointsCount];
        pointsInterpolationFactors = new float[pointsCount];
        edgesSectors = new int[edgesCount];
        edgesInterpolationFactors = new float[edgesCount];
        trianglesCentersSectors = new int[trianglesCount];
        trianglesCentersInterpolationFactors = new float[trianglesCount];

        int pointIndex = 0;
        int edgeIndex = 1;
        int trianglePointIndex = 0;
        int triangleIndex = 0;
        int prevPointIndex1 = 0, prevPointIndex2 = 0;
        int prevEdgeIndex = 0;

        pointsCube[0] = Vector3Int.zero;
        points[0] = Vector3.zero;
        pointsSectors[0] = -1;
        pointsInterpolationFactors[0] = 0;
        Vector3Int cubeCoordinate;

        int pointsAtRing = 1,
            pointsBeforeRing = 0,
            prevPointsBeforeRing = 0,
            edgesBeforeRing = 0,
            trianglesBeforeRing = 0,
            triangleNeighborIndex = 0,
            triangleNeighborPointIndex = 0;

        float dot3 = 1f / 3f,
            edgesSectorCountInverted1, // For triangles centers that are on subline 1
            edgesSectorCountInverted2, // For triangles centers that are on subline 2
            trianglesSectorCountInverted, // For edges centers that are inside the ring
            radiusDoubledInverted, // For points and edges centers that are on the border of the ring
            curTrianglesCenterFactor1,
            curTrianglesCenterFactor2,
            curEdgesCentersFactor,
            curPointFactor;
        int[] positionOffset = new int[] { 1, 2, 0 };
        int temp;
        for (int i = 1; i <= radius; ++i)
        {
            radiusDoubledInverted = 0.5f / i;
            edgesSectorCountInverted1 = 1f / ((3f * radius) - 2f);
            edgesSectorCountInverted2 = 1f / ((3f * radius) - 1f);
            trianglesSectorCountInverted = 1f / ((2f * radius) - 1f);

            cubeCoordinate = 2 * i * CubeCoordinates.neighbors[4];

            pointsBeforeRing += pointsAtRing;
            pointsAtRing = 6 * i;
            prevPointIndex2 = prevPointIndex1;
            prevPointIndex1 = pointsBeforeRing;

            pointIndex++;
            pointsCube[pointIndex] = cubeCoordinate;
            points[pointIndex] = CubeCoordinates.CubeFlatTopToCartesianXZ(pointsCube[pointIndex], unitFactor);
            pointsSectors[pointIndex] = 0;
            pointsInterpolationFactors[pointIndex] = 0f;

            edgesBeforeRing = prevEdgeIndex;
            edgesCentersCube[prevEdgeIndex] = (pointsCube[prevPointIndex2] + pointsCube[prevPointIndex1]) / 2;
            edgesCenters[prevEdgeIndex] = CubeCoordinates.CubeFlatTopToCartesianXZ(edgesCentersCube[prevEdgeIndex], unitFactor);
            edgesSectors[prevEdgeIndex] = 0;
            edgesInterpolationFactors[prevEdgeIndex] = 0f;

            for (int j = 0; j < 5; ++j)
            {
                temp = positionOffset[2];
                positionOffset[2] = positionOffset[1];
                positionOffset[1] = positionOffset[0];
                positionOffset[0] = temp;

                curTrianglesCenterFactor1 = 2f * edgesSectorCountInverted1;
                curTrianglesCenterFactor2 = edgesSectorCountInverted2;
                curEdgesCentersFactor = trianglesSectorCountInverted;
                curPointFactor = radiusDoubledInverted;

                for (int k = 1; k < i; ++k)
                {
                    pointIndex++;
                    cubeCoordinate = CubeCoordinates.GetNeighbor(cubeCoordinate, j, 2);
                    pointsCube[pointIndex] = cubeCoordinate;
                    points[pointIndex] = CubeCoordinates.CubeFlatTopToCartesianXZ(pointsCube[pointIndex], unitFactor);
                    pointsSectors[pointIndex] = j;

                    edgesCentersCube[edgeIndex] = (pointsCube[pointIndex] + pointsCube[prevPointIndex1]) / 2;
                    edgesCentersCube[edgeIndex + 1] = (pointsCube[pointIndex] + pointsCube[prevPointIndex2]) / 2;
                    edgesCenters[edgeIndex] = CubeCoordinates.CubeFlatTopToCartesianXZ(edgesCentersCube[edgeIndex], unitFactor);
                    edgesCenters[edgeIndex + 1] = CubeCoordinates.CubeFlatTopToCartesianXZ(edgesCentersCube[edgeIndex + 1], unitFactor);
                    edgesSectors[edgeIndex] = j;
                    edgesSectors[edgeIndex + 1] = j;
                    edgesInterpolationFactors[edgeIndex] = curPointFactor;
                    edgesInterpolationFactors[edgeIndex + 1] = curEdgesCentersFactor;
                    curPointFactor += radiusDoubledInverted;
                    pointsInterpolationFactors[pointIndex] = curPointFactor;

                    triangles[trianglePointIndex + positionOffset[0]] = prevPointIndex2;
                    triangles[trianglePointIndex + positionOffset[1]] = prevPointIndex1;
                    triangles[trianglePointIndex + positionOffset[2]] = pointIndex;

                    trianglesEdges[trianglePointIndex + positionOffset[0]] = edgeIndex;
                    trianglesEdges[trianglePointIndex + positionOffset[1]] = edgeIndex + 1;
                    trianglesEdges[trianglePointIndex + positionOffset[2]] = prevEdgeIndex;

                    trianglesNeighbors[trianglePointIndex + 3 + positionOffset[1]] = triangleIndex;
                    trianglesNeighbors[trianglePointIndex + positionOffset[1]] = triangleIndex + 1;
                    trianglesNeighbors[trianglePointIndex + positionOffset[0]] = -1;

                    trianglesCenters[triangleIndex] = dot3 * (edgesCenters[trianglesEdges[trianglePointIndex]] + edgesCenters[trianglesEdges[trianglePointIndex + 1]] + edgesCenters[trianglesEdges[trianglePointIndex + 2]]);
                    trianglesCentersSectors[triangleIndex] = j;
                    trianglesCentersInterpolationFactors[triangleIndex] = curTrianglesCenterFactor2;

                    prevPointIndex1 = prevPointIndex2 + 1;
                    prevEdgeIndex = edgeIndex + 1;
                    edgeIndex += 2;
                    trianglePointIndex += 3;
                    triangleIndex++;
                    curTrianglesCenterFactor2 += 3f * edgesSectorCountInverted2;
                    curEdgesCentersFactor += trianglesSectorCountInverted;
                    curPointFactor += radiusDoubledInverted;

                    edgesCentersCube[edgeIndex] = (pointsCube[pointIndex] + pointsCube[prevPointIndex1]) / 2;
                    edgesCenters[edgeIndex] = CubeCoordinates.CubeFlatTopToCartesianXZ(edgesCentersCube[edgeIndex], unitFactor);
                    edgesSectors[edgeIndex] = j;
                    edgesInterpolationFactors[edgeIndex] = curEdgesCentersFactor;

                    triangles[trianglePointIndex + positionOffset[0]] = pointIndex;
                    triangles[trianglePointIndex + positionOffset[1]] = prevPointIndex1;
                    triangles[trianglePointIndex + positionOffset[2]] = prevPointIndex2;

                    trianglesNeighbors[trianglePointIndex + 3 + positionOffset[2]] = triangleIndex;
                    trianglesNeighbors[trianglePointIndex + positionOffset[0]] = triangleNeighborIndex;
                    trianglesNeighbors[trianglePointIndex + positionOffset[2]] = triangleIndex + 1;
                    trianglesNeighbors[triangleNeighborPointIndex + positionOffset[0]] = triangleIndex;

                    trianglesEdges[trianglePointIndex + positionOffset[0]] = trianglesEdges[triangleNeighborPointIndex + positionOffset[0]];
                    trianglesEdges[trianglePointIndex + positionOffset[1]] = prevEdgeIndex;
                    trianglesEdges[trianglePointIndex + positionOffset[2]] = edgeIndex;

                    trianglesCenters[triangleIndex] = dot3 * (edgesCenters[trianglesEdges[trianglePointIndex]] + edgesCenters[trianglesEdges[trianglePointIndex + 1]] + edgesCenters[trianglesEdges[trianglePointIndex + 2]]);
                    trianglesCentersSectors[triangleIndex] = j;
                    trianglesCentersInterpolationFactors[triangleIndex] = curTrianglesCenterFactor1;

                    prevPointIndex2 = prevPointIndex1;
                    prevPointIndex1 = pointIndex;
                    prevEdgeIndex = edgeIndex;
                    edgeIndex += 1;
                    trianglePointIndex += 3;
                    triangleIndex++;
                    triangleNeighborIndex += 2;
                    triangleNeighborPointIndex += 6;
                    curTrianglesCenterFactor1 += 3f * edgesSectorCountInverted1;
                    curEdgesCentersFactor += trianglesSectorCountInverted;
                }

                pointIndex++;
                cubeCoordinate = CubeCoordinates.GetNeighbor(cubeCoordinate, j, 2);
                pointsCube[pointIndex] = cubeCoordinate;
                points[pointIndex] = CubeCoordinates.CubeFlatTopToCartesianXZ(pointsCube[pointIndex], unitFactor);
                pointsSectors[pointIndex] = j + 1;
                pointsInterpolationFactors[pointIndex] = 0f;

                edgesCentersCube[edgeIndex] = (pointsCube[pointIndex] + pointsCube[prevPointIndex1]) / 2;
                edgesCentersCube[edgeIndex + 1] = (pointsCube[pointIndex] + pointsCube[prevPointIndex2]) / 2;
                edgesCenters[edgeIndex] = CubeCoordinates.CubeFlatTopToCartesianXZ(edgesCentersCube[edgeIndex], unitFactor);
                edgesCenters[edgeIndex + 1] = CubeCoordinates.CubeFlatTopToCartesianXZ(edgesCentersCube[edgeIndex + 1], unitFactor);
                edgesSectors[edgeIndex] = j;
                edgesSectors[edgeIndex + 1] = j + 1;
                edgesInterpolationFactors[edgeIndex] = curPointFactor;
                edgesInterpolationFactors[edgeIndex + 1] = 0f;

                triangles[trianglePointIndex + positionOffset[0]] = prevPointIndex2;
                triangles[trianglePointIndex + positionOffset[1]] = prevPointIndex1;
                triangles[trianglePointIndex + positionOffset[2]] = pointIndex;

                trianglesEdges[trianglePointIndex + positionOffset[0]] = edgeIndex;
                trianglesEdges[trianglePointIndex + positionOffset[1]] = edgeIndex + 1;
                trianglesEdges[trianglePointIndex + positionOffset[2]] = prevEdgeIndex;

                trianglesNeighbors[trianglePointIndex + 3 + positionOffset[1]] = triangleIndex;
                trianglesNeighbors[trianglePointIndex + positionOffset[1]] = triangleIndex + 1;
                trianglesNeighbors[trianglePointIndex + positionOffset[0]] = -1;

                trianglesCenters[triangleIndex] = dot3 * (edgesCenters[trianglesEdges[trianglePointIndex]] + edgesCenters[trianglesEdges[trianglePointIndex + 1]] + edgesCenters[trianglesEdges[trianglePointIndex + 2]]);
                trianglesCentersSectors[triangleIndex] = j;
                trianglesCentersInterpolationFactors[triangleIndex] = curTrianglesCenterFactor2;

                prevPointIndex1 = pointIndex;
                prevEdgeIndex = edgeIndex + 1;
                edgeIndex += 2;
                trianglePointIndex += 3;
                triangleIndex++;
                triangleNeighborIndex -= 1;
                triangleNeighborPointIndex -= 3;
            }

            temp = positionOffset[2];
            positionOffset[2] = positionOffset[1];
            positionOffset[1] = positionOffset[0];
            positionOffset[0] = temp;

            curTrianglesCenterFactor1 = 2f * edgesSectorCountInverted1;
            curTrianglesCenterFactor2 = edgesSectorCountInverted2;
            curEdgesCentersFactor = trianglesSectorCountInverted;
            curPointFactor = radiusDoubledInverted;

            for (int k = 2; k < i; ++k)
            {
                pointIndex++;
                cubeCoordinate = CubeCoordinates.GetNeighbor(cubeCoordinate, 5, 2);
                pointsCube[pointIndex] = cubeCoordinate;
                points[pointIndex] = CubeCoordinates.CubeFlatTopToCartesianXZ(pointsCube[pointIndex], unitFactor);
                pointsSectors[pointIndex] = 5;

                edgesCentersCube[edgeIndex] = (pointsCube[pointIndex] + pointsCube[prevPointIndex1]) / 2;
                edgesCentersCube[edgeIndex + 1] = (pointsCube[pointIndex] + pointsCube[prevPointIndex2]) / 2;
                edgesCenters[edgeIndex] = CubeCoordinates.CubeFlatTopToCartesianXZ(edgesCentersCube[edgeIndex], unitFactor);
                edgesCenters[edgeIndex + 1] = CubeCoordinates.CubeFlatTopToCartesianXZ(edgesCentersCube[edgeIndex + 1], unitFactor);
                edgesSectors[edgeIndex] = 5;
                edgesSectors[edgeIndex + 1] = 5;
                edgesInterpolationFactors[edgeIndex] = curPointFactor;
                edgesInterpolationFactors[edgeIndex + 1] = curEdgesCentersFactor;
                curPointFactor += radiusDoubledInverted;
                pointsInterpolationFactors[pointIndex] = curPointFactor;

                triangles[trianglePointIndex + positionOffset[0]] = prevPointIndex2;
                triangles[trianglePointIndex + positionOffset[1]] = prevPointIndex1;
                triangles[trianglePointIndex + positionOffset[2]] = pointIndex;

                trianglesEdges[trianglePointIndex + positionOffset[0]] = edgeIndex;
                trianglesEdges[trianglePointIndex + positionOffset[1]] = edgeIndex + 1;
                trianglesEdges[trianglePointIndex + positionOffset[2]] = prevEdgeIndex;

                trianglesNeighbors[trianglePointIndex + 3 + positionOffset[1]] = triangleIndex;
                trianglesNeighbors[trianglePointIndex + positionOffset[1]] = triangleIndex + 1;
                trianglesNeighbors[trianglePointIndex + positionOffset[0]] = -1;

                trianglesCenters[triangleIndex] = dot3 * (edgesCenters[trianglesEdges[trianglePointIndex]] + edgesCenters[trianglesEdges[trianglePointIndex + 1]] + edgesCenters[trianglesEdges[trianglePointIndex + 2]]);
                trianglesCentersSectors[triangleIndex] = 5;
                trianglesCentersInterpolationFactors[triangleIndex] = curTrianglesCenterFactor2;

                prevPointIndex1 = prevPointIndex2 + 1;
                prevEdgeIndex = edgeIndex + 1;
                edgeIndex += 2;
                trianglePointIndex += 3;
                triangleIndex++;
                curTrianglesCenterFactor2 += 3f * edgesSectorCountInverted2;
                curEdgesCentersFactor += trianglesSectorCountInverted;
                curPointFactor += radiusDoubledInverted;

                edgesCentersCube[edgeIndex] = (pointsCube[pointIndex] + pointsCube[prevPointIndex1]) / 2;
                edgesCenters[edgeIndex] = CubeCoordinates.CubeFlatTopToCartesianXZ(edgesCentersCube[edgeIndex], unitFactor);
                edgesSectors[edgeIndex] = 5;
                edgesInterpolationFactors[edgeIndex] = curEdgesCentersFactor;

                triangles[trianglePointIndex + positionOffset[0]] = pointIndex;
                triangles[trianglePointIndex + positionOffset[1]] = prevPointIndex1;
                triangles[trianglePointIndex + positionOffset[2]] = prevPointIndex2;

                trianglesNeighbors[trianglePointIndex + 3 + positionOffset[2]] = triangleIndex;
                trianglesNeighbors[trianglePointIndex + positionOffset[0]] = triangleNeighborIndex;
                trianglesNeighbors[trianglePointIndex + positionOffset[2]] = triangleIndex + 1;
                trianglesNeighbors[triangleNeighborPointIndex + positionOffset[0]] = triangleIndex;

                trianglesEdges[trianglePointIndex + positionOffset[0]] = trianglesEdges[triangleNeighborPointIndex + positionOffset[0]];
                trianglesEdges[trianglePointIndex + positionOffset[1]] = prevEdgeIndex;
                trianglesEdges[trianglePointIndex + positionOffset[2]] = edgeIndex;

                trianglesCenters[triangleIndex] = dot3 * (edgesCenters[trianglesEdges[trianglePointIndex]] + edgesCenters[trianglesEdges[trianglePointIndex + 1]] + edgesCenters[trianglesEdges[trianglePointIndex + 2]]);
                trianglesCentersSectors[triangleIndex] = 5;
                trianglesCentersInterpolationFactors[triangleIndex] = curTrianglesCenterFactor1;

                prevPointIndex2 = prevPointIndex1;
                prevPointIndex1 = pointIndex;
                prevEdgeIndex = edgeIndex;
                edgeIndex += 1;
                trianglePointIndex += 3;
                triangleIndex++;
                triangleNeighborIndex += 2;
                triangleNeighborPointIndex += 6;
                curTrianglesCenterFactor1 += 3f * edgesSectorCountInverted1;
                curEdgesCentersFactor += trianglesSectorCountInverted;
            }

            // Выполняем отдельно, т.к. prevPointIndex2 будет равен началу предыдущего кольца
            if (i > 1)
            {
                pointIndex++;
                cubeCoordinate = CubeCoordinates.GetNeighbor(cubeCoordinate, 5, 2);
                pointsCube[pointIndex] = cubeCoordinate;
                points[pointIndex] = CubeCoordinates.CubeFlatTopToCartesianXZ(pointsCube[pointIndex], unitFactor);
                pointsSectors[pointIndex] = 5;

                edgesCentersCube[edgeIndex] = (pointsCube[pointIndex] + pointsCube[prevPointIndex1]) / 2;
                edgesCentersCube[edgeIndex + 1] = (pointsCube[pointIndex] + pointsCube[prevPointIndex2]) / 2;
                edgesCenters[edgeIndex] = CubeCoordinates.CubeFlatTopToCartesianXZ(edgesCentersCube[edgeIndex], unitFactor);
                edgesCenters[edgeIndex + 1] = CubeCoordinates.CubeFlatTopToCartesianXZ(edgesCentersCube[edgeIndex + 1], unitFactor);
                edgesSectors[edgeIndex] = 5;
                edgesSectors[edgeIndex + 1] = 5;
                edgesInterpolationFactors[edgeIndex] = curPointFactor;
                edgesInterpolationFactors[edgeIndex + 1] = curEdgesCentersFactor;
                curPointFactor += radiusDoubledInverted;
                pointsInterpolationFactors[pointIndex] = curPointFactor;

                triangles[trianglePointIndex + positionOffset[0]] = prevPointIndex2;
                triangles[trianglePointIndex + positionOffset[1]] = prevPointIndex1;
                triangles[trianglePointIndex + positionOffset[2]] = pointIndex;

                trianglesEdges[trianglePointIndex + positionOffset[0]] = edgeIndex;
                trianglesEdges[trianglePointIndex + positionOffset[1]] = edgeIndex + 1;
                trianglesEdges[trianglePointIndex + positionOffset[2]] = prevEdgeIndex;

                trianglesNeighbors[trianglePointIndex + 3 + positionOffset[1]] = triangleIndex;
                trianglesNeighbors[trianglePointIndex + positionOffset[1]] = triangleIndex + 1;
                trianglesNeighbors[trianglePointIndex + positionOffset[0]] = -1;

                trianglesCenters[triangleIndex] = dot3 * (edgesCenters[trianglesEdges[trianglePointIndex]] + edgesCenters[trianglesEdges[trianglePointIndex + 1]] + edgesCenters[trianglesEdges[trianglePointIndex + 2]]);
                trianglesCentersSectors[triangleIndex] = 5;
                trianglesCentersInterpolationFactors[triangleIndex] = curTrianglesCenterFactor2;

                prevPointIndex1 = prevPointsBeforeRing;
                prevEdgeIndex = edgeIndex + 1;
                edgeIndex += 2;
                trianglePointIndex += 3;
                triangleIndex++;
                curTrianglesCenterFactor2 += 3f * edgesSectorCountInverted2;
                curEdgesCentersFactor += trianglesSectorCountInverted;
                curPointFactor += radiusDoubledInverted;

                edgesCentersCube[edgeIndex] = (pointsCube[pointIndex] + pointsCube[prevPointIndex1]) / 2;
                edgesCenters[edgeIndex] = CubeCoordinates.CubeFlatTopToCartesianXZ(edgesCentersCube[edgeIndex], unitFactor);
                edgesSectors[edgeIndex] = 5;
                edgesInterpolationFactors[edgeIndex] = curEdgesCentersFactor;

                triangles[trianglePointIndex + positionOffset[0]] = pointIndex;
                triangles[trianglePointIndex + positionOffset[1]] = prevPointIndex1;
                triangles[trianglePointIndex + positionOffset[2]] = prevPointIndex2;

                trianglesNeighbors[trianglePointIndex + 3 + positionOffset[2]] = triangleIndex;
                trianglesNeighbors[trianglePointIndex + positionOffset[0]] = triangleNeighborIndex;
                trianglesNeighbors[trianglePointIndex + positionOffset[2]] = triangleIndex + 1;
                trianglesNeighbors[triangleNeighborPointIndex + positionOffset[0]] = triangleIndex;

                trianglesEdges[trianglePointIndex + positionOffset[0]] = trianglesEdges[triangleNeighborPointIndex + positionOffset[0]];
                trianglesEdges[trianglePointIndex + positionOffset[1]] = prevEdgeIndex;
                trianglesEdges[trianglePointIndex + positionOffset[2]] = edgeIndex;

                trianglesCenters[triangleIndex] = dot3 * (edgesCenters[trianglesEdges[trianglePointIndex]] + edgesCenters[trianglesEdges[trianglePointIndex + 1]] + edgesCenters[trianglesEdges[trianglePointIndex + 2]]);
                trianglesCentersSectors[triangleIndex] = 5;
                trianglesCentersInterpolationFactors[triangleIndex] = curTrianglesCenterFactor1;

                prevPointIndex2 = prevPointIndex1;
                prevPointIndex1 = pointIndex;
                prevEdgeIndex = edgeIndex;
                edgeIndex += 1;
                trianglePointIndex += 3;
                triangleIndex++;
                triangleNeighborIndex += 2;
                triangleNeighborPointIndex += 6;
                curTrianglesCenterFactor1 += 3f * edgesSectorCountInverted1;
                curEdgesCentersFactor += trianglesSectorCountInverted;
            }
            //--------------------------------------------

            edgesCentersCube[edgeIndex] = (pointsCube[pointsBeforeRing] + pointsCube[prevPointIndex1]) / 2;
            edgesCentersCube[edgesBeforeRing] = (pointsCube[pointsBeforeRing] + pointsCube[prevPointIndex2]) / 2;
            edgesCenters[edgeIndex] = CubeCoordinates.CubeFlatTopToCartesianXZ(edgesCentersCube[edgeIndex], unitFactor);
            edgesCenters[edgesBeforeRing] = CubeCoordinates.CubeFlatTopToCartesianXZ(edgesCentersCube[edgesBeforeRing], unitFactor);
            edgesSectors[edgeIndex] = 5;
            edgesInterpolationFactors[edgeIndex] = curPointFactor;

            triangles[trianglePointIndex + positionOffset[0]] = prevPointIndex2;
            triangles[trianglePointIndex + positionOffset[1]] = prevPointIndex1;
            triangles[trianglePointIndex + positionOffset[2]] = pointsBeforeRing;

            trianglesEdges[trianglePointIndex + positionOffset[0]] = edgeIndex;
            trianglesEdges[trianglePointIndex + positionOffset[1]] = edgesBeforeRing;
            trianglesEdges[trianglePointIndex + positionOffset[2]] = prevEdgeIndex;

            trianglesBeforeRing = 6 * (i - 1) * (i - 1);
            triangleNeighborIndex = trianglesBeforeRing;
            triangleNeighborPointIndex = 3 * trianglesBeforeRing;

            trianglesNeighbors[triangleNeighborPointIndex + positionOffset[1]] = triangleIndex;
            trianglesNeighbors[trianglePointIndex + positionOffset[1]] = trianglesBeforeRing;
            trianglesNeighbors[trianglePointIndex + positionOffset[0]] = -1;

            trianglesCenters[triangleIndex] = dot3 * (edgesCenters[trianglesEdges[trianglePointIndex]] + edgesCenters[trianglesEdges[trianglePointIndex + 1]] + edgesCenters[trianglesEdges[trianglePointIndex + 2]]);
            trianglesCentersSectors[triangleIndex] = 5;
            trianglesCentersInterpolationFactors[triangleIndex] = curTrianglesCenterFactor2;

            prevPointIndex1 = pointsBeforeRing;
            prevEdgeIndex = edgeIndex + 1;
            edgeIndex += 2;
            trianglePointIndex += 3;
            triangleIndex++;

            prevPointsBeforeRing = pointsBeforeRing;
        }

        pointsBeforeLastRing = pointsBeforeRing;
        pointsAtLastRing = pointsAtRing;
        hexagonRadius = radius;
        hexagonSize = unitFactor * radius * 3.46410161514f; // 4 * sqrt(3)/2 - Почему?
        edgesBeforeLastRing = 9 * radius * radius - 15 * radius + 6;
        edgesSectorCountAtLastRing = 3 * radius - 1;
    }

    private void FormAggregateArray()
    {
        pointsAndEdgesCenters = new Vector3[pointsAndEdgesCount];
        Array.Copy(points, 0, pointsAndEdgesCenters, 0, pointsCount);
        Array.Copy(edgesCenters, 0, pointsAndEdgesCenters, pointsCount, edgesCount);
    }

    public static void GetTrianglePointsIndices(int trianglePointIndex, ref int index0, ref int index1, ref int index2)
    {
        index0 = triangles[trianglePointIndex];
        index1 = triangles[trianglePointIndex + 1];
        index2 = triangles[trianglePointIndex + 2];
    }

    public static void GetEdgesIndicesWithPointOffset(int trianglePointIndex, ref int index0, ref int index1, ref int index2)
    {
        index0 = trianglesEdges[trianglePointIndex] + pointsCount;
        index1 = trianglesEdges[trianglePointIndex + 1] + pointsCount;
        index2 = trianglesEdges[trianglePointIndex + 2] + pointsCount;
    }

    public static void GetQuadPointsAndEdgesIndicesWithPointOffset(int trianglePointIndex, int neighborPointIndex, int neighborNumber,
        ref int pointIndex0, ref int pointIndex1, ref int pointIndex2, ref int pointIndex3,
        ref int edgeIndex0, ref int edgeIndex1, ref int edgeIndex2, ref int edgeIndex3, ref int edgeIndexQuadCenter)
    {
        int p1 = (neighborNumber + 1) % 3,
            p2 = (neighborNumber + 2) % 3;
        edgeIndexQuadCenter = trianglesEdges[trianglePointIndex + neighborNumber] + pointsCount;
        pointIndex0 = triangles[trianglePointIndex + neighborNumber];
        edgeIndex0 = trianglesEdges[trianglePointIndex + p2] + pointsCount;
        pointIndex1 = triangles[trianglePointIndex + p1];
        edgeIndex1 = trianglesEdges[neighborPointIndex + p1] + pointsCount;
        pointIndex2 = triangles[neighborPointIndex + neighborNumber];
        edgeIndex2 = trianglesEdges[neighborPointIndex + p2] + pointsCount;
        pointIndex3 = triangles[trianglePointIndex + p2];
        edgeIndex3 = trianglesEdges[trianglePointIndex + p1] + pointsCount;
    }

    public static bool IsBorderVertex(int index)
    {
        if (index < pointsAndEdgesCount)
        {
            if (index < pointsCount)
            {
                return index >= pointsBeforeLastRing;
            }
            else
            {
                int edgeIndex = index - pointsCount - edgesBeforeLastRing;
                if (edgeIndex > 0)
                {
                    edgeIndex %= edgesSectorCountAtLastRing;
                    return (edgeIndex - 1) % 3 == 0;
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            return false;
        }
    }

    private void FormAuxiliaryArrays()
    {
        pointIndexArray = new int[6];
        pointIndexOppositeArray = new int[6];
        edgeIndexArray = new int[6];
        edgeIndexOppositeArray = new int[6];

        pointIndexArray[0] = pointsBeforeLastRing;
        pointIndexOppositeArray[0] = pointIndexArray[0] + hexagonRadius;
        edgeIndexArray[0] = pointsCount + edgesBeforeLastRing + 1;
        edgeIndexOppositeArray[0] = pointsCount + edgesBeforeLastRing + edgesSectorCountAtLastRing - 1;
        for (int i = 1; i < 6; ++i)
        {
            pointIndexArray[i] = pointIndexOppositeArray[i - 1];
            pointIndexOppositeArray[i] = pointIndexArray[i] + hexagonRadius;
            edgeIndexArray[i] = edgeIndexArray[i - 1] + edgesSectorCountAtLastRing;
            edgeIndexOppositeArray[i] = edgeIndexOppositeArray[i - 1] + edgesSectorCountAtLastRing;
        }
    }
}