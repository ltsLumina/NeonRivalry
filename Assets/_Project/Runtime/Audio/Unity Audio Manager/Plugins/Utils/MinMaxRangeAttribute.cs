using UnityEngine;

public class MinMaxRangeAttribute : PropertyAttribute
{
    public readonly float MinLimit;
    public readonly float MaxLimit;

    public MinMaxRangeAttribute(float minLimit, float maxLimit)
    {
        MinLimit = minLimit;
        MaxLimit = maxLimit;
    }
}