using NaughtyAttributes;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    public int HeartPoins => heartPoints;

    [SerializeField]
    [BoxGroup("Properties")]
    private int heartPoints = 5;

    private void Awake()
    {
        var sc = FindObjectOfType<SerialController>();
        sc.OnSerialMessageReceived += PositionOnSerial;
        sc.OnSerialMessageReceived += Animate;
    }

    private void Update()
    {
#if UNITY_EDITOR
        Move();
#endif
    }
}