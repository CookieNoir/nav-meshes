using System;
using System.Linq;
using UnityEngine;

public class HexagonMeshCreator
{
    private Mesh mesh;
    private MeshFilter meshFilter;
    public Vector3[] Vertices { get; private set; }
    public int[] Quads { get; private set; }
    public int[] TrianglesCentersIndices { get; private set; }
    private bool uvSet = false;
    private bool useUnityMesh;
    public bool IsRelaxed { get; private set; }

    public HexagonMeshCreator(MeshFilter meshFilter)
    {
        this.meshFilter = meshFilter;
        mesh = new Mesh();
        useUnityMesh = true;
        CreateMesh();
    }

    public HexagonMeshCreator()
    {
        useUnityMesh = false;
        CreateMesh();
    }

    private void CreateMesh()
    {
        int trianglesCount = UniversalHexagon.trianglesCount;
        int trianglesPointsCount = trianglesCount * 3;
        int[] pairs = Enumerable.Repeat(-2, trianglesCount).ToArray();

        int i = trianglesCount;
        int trianglesNeighborsPointIndex = UnityEngine.Random.Range(0, trianglesPointsCount),
            randomTriangle = trianglesNeighborsPointIndex / 3,
            neighborIndex = trianglesNeighborsPointIndex % 3,
            triangleNeighbor = UniversalHexagon.trianglesNeighbors[trianglesNeighborsPointIndex];

        int quadsCount = 0;
        while (i > 0)
        {
            if (pairs[randomTriangle] < -1)
            {
                if (triangleNeighbor > -1 && pairs[triangleNeighbor] < -1)
                {
                    pairs[randomTriangle] = trianglesNeighborsPointIndex;
                    pairs[triangleNeighbor] = triangleNeighbor * 3 + neighborIndex;
                    quadsCount++;
                    i -= 2;
                }
                else
                {
                    neighborIndex = (trianglesNeighborsPointIndex + 1) % 3;
                    trianglesNeighborsPointIndex = randomTriangle * 3 + neighborIndex;
                    triangleNeighbor = UniversalHexagon.trianglesNeighbors[trianglesNeighborsPointIndex];
                    if (triangleNeighbor > -1 && pairs[triangleNeighbor] < -1)
                    {
                        pairs[randomTriangle] = trianglesNeighborsPointIndex;
                        pairs[triangleNeighbor] = triangleNeighbor * 3 + neighborIndex;
                        quadsCount++;
                        i -= 2;
                    }
                    else
                    {
                        neighborIndex = (trianglesNeighborsPointIndex + 1) % 3;
                        trianglesNeighborsPointIndex = randomTriangle * 3 + neighborIndex;
                        triangleNeighbor = UniversalHexagon.trianglesNeighbors[trianglesNeighborsPointIndex];
                        if (triangleNeighbor > -1 && pairs[triangleNeighbor] < -1)
                        {
                            pairs[randomTriangle] = trianglesNeighborsPointIndex;
                            pairs[triangleNeighbor] = triangleNeighbor * 3 + neighborIndex;
                            quadsCount++;
                            i -= 2;
                        }
                        else
                        {
                            pairs[randomTriangle] = -1;
                            i -= 1;
                        }
                    }
                }
                trianglesNeighborsPointIndex = (trianglesNeighborsPointIndex + UnityEngine.Random.Range(0, trianglesPointsCount)) % trianglesPointsCount;
            }
            else
            {
                trianglesNeighborsPointIndex = (trianglesNeighborsPointIndex + 3) % trianglesPointsCount;
            }
            randomTriangle = trianglesNeighborsPointIndex / 3;
            neighborIndex = trianglesNeighborsPointIndex % 3;
            triangleNeighbor = UniversalHexagon.trianglesNeighbors[trianglesNeighborsPointIndex];
        }
        int trianglesCentersCount = trianglesCount - quadsCount - quadsCount;
        Vertices = new Vector3[UniversalHexagon.pointsAndEdgesCount + trianglesCentersCount];
        Array.Copy(UniversalHexagon.pointsAndEdgesCenters, Vertices, UniversalHexagon.pointsAndEdgesCount);
        int subdividedQuadsCount = quadsCount * 4 + trianglesCentersCount * 3;
        Quads = new int[4 * subdividedQuadsCount];
        TrianglesCentersIndices = new int[trianglesCentersCount];
        int[] triangles = new int[6 * subdividedQuadsCount];
        int quadPointIndex = 0, trianglePointIndex = 0;
        int pointIndex0 = -1, pointIndex1 = -1, pointIndex2 = -1, pointIndex3 = -1;
        int edgeIndex0 = -1, edgeIndex1 = -1, edgeIndex2 = -1, edgeIndex3 = -1, edgeIndexQuadCenter = -1;
        int triangleCenterIndex = 0;
        int triangleCenterIndexWithOffset = UniversalHexagon.pointsAndEdgesCount;
        int i3;
        for (; i < trianglesCount; ++i)
        {
            i3 = i * 3;
            if (pairs[i] == -1)
            {
                Vertices[triangleCenterIndexWithOffset] = UniversalHexagon.trianglesCenters[i];
                TrianglesCentersIndices[triangleCenterIndex] = i;
                UniversalHexagon.GetTrianglePointsIndices(i3, ref pointIndex0, ref pointIndex1, ref pointIndex2);
                UniversalHexagon.GetEdgesIndicesWithPointOffset(i3, ref edgeIndex0, ref edgeIndex1, ref edgeIndex2);

                Quads[quadPointIndex] = pointIndex0;
                Quads[quadPointIndex + 1] = edgeIndex1;
                Quads[quadPointIndex + 2] = triangleCenterIndexWithOffset;
                Quads[quadPointIndex + 3] = edgeIndex2;

                triangles[trianglePointIndex] = triangleCenterIndexWithOffset;
                triangles[trianglePointIndex + 1] = pointIndex0;
                triangles[trianglePointIndex + 2] = edgeIndex1;
                triangles[trianglePointIndex + 3] = pointIndex0;
                triangles[trianglePointIndex + 4] = triangleCenterIndexWithOffset;
                triangles[trianglePointIndex + 5] = edgeIndex2;

                Quads[quadPointIndex + 4] = pointIndex1;
                Quads[quadPointIndex + 5] = edgeIndex2;
                Quads[quadPointIndex + 6] = triangleCenterIndexWithOffset;
                Quads[quadPointIndex + 7] = edgeIndex0;

                triangles[trianglePointIndex + 6] = triangleCenterIndexWithOffset;
                triangles[trianglePointIndex + 7] = pointIndex1;
                triangles[trianglePointIndex + 8] = edgeIndex2;
                triangles[trianglePointIndex + 9] = pointIndex1;
                triangles[trianglePointIndex + 10] = triangleCenterIndexWithOffset;
                triangles[trianglePointIndex + 11] = edgeIndex0;

                Quads[quadPointIndex + 8] = pointIndex2;
                Quads[quadPointIndex + 9] = edgeIndex0;
                Quads[quadPointIndex + 10] = triangleCenterIndexWithOffset;
                Quads[quadPointIndex + 11] = edgeIndex1;

                triangles[trianglePointIndex + 12] = triangleCenterIndexWithOffset;
                triangles[trianglePointIndex + 13] = pointIndex2;
                triangles[trianglePointIndex + 14] = edgeIndex0;
                triangles[trianglePointIndex + 15] = pointIndex2;
                triangles[trianglePointIndex + 16] = triangleCenterIndexWithOffset;
                triangles[trianglePointIndex + 17] = edgeIndex1;

                quadPointIndex += 12;
                trianglePointIndex += 18;
                triangleCenterIndex++;
                triangleCenterIndexWithOffset++;
            }
            else if (pairs[i] > -1)
            {
                triangleNeighbor = UniversalHexagon.trianglesNeighbors[pairs[i]];
                UniversalHexagon.GetQuadPointsAndEdgesIndicesWithPointOffset(i3, triangleNeighbor * 3, pairs[i] % 3,
                    ref pointIndex0, ref pointIndex1, ref pointIndex2, ref pointIndex3,
                    ref edgeIndex0, ref edgeIndex1, ref edgeIndex2, ref edgeIndex3, ref edgeIndexQuadCenter);

                pairs[i] = -2;
                pairs[triangleNeighbor] = -2;

                Quads[quadPointIndex] = pointIndex0;
                Quads[quadPointIndex + 1] = edgeIndex3;
                Quads[quadPointIndex + 2] = edgeIndexQuadCenter;
                Quads[quadPointIndex + 3] = edgeIndex0;

                triangles[trianglePointIndex] = pointIndex0;
                triangles[trianglePointIndex + 1] = edgeIndexQuadCenter;
                triangles[trianglePointIndex + 2] = edgeIndex0;
                triangles[trianglePointIndex + 3] = edgeIndexQuadCenter;
                triangles[trianglePointIndex + 4] = pointIndex0;
                triangles[trianglePointIndex + 5] = edgeIndex3;

                Quads[quadPointIndex + 4] = pointIndex1;
                Quads[quadPointIndex + 5] = edgeIndex0;
                Quads[quadPointIndex + 6] = edgeIndexQuadCenter;
                Quads[quadPointIndex + 7] = edgeIndex1;

                triangles[trianglePointIndex + 6] = pointIndex1;
                triangles[trianglePointIndex + 7] = edgeIndexQuadCenter;
                triangles[trianglePointIndex + 8] = edgeIndex1;
                triangles[trianglePointIndex + 9] = edgeIndexQuadCenter;
                triangles[trianglePointIndex + 10] = pointIndex1;
                triangles[trianglePointIndex + 11] = edgeIndex0;

                Quads[quadPointIndex + 8] = pointIndex2;
                Quads[quadPointIndex + 9] = edgeIndex1;
                Quads[quadPointIndex + 10] = edgeIndexQuadCenter;
                Quads[quadPointIndex + 11] = edgeIndex2;

                triangles[trianglePointIndex + 12] = pointIndex2;
                triangles[trianglePointIndex + 13] = edgeIndexQuadCenter;
                triangles[trianglePointIndex + 14] = edgeIndex2;
                triangles[trianglePointIndex + 15] = edgeIndexQuadCenter;
                triangles[trianglePointIndex + 16] = pointIndex2;
                triangles[trianglePointIndex + 17] = edgeIndex1;

                Quads[quadPointIndex + 12] = pointIndex3;
                Quads[quadPointIndex + 13] = edgeIndex2;
                Quads[quadPointIndex + 14] = edgeIndexQuadCenter;
                Quads[quadPointIndex + 15] = edgeIndex3;

                triangles[trianglePointIndex + 18] = pointIndex3;
                triangles[trianglePointIndex + 19] = edgeIndexQuadCenter;
                triangles[trianglePointIndex + 20] = edgeIndex3;
                triangles[trianglePointIndex + 21] = edgeIndexQuadCenter;
                triangles[trianglePointIndex + 22] = pointIndex3;
                triangles[trianglePointIndex + 23] = edgeIndex2;

                quadPointIndex += 16;
                trianglePointIndex += 24;
            }
        }
        if (useUnityMesh)
        {
            mesh.vertices = Vertices;
            mesh.normals = Enumerable.Repeat(Vector3.up, UniversalHexagon.pointsAndEdgesCount + trianglesCentersCount).ToArray();
            mesh.triangles = triangles;
            meshFilter.mesh = mesh;
        }
    }

