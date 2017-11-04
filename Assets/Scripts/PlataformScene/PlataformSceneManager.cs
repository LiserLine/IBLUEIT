using UnityEngine;

public class PlataformSceneManager : MonoBehaviour
{
    private float _dt;

    private void Update()
    {
        if (GameManager.Instance.Stage == null || !GameManager.Instance.Stage.IsRunning || !SerialGetOffset.IsUsingOffset)
            return;

        _dt += Time.deltaTime;

#if UNITY_EDITOR
        if (_dt <= 60)
            return;

        StopStageOnTimeLimit();
#else
            if (_dt <= ((PlataformStage)GameManager.Instance.Stage).TimeLimit)
                return;

            StopStageOnTimeLimit();
#endif

        _dt = 0;
    }

    public void StopStageOnTimeLimit()
    {
        if (!GameManager.Instance.Stage.IsRunning)
            return;

        var sComm = GameObject.FindGameObjectWithTag("SerialController");
        Destroy(sComm);

        GameManager.Instance.Stage.Stop();

        Debug.Log($"Stage {GameManager.Instance.Stage.Id} terminated.");
    }
}
