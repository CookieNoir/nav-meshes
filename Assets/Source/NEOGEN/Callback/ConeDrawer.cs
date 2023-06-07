using UnityEngine;

public class ConeDrawer : MonoBehaviour
{
    [SerializeField] private Vector3 _coneOrigin;
    [SerializeField] private Vector3 _coneLeft;
    [SerializeField] private Vector3 _coneRight;
    [SerializeField] private float _length;

    private void OnValidate()
    {
        Vector3 fromPos = 2 * _coneOrigin - _coneLeft;
        Vector3 toPos = 2 * _coneOrigin - _coneRight;
        Debug.Log($"{fromPos} {toPos} {ANavMG.Sign(fromPos, toPos, _coneOrigin)}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_coneOrigin, 0.1f);
        Gizmos.DrawSphere(_coneLeft, 0.1f);
        Gizmos.DrawSphere(_coneRight, 0.1f);
        Vector3 leftDirection = _coneLeft - _coneOrigin;
        Gizmos.DrawLine(_coneOrigin, _coneOrigin + _length * leftDirection.normalized);
        Vector3 rightDirection = _coneRight - _coneOrigin;
        Gizmos.DrawLine(_coneOrigin, _coneOrigin + _length * rightDirection.normalized);
    }
}
