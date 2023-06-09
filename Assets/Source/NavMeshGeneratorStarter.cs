using System;
using UnityEngine;
using UnityEngine.Events;

public class NavMeshGeneratorStarter : MonoBehaviour
{
    [SerializeField] private NavMeshGenerator _navMeshGenerator;
    [SerializeField] private GameObject[] _gameObjects;
    [SerializeField, Min(1)] private int _iterations = 1;
    public UnityEvent<CellAndPortalGraph> OnGraphReceived;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            OnGraphReceived.Invoke(_navMeshGenerator.Generate(_gameObjects));
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Test();
        }
    }

    private void Test()
    {
        DateTime startTime = DateTime.Now;
        for (int i = 0; i < _iterations; ++i)
        {
            _navMeshGenerator.Generate(_gameObjects);
        }
        DateTime endTime = DateTime.Now;
        Debug.Log($"Mean time - {(endTime - startTime).TotalSeconds / _iterations}");
    }
}
