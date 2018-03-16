using Ibit.Calibration;
using Ibit.Core.Audio;
using Ibit.Core.Data;
using Ibit.Core.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Ibit.MainMenu.UI.Canvas
{
    public partial class CanvasManager : MonoBehaviour
    {
        public void ShowPlayerInfo()
        {
            SysMessage.Info($"Jogador: {Pacient.Loaded.Name}\n" +
                            $"Condição: {Pacient.Loaded.Condition}\n" +
                            $"Partidas Jogadas: {Pacient.Loaded.PlaySessionsDone}\n" +
                            $"Pico Exp.: {FlowMath.ToLitresPerMinute(Pacient.Loaded.Capacities.RawExpPeakFlow)} L/min ({Pacient.Loaded.Capacities.RawExpPeakFlow} Pa)\n" +
                            $"Pico Ins.: {FlowMath.ToLitresPerMinute(Pacient.Loaded.Capacities.RawInsPeakFlow)} L/min ({Pacient.Loaded.Capacities.RawInsPeakFlow} Pa)\n" +
                            $"Tempo Ins.: {Pacient.Loaded.Capacities.RawInsFlowDuration / 1000f:F1} s\n" +
                            $"Tempo Exp.: {Pacient.Loaded.Capacities.RawExpFlowDuration / 1000f:F1} s\n" +
                            $"Tins/Texp: {((Pacient.Loaded.Capacities.RawInsFlowDuration / 1000f) / (Pacient.Loaded.Capacities.RawExpFlowDuration / 1000f)):F1}\n" +
                            $"Freq. Resp. Média: {Pacient.Loaded.Capacities.RawRespCycleDuration / 1000f:F1} sec/cycle");
        }

        private void AddClickSfxToButtons()
        {
            foreach (var component in GameObject.Find("Canvas").GetComponentsInChildren(typeof(Button), true))
            {
                var btn = (Button)component;
                btn.onClick.AddListener(PlayClick);
            }
        }

        private void Awake()
        {
            if (Pacient.Loaded == null)
                return;

            GameObject.Find("Canvas").transform.Find("Start Panel").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("Player Menu").gameObject.SetActive(true);
        }

        private void PlayClick() => SoundManager.Instance.PlaySound("BtnClickUI");

        private void Start() => AddClickSfxToButtons();

        public void SetCalibrationToRespCycle() => CalibrationManager.CalibrationToLoad = CalibrationExercise.RespiratoryFrequency;
        public void SetCalibrationToExpPeak() => CalibrationManager.CalibrationToLoad = CalibrationExercise.ExpiratoryPeak;
        public void SetCalibrationToInsPeak() => CalibrationManager.CalibrationToLoad = CalibrationExercise.InspiratoryPeak;
        public void SetCalibrationToExpDur() => CalibrationManager.CalibrationToLoad = CalibrationExercise.ExpiratoryDuration;
        public void SetCalibrationToInsDur() => CalibrationManager.CalibrationToLoad = CalibrationExercise.InspiratoryDuration;
    }
}