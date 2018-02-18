using System;

[Serializable]
public class Pacient
{
    public static Pacient Loaded;

    public int Id;
    public string Name;
    public DateTime Birthday;
    public Capacities Capacities;
    public string Observations;
    public ConditionType Condition;
    public int UnlockedLevels;
    public float AccumulatedScore;
    public int PlaySessionsDone;
    public bool CalibrationDone;

#if UNITY_EDITOR
    static Pacient()
    {
        if (Loaded == null)
            Loaded = new Pacient
            {
                Id = -1,
                Birthday = DateTime.Now,
                CalibrationDone = true,
                Condition = ConditionType.Normal,
                Name = "NetRunner",
                PlaySessionsDone = 0,
                UnlockedLevels = 15,
                AccumulatedScore = 0,
                Capacities = new Capacities
                {
                    RespCycleDuration = 3000,
                    ExpPeakFlow = 600,
                    InsPeakFlow = -200,
                    ExpFlowDuration = 6000,
                    InsFlowDuration = 6000
                }
            };
    }
#endif

}

public enum ConditionType
{
    Restrictive = 1,
    Normal = 2,
    Obstructive = 3
}
