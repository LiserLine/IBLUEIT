using UnityEngine;
using UnityEngine.UI;

public class PitacoStatus : MonoBehaviour
{
    public Sprite Disconnected;
    public Sprite Connected;

    private SerialController _serialController;
    private Image _image;

    private float _delta;

    void Start()
    {
        _serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
        _image = GetComponent<Image>();
    }

    void Update()
    {
        _delta += Time.deltaTime;

        if (_delta > 1f)
        {
            var message = _serialController.ReadSerialMessage();
            if (message == null)
                return;

            if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_CONNECTED))
                _image.sprite = Connected;
            else if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
                _image.sprite = Disconnected;

            _delta = 0f;
        }
    }
}
