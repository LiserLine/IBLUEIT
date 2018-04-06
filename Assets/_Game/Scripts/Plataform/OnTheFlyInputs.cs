using System;
using Ibit.Core.Audio;
using Ibit.Core.Game;
using Ibit.Core.Serial;
using Ibit.Plataform.Manager.Spawn;
using Ibit.Plataform.Manager.Stage;
using Ibit.Plataform.UI;
using UnityEngine;

namespace Ibit.Plataform
{
    public class OnTheFlyInputs : MonoBehaviour
    {
        [SerializeField]
        private GameObject _helpPanel;

        private void Update()
        {
            // ESC - SPACE
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
            {
                if (FindObjectOfType<StageManager>().IsRunning)
                {
                    if (!GameManager.GameIsPaused)
                        FindObjectOfType<CanvasManager>().PauseGame();
                    else
                        FindObjectOfType<CanvasManager>().UnPauseGame();
                }
            }

            // F1
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ShowHelp();
            }

            // F2
            if (Input.GetKeyDown(KeyCode.F2))
            {
                FindObjectOfType<SerialController>().Recalibrate();
            }

            // S
            if (Input.GetKeyDown(KeyCode.S))
            {
                ToggleSound();
            }

            // T
            if (Input.GetKeyDown(KeyCode.T))
            {
                ChangeMusic();
            }

            // +
            if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                IncreaseGamingFactors();
            }

            // -
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                DecreaseGamingFactors();
            }

            // ←
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                IncreaseSpeedFactor();
            }

            // →
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                DecreaseSpeedFactor();
            }
        }

        private void ShowHelp()
        {
            _helpPanel.SetActive(!_helpPanel.activeSelf);
        }

        private void ToggleSound()
        {
            if (AudioListener.volume > 0f)
                AudioListener.volume = 0f;
            else
                AudioListener.volume = 0.8f;
        }

        private void ChangeMusic()
        {
            SoundManager.Instance.PlayAnotherBgm();
        }

        private void IncreaseGamingFactors()
        {
            var spwn = FindObjectOfType<Spawner>();
            spwn.IncrementExpHeightAcc();
            spwn.IncrementExpSizeAcc();
            spwn.IncrementInsHeightAcc();
            spwn.IncrementInsSizeAcc();
        }

        private void DecreaseGamingFactors()
        {
            var spwn = FindObjectOfType<Spawner>();
            spwn.DecrementExpHeightAcc();
            spwn.DecrementExpSizeAcc();
            spwn.DecrementInsHeightAcc();
            spwn.DecrementInsSizeAcc();
        }

        private void IncreaseSpeedFactor()
        {
            Data.Stage.Loaded.ObjectSpeedMultiplier *= 1.05f;

            foreach (var obj in FindObjectOfType<Spawner>().SpawnedObjects)
            {
                obj.GetComponent<MoveObject>().Speed *= 1.05f;
            }
        }

        private void DecreaseSpeedFactor()
        {
            Data.Stage.Loaded.ObjectSpeedMultiplier *= 0.95f;

            foreach (var obj in FindObjectOfType<Spawner>().SpawnedObjects)
            {
                obj.GetComponent<MoveObject>().Speed *= 0.95f;
            }
        }
    }
}