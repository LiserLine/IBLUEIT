using Ibit.Core.Game;
using Ibit.Core.Serial;
using Ibit.Plataform.Manager.Stage;
using Ibit.Plataform.UI;
using UnityEngine;

namespace Ibit.Plataform
{
    public class OnTheFlyInputs : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
            {
                if (GetComponent<StageManager>().IsRunning)
                {
                    if (!GameManager.GameIsPaused)
                        FindObjectOfType<CanvasManager>().PauseGame();
                    else
                        FindObjectOfType<CanvasManager>().UnPauseGame();
                }
            }

            if (Input.GetKeyDown(KeyCode.F2))
                FindObjectOfType<SerialController>().Recalibrate();
        }
    }
}