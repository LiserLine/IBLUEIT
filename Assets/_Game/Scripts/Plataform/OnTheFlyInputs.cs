﻿using Ibit.Core.Game;
using UnityEngine;
using Ibit.Plataform.UI;

namespace Ibit.Plataform
{
    public class OnTheFlyInputs : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
            {
                if (!GameManager.GameIsPaused)
                    FindObjectOfType<CanvasManager>().PauseGame();
                else
                    FindObjectOfType<CanvasManager>().UnPauseGame();
            }

            if (Input.GetKeyDown(KeyCode.F2))
                FindObjectOfType<Ibit.Core.Serial.SerialController>().Recalibrate();
        }
    }
}