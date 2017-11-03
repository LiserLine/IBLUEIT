using UnityEngine;

public class PlataformSceneManager : MonoBehaviour
{
    private float _dt;

    private void Update()
    {
        if (GameManager.Instance.Stage != null && GameManager.Instance.Stage.IsRunning && SerialGetOffset.IsUsingOffset)
        {
            _dt += Time.deltaTime;

#if UNITY_EDITOR
            if (_dt >= 60)
            {
                StopStageOnTimeLimit();
            }
#else
            if (_dt >= ((PlataformStage)GameManager.Instance.Stage).TimeLimit)
            {
                StopStageOnTimeLimit();
            }
#endif

        }
    }

    public void StopStageOnTimeLimit()
    {
        if (!GameManager.Instance.Stage.IsRunning)
            return;

        var sComm = GameObject.FindGameObjectWithTag("SerialController");
        Destroy(sComm);

        GameManager.Instance.Stage.Stop();
        _dt = 0;

        Debug.Log($"Stage {GameManager.Instance.Stage.Id} terminated.");        
    }
}
