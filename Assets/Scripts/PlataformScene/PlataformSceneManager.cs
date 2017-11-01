using UnityEngine;

public class PlataformSceneManager : MonoBehaviour
{
    private float _dt;

    private void Update()
    {
        if (GameManager.Instance.Stage != null && GameManager.Instance.Stage.IsRunning && SerialGetOffset.IsUsingOffset)
        {
            _dt += Time.deltaTime;
            if (_dt >= ((PlataformStage)GameManager.Instance.Stage).TimeLimit)
            {
                EndStage();
            }
        }
    }

    public void StartStage(int stageId)
    {
        GameManager.Instance.Stage = new PlataformStage { Id = stageId };
        GameManager.Instance.Stage.Start();
    }

    public void EndStage()
    {
        if (!GameManager.Instance.Stage.IsRunning)
            return;

        var sComm = GameObject.FindGameObjectWithTag("SerialController");
        Destroy(sComm);

        Debug.LogWarning("ToDo - write player session data"); // ToDo - write player session data

        GameManager.Instance.Stage.Stop();
        Debug.Log($"Stage {GameManager.Instance.Stage.Id} terminated.");
    }
}
