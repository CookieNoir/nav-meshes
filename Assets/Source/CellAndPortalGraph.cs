using System.Collections.Generic;
using UnityEngine;

public class CellAndPortalGraph
{
    public List<NavMeshCell> Cells { get; private set; }

    public CellAndPortalGraph(List<NavMeshCell> cells)
    {
        Cells = cells;
    }
}
