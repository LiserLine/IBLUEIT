using UnityEngine;
using UnityEngine.UI;

public class PitacoStatus : MonoBehaviour
{
    public Sprite Disconnected;
    public Sprite Connected;

    private SerialMessengerListener _serialMessengerListener;
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
        _serialMessengerListener = GetComponent<SerialMessengerListener>();
    }

    private void Update()
    {
        _image.sprite = _serialMessengerListener.IsConnected ? Connected : Disconnected;
    }
}
