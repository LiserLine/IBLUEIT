using System;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    public static PlayerDto playerDto;

    [SerializeField]
    private int heartPoints = 5;

    private void OnEnable()
    {

#if UNITY_EDITOR
        if (playerDto == null)
            playerDto = new PlayerDto
            {
                Id = 999,
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

        SerialController.OnSerialMessageReceived += PositionOnSerial;
    }

    private void OnDisable()
    {
        SerialController.OnSerialMessageReceived -= PositionOnSerial;
    }

    private void Update()
    {

#if UNITY_EDITOR
        Move();
#endif

    }
}
