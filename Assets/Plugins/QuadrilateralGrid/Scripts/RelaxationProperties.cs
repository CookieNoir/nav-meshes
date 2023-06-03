
using UnityEngine;

public class RelaxationProperties : MonoBehaviour
{
    [Min(0)] public int _iterationsCount;
    [Min(0f)] public float _baseOffsetMultiplier;
    [Min(0f)] public float _offsetMultiplierPerIteration;
    public static int iterationsCount;
    public static float baseOffsetMultiplier;
    public static float offsetMultiplierPerIteration;

    public void ApplyProperties()
    {
        iterationsCount = _iterationsCount;
        baseOffsetMultiplier = _baseOffsetMultiplier;
        offsetMultiplierPerIteration = _offsetMultiplierPerIteration;
    }
}