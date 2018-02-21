﻿using System.Collections.Generic;
using System.Linq;

public class FlowMath
{
    /// <summary>
    /// Calculates a mean of respiratory duration in Seconds per Cycle.
    /// </summary>
    /// <param name="data">Dictionary containing respiratory samples from PITACO.</param>
    public static float MeanFlow(Dictionary<long, float> data)
    {
        var samples = data.ToList();

        long startTime = 0, firstCurveTime = 0, secondCurveTime = 0, sumTimes = 0;
        float quantCycles = 0;

        for (var i = 1; i < samples.Count; i++)
        {
            var actualTime = samples[i].Key;
            var actualValue = samples[i].Value;

            var lastTime = samples[i - 1].Key;

            if (actualValue < -GameManager.PitacoFlowThreshold || actualValue > GameManager.PitacoFlowThreshold)
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

            if (firstCurveTime == 0 || secondCurveTime == 0)
                continue;

            var cycleTime = firstCurveTime + secondCurveTime;
            sumTimes += cycleTime;
            quantCycles++;
            firstCurveTime = 0;
            secondCurveTime = 0;
        }

        return sumTimes / quantCycles;
    }
}