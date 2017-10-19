using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public static class GameUtilities
{
    public static void WriteAllText(string filepath, string contents)
    {
        var directory = Path.GetDirectoryName(filepath);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(filepath, contents, Encoding.UTF8);

        Debug.LogFormat("File saved: {0}", filepath);
    }

    public static string ReadAllText(string filepath)
    {
        return File.ReadAllText(filepath, Encoding.UTF8);
    }

    public static float ParseFloat(string value) => float.Parse(value.Replace('.', ','));

    public static float CalculateMeanFlow(List<KeyValuePair<long, float>> respiratorySamples)
    {
        long startTime = 0, firstCurveTime = 0, secondCurveTime = 0, sumTimes = 0;
        float quantCycles = 0;

        for (var i = 1; i < respiratorySamples.Count; i++)
        {
            var actualTime = respiratorySamples[i].Key;
            var actualValue = respiratorySamples[i].Value;

            var lastTime = respiratorySamples[i - 1].Key;

            if (actualValue < -GameConstants.PitacoThreshold || actualValue > GameConstants.PitacoThreshold)
            {
                if (startTime == 0)
                {
                    startTime = lastTime;
                }
            }
            else
            {
                if (startTime == 0)
                    continue;

                if (firstCurveTime == 0)
                {
                    firstCurveTime = actualTime - startTime;
                }
                else if (secondCurveTime == 0)
                {
                    secondCurveTime = actualTime - startTime;
                }

                startTime = 0;
            }

            if (firstCurveTime == 0 || secondCurveTime == 0) continue;

            var cycleTime = firstCurveTime + secondCurveTime;
            sumTimes += cycleTime;
            quantCycles++;
            firstCurveTime = 0;
            secondCurveTime = 0;
        }

        return sumTimes / quantCycles;
    }
}
