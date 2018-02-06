using UnityEngine;
using UnityEngine.UI;

public class CalibrationResultUI : MonoBehaviour
{
    [SerializeField]
    private Text resultText;

    private void OnEnable()
    {
        resultText.text = $"Pico Exp.: {Pacient.Loaded.RespiratoryData.ExpiratoryPeakFlow} Pa\n" +
                          $"Pico Ins.: {Pacient.Loaded.RespiratoryData.InspiratoryPeakFlow} Pa\n" +
                          $"Tempo Exp.: {Pacient.Loaded.RespiratoryData.ExpiratoryFlowTime/1000f:F1}s\n" +
                          $"Tempo Ins.: {Pacient.Loaded.RespiratoryData.InspiratoryFlowTime/1000f:F1}s\n" +
                          $"Freq. Resp. Média: {Pacient.Loaded.RespiratoryData.RespiratoryFrequency/1000f:F1}s";
    }
}