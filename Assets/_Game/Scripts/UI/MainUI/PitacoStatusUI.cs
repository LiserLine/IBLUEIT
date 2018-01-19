using UnityEngine;
using UnityEngine.UI;

public class PitacoStatusUI : BasicUI<PitacoStatusUI>
{
    [SerializeField]
    private Image imgComponent;

    [SerializeField]
    private Sprite offline, online;

    private void Awake()
    {
        SerialController.Instance.OnSerialConnected += OnSerialConnected;
        SerialController.Instance.OnSerialDisconnected += OnSerialDisconnected;
    }

    private void OnSerialConnected() => imgComponent.sprite = online;
    private void OnSerialDisconnected() => imgComponent.sprite = offline;
}
