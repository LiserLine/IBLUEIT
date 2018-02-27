using UnityEngine;
using Ibit.Core.Util;

namespace Ibit.Calibration
{
    public partial class CalibrationManager
    {
        private void OnSerialMessageReceived(string msg)
        {
            if (!acceptingValues || msg.Length < 1)
                return;

            var tmp = Parsers.Float(msg);

            switch (currentExercise)
            {
                case CalibrationExercise.ExpiratoryPeak:
                    if (tmp > flowMeter)
                    {
                        flowMeter = tmp;

                        if (flowMeter > newCapacities.ExpPeakFlow)
                        {
                            newCapacities.ExpPeakFlow = flowMeter;
                            Debug.Log($"ExpPeakFlow: {flowMeter}");
                        }
                    }
                    break;

                case CalibrationExercise.InspiratoryPeak:
                    if (tmp < flowMeter)
                    {
                        flowMeter = tmp;

                        if (flowMeter < newCapacities.InsPeakFlow)
                        {
                            newCapacities.InsPeakFlow = flowMeter;
                            Debug.Log($"InsPeakFlow: {flowMeter}");
                        }
                    }
                    break;

                case CalibrationExercise.ExpiratoryDuration:
                case CalibrationExercise.InspiratoryDuration:
                    flowMeter = tmp;
                    break;

                case CalibrationExercise.RespiratoryFrequency:
                    if (flowWatch.IsRunning)
                        capturedSamples.Add(flowWatch.ElapsedMilliseconds, tmp);
                    break;
            }
        }
    }
}