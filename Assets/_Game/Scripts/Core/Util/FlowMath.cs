using System.Collections.Generic;

public class FlowMath
{
    public static float MeanFlow(List<KeyValuePair<long, float>> samples)
    {
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