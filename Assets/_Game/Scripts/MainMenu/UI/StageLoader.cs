﻿using Ibit.Core.Data;
using Ibit.Core.Util;
using Ibit.Plataform.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Ibit.MainMenu.UI
{
    public class StageLoader : MonoBehaviour
    {
        public StageModel stage;

        private void OnEnable()
        {
            this.GetComponent<Button>().onClick.AddListener(OnStageSelected);
        }

        private void OnStageSelected()
        {
            if (!Pacient.Loaded.CalibrationDone)
            {
                SysMessage.Warning("Calibração não foi feita!");
                return;
            }

#if !UNITY_EDITOR

            if (!FindObjectOfType<Ibit.Core.Serial.SerialController>().IsConnected)
            {
                SysMessage.Warning("Pitaco não está conectado! Conecte antes de jogar!");
                return;
            }

#endif

            StageModel.Loaded = stage;
            FindObjectOfType<SceneLoader>().LoadScene(1);
            Debug.Log($"Stage {stage.Id} loaded.");
        }
    }
}