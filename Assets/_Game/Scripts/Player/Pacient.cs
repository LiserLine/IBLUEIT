using System;

public class Pacient
{
    public static Pacient Loaded;

    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime Birthday { get; set; }

    public RespiratoryData RespiratoryData { get; set; }

    public string Observations { get; set; }

    public DisfunctionType Disfunction { get; set; }

    public int StagesOpened { get; set; }

    public float TotalScore { get; set; }

    public int PlaySessionsDone { get; set; }

    public bool CalibrationDone { get; set; }

    static Pacient()
    {

#if UNITY_EDITOR
        if (Loaded == null)
            Loaded = new Pacient
            {
                Id = -1,
                Birthday = DateTime.Now,
                CalibrationDone = true,
                Disfunction = DisfunctionType.Normal,
                Name = "NetRunner",
                PlaySessionsDone = 0,
                StagesOpened = 1,
                TotalScore = 0,
                RespiratoryData = new RespiratoryData
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
