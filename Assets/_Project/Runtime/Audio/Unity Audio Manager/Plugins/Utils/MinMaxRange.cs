using System;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class MinMaxRange<T>
{
    public T RangeMin, RangeMax;

    public abstract T Value { get; }

    /// <summary>
    /// Returns a random value between [rangeStart, rangeEnd] using the provided random function.
    /// </summary>
    /// <param name="MinMaxInclusiveRandomFunction">Inclusive random function that takes a minimum and a maximum value as parameters.</param>
    protected T GetRandomValue(System.Func<T, T, T> MinMaxInclusiveRandomFunction)
    {
        return MinMaxInclusiveRandomFunction(RangeMin, RangeMax);
    }    
    
    public abstract T RangeMiddle { get; }

    public abstract bool IsInRange(T value);
}

[Serializable]
public class MinMaxRange : MinMaxRange<float>
{
    public override float Value => GetRandomValue((min, max) => Mathf.Lerp(min, max, Random.value));

    public override float RangeMiddle => RangeMin + ((RangeMax - RangeMin) * 0.5f);

    public override bool IsInRange(float value)
    {
        return !((value > RangeMax) || (value < RangeMin));
    }
}