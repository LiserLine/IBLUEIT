using UnityEngine;

public class GameUtilities
{
    public static float ParseFloat(string value) => float.Parse(value.Replace('.', ','));

    public static float ClipValue(float value, float min, float max)
    {
        if (value < min) { return min; }
        return value > max ? max : value;
    }
}
