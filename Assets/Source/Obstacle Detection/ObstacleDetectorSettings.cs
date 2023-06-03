using System;
using UnityEngine;

[Serializable]
public class ObstacleDetectorSettings
{
    [field: SerializeField] public bool UseRange { get; private set; }
    [field: SerializeField] public Vector2 DepthRange { get; private set; }
    [field: SerializeField, Min(0f)] public float MaxAllowedDistance { get; private set; }
    [field: SerializeField, Range(0f, 90f)] public float MaxAllowedAngleWithUpAxis { get; private set; }
}
