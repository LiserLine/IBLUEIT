using UnityEngine;

public class OnTheFlyInputs : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            if (!GameManager.GameIsPaused)
                FindObjectOfType<CanvasManager_P>().PauseGame();
            else
                FindObjectOfType<CanvasManager_P>().UnPauseGame();
        }

        if (Input.GetKeyDown(KeyCode.F2))
            FindObjectOfType<SerialController>().Recalibrate();
    }
}
