using System.Collections.Generic;
using UnityEngine;

public struct ReducedLayersData
{
    public Vector3[,] Positions;
    public List<ReducedLayer> ReducedLayers;

    public ReducedLayersData(Vector3[,] positions, List<ReducedLayer> reducedLayers)
    {
        Positions = positions;
        ReducedLayers = reducedLayers;
    }
}
