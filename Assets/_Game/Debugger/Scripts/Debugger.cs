using UnityEngine;

public partial class Debugger : MonoBehaviour
{
    public void Awake()
    {

#if UNITY_EDITOR
#else
        Destroy(this.gameObject);
#endif

        Application.logMessageReceived += (message, stacktrace, type) =>
        {
            _messageOutput = message;
        };

        AlignDisplayBox();
    }

    private void OnGUI()
    {
        DisplayFramesPerSecond();
        DisplayLogMessages();
    }

    private void Update()
    {
        UpdateFpsDisplayTimer();
    }
}
