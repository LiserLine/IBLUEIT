using Ibit.Calibration;
using Ibit.Core.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Ibit.MainMenu.UI
{
    public class CalibrationSelectButton : MonoBehaviour
    {
        [SerializeField]
        private CalibrationExercise _calibrationToLoad;

        private const string _checkmark = "  ✓";

        private void Start()
        {
            this.GetComponent<Button>().onClick.AddListener(SetExercise);

            switch (_calibrationToLoad)
            {
                case CalibrationExercise.RespiratoryFrequency:
                    if (Pacient.Loaded.Capacities.RespiratoryRate != 0)
                    {
                        CheckExercise();
                    }
                    break;
                case CalibrationExercise.InspiratoryPeak:
                    if (Pacient.Loaded.Capacities.InsPeakFlow != 0)
                    {
                        CheckExercise();
                    }
                    break;
                case CalibrationExercise.InspiratoryDuration:
                    if (Pacient.Loaded.Capacities.InsFlowDuration != 0)
                    {
                        CheckExercise();
                    }
                    break;
                case CalibrationExercise.ExpiratoryPeak:
                    if (Pacient.Loaded.Capacities.ExpPeakFlow != 0)
                    {
                        CheckExercise();
                    }
                    break;
                case CalibrationExercise.ExpiratoryDuration:
                    if (Pacient.Loaded.Capacities.ExpFlowDuration != 0)
                    {
                        CheckExercise();
                    }
                    break;
            }
        }

        private void CheckExercise()
        {
            GetComponentInChildren<Text>().text += _checkmark;
        }

        private void SetExercise()
        {
            CalibrationManager.CalibrationToLoad = _calibrationToLoad;
        }
    }
}
