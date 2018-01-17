using NaughtyAttributes;
using UnityEngine;

public partial class Player : Singleton<Player>
{
    public int HeartPoins => heartPoints;

    [SerializeField]
    [BoxGroup("Properties")]
    private int heartPoints = 5;

    private void OnEnable()
    {
        SerialController.Instance.OnSerialMessageReceived += PositionOnSerial;
        SerialController.Instance.OnSerialMessageReceived += AnimationOnSerial;
    }

    private void Update()
    {

#if UNITY_EDITOR
        Move();
#endif

    }
}
