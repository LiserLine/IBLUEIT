using UnityEngine;
using UnityEngine.UI;

public class PitacoStatus : MonoBehaviour
{
    public Sprite Disconnected;
    public Sprite Connected;

    private SerialListener _serialMessengerListener;
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _serialMessengerListener = GetComponent<SerialListener>();
    }

    private void Update()
    {
        _image.sprite = _serialMessengerListener.IsConnected ? Connected : Disconnected;
    }
}
