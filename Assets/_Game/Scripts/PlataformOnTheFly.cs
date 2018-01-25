using UnityEngine;

public class PlataformOnTheFly : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
            SerialController.Instance.Recalibrate();

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            if (GameMaster.Instance.GameIsPaused)
            {
                FindObjectOfType<PauseMenuUI>().Hide();
                GameMaster.Instance.UnPauseGame();
            }
            else
            {
                FindObjectOfType<PauseMenuUI>().Show();
                GameMaster.Instance.PauseGame();
            }
        }
    }
}
