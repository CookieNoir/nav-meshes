using System.Collections.Generic;
using UnityEngine;

public static class MeshDataReceiver
{
    public static List<MeshData> GetMeshDatas(ICollection<GameObject> gameObjects)
    {
        List<MeshData> meshDatas = new List<MeshData>();
        foreach (GameObject parent in gameObjects)
        {
            GetMeshDatasRecursively(parent.transform, meshDatas);
        }
        return meshDatas;
    }

    private static void GetMeshDatasRecursively(Transform parent, List<MeshData> meshDatas)   
    {
        MeshFilter meshFilter = parent.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = parent.GetComponent<MeshRenderer>();
        if (meshFilter && meshRenderer)
        {
            meshDatas.Add(new MeshData(meshFilter, meshRenderer));
        }
        foreach (Transform child in parent)
        {
            GetMeshDatasRecursively(child, meshDatas);
        }
    }
}
