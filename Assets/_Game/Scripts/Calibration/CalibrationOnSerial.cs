using UnityEngine;

public partial class CalibrationManager
{
    private void OnSerialMessageReceived(string msg)
    {
        if (!acceptingValues || msg.Length < 1)
            return;

        var tmp = Utils.ParseFloat(msg);

        switch (currentExercise)
        {
            case CalibrationExercise.ExpiratoryPeak:
                if (tmp > flowMeter)
                {
                    flowMeter = tmp;

                    if (flowMeter > tempRespiratoryInfo.ExpiratoryPeakFlow)
                    {
                        tempRespiratoryInfo.ExpiratoryPeakFlow = flowMeter;
                        Debug.Log($"ExpiratoryPeakFlow: {flowMeter}");
                    }
                }
                break;

            case CalibrationExercise.InspiratoryPeak:
                if (tmp < flowMeter)
                {
                    flowMeter = tmp;

                    if (flowMeter < tempRespiratoryInfo.InspiratoryPeakFlow)
                    {
                        tempRespiratoryInfo.InspiratoryPeakFlow = flowMeter;
                        Debug.Log($"InspiratoryPeakFlow: {flowMeter}");
                    }
                }
                break;

            case CalibrationExercise.ExpiratoryFlow:
            case CalibrationExercise.InspiratoryFlow:
                flowMeter = tmp;
                break;

            case CalibrationExercise.RespiratoryFrequency:
                if (flowWatch.IsRunning)
                    samples.Add(flowWatch.ElapsedMilliseconds, tmp);
                break;
        }
    }
}