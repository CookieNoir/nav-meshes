using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonModelsDrawer : MonoBehaviour
{
    private List<HexagonModel> _hexagonModels;

    public void Draw(List<HexagonModel> hexagonModels)
    {
        _hexagonModels = hexagonModels;
    }

    private void OnDrawGizmos()
    {
        if (_hexagonModels == null) { return; }

        Gizmos.color = Color.black;
        foreach (var model in _hexagonModels)
        {
            model.GizmosDrawModel();
        }
    }
}
