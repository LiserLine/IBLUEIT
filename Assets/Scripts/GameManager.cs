using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Player Player { get; set; }
    public Stage Stage { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        
#if UNITY_EDITOR // debug player for testing

        if (Player == null)
        {
            Player = new Player
            {
                Name = "Netrunner",
                Id = 9999,
                RespiratoryInfo = new RespiratoryInfo
                {
                    // raw simulated values
                    InspiratoryPeakFlow = 300f,
                    ExpiratoryPeakFlow = 600f,
                    InspiratoryFlowTime = 1f,
                    ExpiratoryFlowTime = 3f,
                    RespirationFrequency = 3f,
                },
                CalibrationDone = true,
                SessionsDone = 1,
                OpenLevel = 1,
                Birthday = DateTime.Parse("08/08/2018"),
                Disfunction = Disfunctions.Normal,
                Observations = string.Empty,
                TotalScore = 1337,
            };
        }
#endif

    }
}
