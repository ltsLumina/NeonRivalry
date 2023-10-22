using UnityEngine;

public static class AnimationCurveExtensions
{
    public static float EvaluateInverse(this AnimationCurve curve, float value, int accuracy = 15)
    {
        if (curve.keys.Length == 0)
        {
            Debug.LogError("Null curve given!");
            return value;
        }

        float startTime = curve.keys[0].time;
        float endTime = curve.keys[curve.length - 1].time;
        float nearestTime = startTime;
        float step = endTime - startTime;

        for (int i = 0; i < accuracy; i++)
        {

            float valueAtNearestTime = curve.Evaluate(nearestTime);
            float distanceToValueAtNearestTime = Mathf.Abs(value - valueAtNearestTime);

            float timeToCompare = nearestTime + step;
            float valueAtTimeToCompare = curve.Evaluate(timeToCompare);
            float distanceToValueAtTimeToCompare = Mathf.Abs(value - valueAtTimeToCompare);

            if (distanceToValueAtTimeToCompare < distanceToValueAtNearestTime)
            {
                nearestTime = timeToCompare;
                valueAtNearestTime = valueAtTimeToCompare;
            }

            step = Mathf.Abs(step * 0.5f) * Mathf.Sign(value - valueAtNearestTime);
        }

        return nearestTime;
    }
}