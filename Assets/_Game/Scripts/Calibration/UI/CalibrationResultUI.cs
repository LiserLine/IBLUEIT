using Ibit.Core.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Ibit.Calibration.UI
{
    public class CalibrationResultUI : MonoBehaviour
    {
        [SerializeField]
        private Text resultText;

        private void OnEnable()
        {
            resultText.text = $"Pico Exp.: {Pacient.Loaded.Capacities.RawExpPeakFlow} Pa\n" +
                              $"Pico Ins.: {Pacient.Loaded.Capacities.RawInsPeakFlow} Pa\n" +
                              $"Tempo Exp.: {Pacient.Loaded.Capacities.RawExpFlowDuration / 1000f:F1} s\n" +
                              $"Tempo Ins.: {Pacient.Loaded.Capacities.RawInsFlowDuration / 1000f:F1} s\n" +
                              $"Freq. Resp. Média: {Pacient.Loaded.Capacities.RawRespCycleDuration / 1000f:F1} s";
        }
    }
}