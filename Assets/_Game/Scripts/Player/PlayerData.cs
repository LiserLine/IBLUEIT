using System;

public class PlayerData
{
    public static PlayerData Player;

    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime Birthday { get; set; }

    public RespiratoryInfo RespiratoryInfo { get; set; }

    public string Observations { get; set; }

    public DisfunctionType Disfunction { get; set; }

    public int StagesOpened { get; set; }

    public float TotalScore { get; set; }

    public int SessionsDone { get; set; }

    public bool CalibrationDone { get; set; }

    static PlayerData()
    {

#if UNITY_EDITOR
        if (Player == null)
            Player = new PlayerData
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
                    RespirationFrequency = 3100,
                    ExpiratoryPeakFlow = 1300,
                    InspiratoryPeakFlow = -260,
                    ExpiratoryFlowTime = 8200,
                    InspiratoryFlowTime = 7600
                }
            };
#endif

    }
}

public enum DisfunctionType
{
    Restrictive = 1,
    Normal = 2,
    Obstructive = 3
}
