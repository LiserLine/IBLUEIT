using Ibit.Core.Game;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ibit.Core.Util
{
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

        
        /// <summary>
        /// Converts m³/s to L/min
        /// </summary>
        private const float LitresPerMinuteConverter = 60000;

        /* Pitaco Measures */
        private const float tubeRadius = 0.016f;
        private const float tubeLenght = 0.2f;
        private const float airViscosity = 18.2f * 0.000001f;

        /// <summary>
        /// Returns the volumetric flow of air in Cubic Meter / Second
        /// </summary>
        /// <param name="differentialPressure">Pressure difference in Pascal (Pa)</param>
        /// <returns></returns>
        private static float Poiseulle(float differentialPressure) =>
            differentialPressure * Mathf.PI * Mathf.Pow(tubeRadius, 4) / (8 * airViscosity * tubeLenght);

        /// <summary>
        /// Returns the volumetric flow of air in Litres/Minute
        /// </summary>
        /// <param name="differentialPressure">Pressure difference in Pascal (Pa)</param>
        /// <returns></returns>
        public static float ToLitresPerMinute(float differentialPressure) => Poiseulle(differentialPressure / 1000f) * LitresPerMinuteConverter;
    }
}