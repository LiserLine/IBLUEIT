using NaughtyAttributes;
using UnityEngine;

public partial class Player : Singleton<Player>
{
    public int HeartPoins => heartPoints;

    [SerializeField]
    [BoxGroup("Properties")]
    private int heartPoints = 5;

    private void Start()
    {
        SerialController.Instance.OnSerialMessageReceived += PositionOnSerial;
        SerialController.Instance.OnSerialMessageReceived += AnimationOnSerial;
        StageManager.Instance.OnStageEnd += SaveRecords;
    }

    private void Update()
    {

#if UNITY_EDITOR
        Move();
#endif

    }

    private void SaveRecords()
    {
        PlayerData.Player.SessionsDone++;
        PlayerData.Player.TotalScore += Scorer.Instance.Score;
        PlayerDb.Instance.Save();
    }
}
