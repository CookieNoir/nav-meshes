using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NavMeshPolygon
{
    [SerializeField] public Vector3[] Vertices { get; protected set; }

    public NavMeshPolygon(List<Vector3> vertices)
    {
        Vertices = vertices.ToArray();
    }

    public virtual void Simplify(float threshold) { }
}
