using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
public class HexagonContent : MonoBehaviour
{
    public float[] Values { get; set; }
    public HexagonRelaxationData relaxationData;
    public HexagonMeshCreator meshCreator;

    private void Awake()
    {
        meshCreator = new HexagonMeshCreator(GetComponent<MeshFilter>());
        relaxationData = new HexagonRelaxationData(meshCreator);
    }

    public void AddObjectToHexagon(Transform newObject, Vector3 localPosition)
    {
        newObject.parent = transform;
        newObject.transform.localPosition = localPosition;
    }

    public void DestroyChildren()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        meshCreator?.GizmosDrawQuads(transform);
    }
}