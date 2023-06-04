using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Obstacle Detector Settings", menuName = "ScriptableObjects/Obstacle Detector Settings", order = 1)]
public class ObstacleDetectorSettings: ScriptableObject
{
    [field: SerializeField] public bool UseRange { get; private set; }
    [field: SerializeField] public Vector2 DepthRange { get; private set; }
    [field: SerializeField, Min(0f)] public float MaxAllowedDistance { get; private set; }
    [field: SerializeField, Range(0f, 90f)] public float MaxAllowedAngleWithUpAxis { get; private set; }
}
