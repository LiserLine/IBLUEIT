using System;

public class PlayerData
{
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
}

public enum DisfunctionType
{
    Restrictive = 1,
    Normal = 2,
    Obstructive = 3
}
