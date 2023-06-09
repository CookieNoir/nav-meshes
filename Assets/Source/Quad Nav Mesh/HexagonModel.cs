using UnityEngine;

public class HexagonModel
{
    public Vector3 Position { get; private set; }
    public HexagonRelaxationData RelaxationData { get; private set; }
    public HexagonMeshCreator MeshCreator { get; private set; }
    public HexagonModel[] Neighbors { get; private set; }

    public bool IsNeighbor(HexagonModel hexagonModel)
    {
        for (int i = 0; i < 6; ++i)
        {
            if (Neighbors[i] == hexagonModel) { return true; }
        }
        return false;
    }

    public HexagonModel(Vector3 position)
    {
        Position = position;
        MeshCreator = new HexagonMeshCreator();
        RelaxationData = new HexagonRelaxationData(MeshCreator);
        Neighbors = new HexagonModel[6];
    }

    public void GizmosDrawModel()
    {
        Gizmos.color = Color.black;
        Vector3[] vertices = MeshCreator.Vertices;
        int[] quads = MeshCreator.Quads;
        for (int i = 0; i < quads.Length; i += 4)
        {
            Gizmos.DrawLine(Position + vertices[quads[i]], Position + vertices[quads[i + 1]]);
            Gizmos.DrawLine(Position + vertices[quads[i + 1]], Position + vertices[quads[i + 2]]);
            Gizmos.DrawLine(Position + vertices[quads[i + 2]], Position + vertices[quads[i + 3]]);
            Gizmos.DrawLine(Position + vertices[quads[i + 3]], Position + vertices[quads[i]]);
        }
    }
}
