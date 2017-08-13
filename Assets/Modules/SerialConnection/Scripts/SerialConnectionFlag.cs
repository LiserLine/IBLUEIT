using UnityEngine;
using UnityEngine.UI;

public class SerialConnectionFlag : MonoBehaviour
{
    public Sprite Connected;
    public Sprite Disconnected;
    private Image _image;

    private void Start()
    {
        _image = this.GetComponent<Image>();
    }

    private void Update()
    {
        if (SerialConnectionManager.Instance.IsConnected)
        {
            _image.sprite = Connected;
        }
        else
        {
            _image.sprite = Disconnected;
        }
    }
}
