using System;

public class Player
{
    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime Birthday { get; set; }

    public RespiratoryInfo RespiratoryInfo { get; set; }

    public string Observations { get; set; }

    public Disfunctions Disfunction { get; set; }

    public int OpenLevel { get; set; }

    public int TotalScore { get; set; }

    public int SessionsDone { get; set; }

    public bool CalibrationDone { get; set; }
}
