using System;
using NaughtyAttributes;
using UnityEngine;

public partial class Player : Singleton<Player>
{
    public static PlayerData Data;

    public int HeartPoins => heartPoints;

    [SerializeField]
    [BoxGroup("Properties")]
    private int heartPoints = 5;

    private void OnEnable()
    {

#if UNITY_EDITOR
        if (Data == null)
            Data = new PlayerData
            {
                Id = -1,
                Birthday = DateTime.Now,
                CalibrationDone = true,
                Disfunction = DisfunctionType.Normal,
                Name = "NetRunner",
                SessionsDone = 0,
                StagesOpened = 1,
                TotalScore = 0,
                RespiratoryInfo = new RespiratoryInfo
                {
                    RespirationFrequency = 2500,
                    ExpiratoryPeakFlow = 500,
                    InspiratoryPeakFlow = -150,
                    ExpiratoryFlowTime = 3500,
                    InspiratoryFlowTime = 3500
                }
            };
#endif

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
