using System;
using System.Collections.Generic;
using UnityEngine;

public class RecastPolygon : NavMeshPolygon, IComparable<RecastPolygon>
{
    [SerializeField] public int Index { get; private set; }
    public List<HashSet<int>> NeighborData { get; private set; }

    public RecastPolygon(int index, List<Vector3> vertices, List<HashSet<int>> neighborData) : base(vertices)
    {
        Index = index;
        NeighborData = neighborData;
    }

    public override void Simplify(float threshold)
    {
        bool[] isRemoved = new bool[Vertices.Length];
        float thresholdSquared = threshold * threshold;
        List<int> startIndices = new List<int>();

        int firstStartIndex = 0;
        int iterator = 1;
        while (iterator < Vertices.Length && NeighborData[iterator].SetEquals(NeighborData[firstStartIndex]))
        {
            iterator++;
        }
        if (iterator == Vertices.Length)
        {
            startIndices.Add(0);
            startIndices.Add(Vertices.Length / 2 + 1);
            startIndices.Add(Vertices.Length + 1);
        }
        else
        {
            firstStartIndex = iterator;

            startIndices.Add(iterator);
            int prevStartIndex = iterator % Vertices.Length;

            iterator++;
            while (iterator % Vertices.Length != firstStartIndex)
            {
                int iteratorClamped = iterator % Vertices.Length;
                if (!NeighborData[iteratorClamped].SetEquals(NeighborData[prevStartIndex]))
                {
                    startIndices.Add(iterator);
                    prevStartIndex = iteratorClamped;
                }
                iterator++;
            }
            startIndices.Add(iterator);
        }

        for (int i = 0; i < startIndices.Count - 1; ++i)
        {
            RamerDouglasPeucker.SimplifyPartial(Vertices, isRemoved, thresholdSquared, startIndices[i], startIndices[i + 1] - 1);
        }

        int survivedCount = Vertices.Length;
        for (int i = 0; i < isRemoved.Length; ++i)
        {
            if (isRemoved[i]) { survivedCount--; }
        }

        Vector3[] newVertices = new Vector3[survivedCount];
        int index = 0;
        for (int i = 0; i < Vertices.Length; ++i)
        {
            if (!isRemoved[i])
            {
                newVertices[index] = Vertices[i];
                index++;
            }
        }
        Vertices = newVertices;
    }

    public int CompareTo(RecastPolygon other)
    {
        return Index.CompareTo(other.Index);
    }
}
