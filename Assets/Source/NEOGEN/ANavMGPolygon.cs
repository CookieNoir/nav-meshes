using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ANavMGPolygon: NavMeshPolygon
{
    public ANavMGPolygon(List<Vector3> vertices): base(vertices)
    {

    }

    public override void Simplify(float threshold)
    {
        bool[] isRemoved = new bool[Vertices.Length];
        float thresholdSquared = threshold * threshold;

        int midPoint = Vertices.Length / 2;
        RamerDouglasPeucker.SimplifyPartial(Vertices, isRemoved, thresholdSquared, 0, midPoint);
        RamerDouglasPeucker.SimplifyPartial(Vertices, isRemoved, thresholdSquared, midPoint, Vertices.Length);

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
}
