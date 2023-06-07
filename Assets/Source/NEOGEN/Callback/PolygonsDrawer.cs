using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonsDrawer : MonoBehaviour
{
    [SerializeField] private List<Polygon> _polygons;
    [SerializeField] private Color[] _palette;

    public void SetPolygons(List<Polygon> polygons)
    {
        _polygons = polygons;
    }

    private void OnDrawGizmos()
    {
        if (_polygons == null) { return; }
        for (int p = 0; p < _polygons.Count; ++p)
        {
            if (_palette == null || _palette.Length == 0) { Gizmos.color = Color.green; }
            else { Gizmos.color = _palette[p % _palette.Length]; }
            Vector3[] vertices = _polygons[p].Vertices;
            for (int i = 0; i < vertices.Length; ++i)
            {
                Gizmos.DrawLine(vertices[i], vertices[(i + 1) % vertices.Length]);
            }
        }
    }
}