    public void ApplyVerticesToMesh(Vector3[] newVertices)
    {
        Vertices = newVertices;
        IsRelaxed = true;
        if (useUnityMesh) { mesh.vertices = Vertices; }
    }

    public void SetUV(Vector3 worldOrigin)
    {
        if (!uvSet)
        {
            Vector2[] uv = new Vector2[Vertices.Length];
            for (int i = 0; i < uv.Length; ++i)
            {
                uv[i] = new Vector2(Vertices[i].x + worldOrigin.x, Vertices[i].z + worldOrigin.z);
            }
            mesh.uv = uv;
            uvSet = true;
        }
    }

    public void SetVertexColors(Color[] colors)
    {
        if (colors.Length == Vertices.Length)
            mesh.colors = colors;
    }

    public void DrawQuads(Transform transform)
    {
        if (uvSet)
        {
            int posPoint = UniversalHexagon.pointsBeforeLastRing, posEdge = UniversalHexagon.pointsCount + UniversalHexagon.edgesBeforeLastRing + 2;
            for (int i = 0; i < 5; ++i)
            {
                posEdge--;
                for (int j = 0; j < UniversalHexagon.hexagonRadius; ++j)
                {
                    Debug.DrawLine(transform.position + Vertices[posPoint], transform.position + Vertices[posEdge], Color.black);
                    posPoint++;
                    Debug.DrawLine(transform.position + Vertices[posPoint], transform.position + Vertices[posEdge], Color.black);
                    posEdge += 3;
                }
            }
            posEdge--;
            for (int j = 1; j < UniversalHexagon.hexagonRadius; ++j)
            {
                Debug.DrawLine(transform.position + Vertices[posPoint], transform.position + Vertices[posEdge], Color.black);
                posPoint++;
                Debug.DrawLine(transform.position + Vertices[posPoint], transform.position + Vertices[posEdge], Color.black);
                posEdge += 3;
            }
            Debug.DrawLine(transform.position + Vertices[UniversalHexagon.pointsBeforeLastRing], transform.position + Vertices[posEdge], Color.black);
        }
        else
        {
            for (int i = 0; i < Quads.Length; i += 4)
            {
                Debug.DrawLine(transform.position + Vertices[Quads[i]], transform.position + Vertices[Quads[i + 1]], Color.black);
                Debug.DrawLine(transform.position + Vertices[Quads[i + 1]], transform.position + Vertices[Quads[i + 2]], Color.black);
                Debug.DrawLine(transform.position + Vertices[Quads[i + 2]], transform.position + Vertices[Quads[i + 3]], Color.black);
                Debug.DrawLine(transform.position + Vertices[Quads[i + 3]], transform.position + Vertices[Quads[i]], Color.black);
            }
        }
    }

    public void GizmosDrawQuads(Transform transform)
    {
        Gizmos.color = Color.black;
        for (int i = 0; i < Quads.Length; i += 4)
        {
            Gizmos.DrawLine(transform.position + Vertices[Quads[i]], transform.position + Vertices[Quads[i + 1]]);
            Gizmos.DrawLine(transform.position + Vertices[Quads[i + 1]], transform.position + Vertices[Quads[i + 2]]);
            Gizmos.DrawLine(transform.position + Vertices[Quads[i + 2]], transform.position + Vertices[Quads[i + 3]]);
            Gizmos.DrawLine(transform.position + Vertices[Quads[i + 3]], transform.position + Vertices[Quads[i]]);
        }
    }
}