using System.Collections.Generic;
using UnityEngine;

public class NavMeshCell
{
    public Vector3[] Vertices { get; private set; }
    public List<NavMeshCell> Neighbors { get; private set; }
    private Vector2 _lowerBound;
    private Vector2 _upperBound;

    public NavMeshCell(Vector3[] vertices)
    {
        Vertices = vertices;
        Neighbors = new List<NavMeshCell>();
        CalculateBounds();
    }

    private void CalculateBounds()
    {
        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;
        float minZ = float.PositiveInfinity;
        float maxZ = float.NegativeInfinity;
        for (int i = 0; i < Vertices.Length; ++i)
        {
            if (Vertices[i].x < minX) { minX = Vertices[i].x; }
            if (Vertices[i].x > maxX) { maxX = Vertices[i].x; }
            if (Vertices[i].z < minZ) { minZ = Vertices[i].z; }
            if (Vertices[i].z > maxZ) { maxZ = Vertices[i].z; }
        }
        _lowerBound = new Vector2(minX, minZ);
        _upperBound = new Vector2(maxX, maxZ);
    }

    public bool IsInsideBounds(Vector3 position)
    {
        return (position.x >= _lowerBound.x && position.x <= _upperBound.x &&
                position.z >= _lowerBound.y && position.z <= _upperBound.y);
    }
}
