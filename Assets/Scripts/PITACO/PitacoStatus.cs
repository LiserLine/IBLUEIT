using UnityEngine;
using UnityEngine.UI;

public class PitacoStatus : MonoBehaviour
{
    public Sprite Disconnected;
    public Sprite Connected;
    public SerialController SerialController;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        _image.sprite = SerialController.IsConnected ? Connected : Disconnected;
    }
}
