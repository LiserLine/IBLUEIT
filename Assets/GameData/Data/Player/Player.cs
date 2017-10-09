using System;

public class Player
{
    public uint Id { get; set; }

    public string Name { get; set; }

    public DateTime Birthday { get; set; }

    public RespiratoryInfo RespiratoryInfo { get; set; }

    public string Observations { get; set; }

    public Disfunctions Disfunction { get; set; }

    public byte LastLevel { get; set; }

    public byte OpenLevel { get; set; }

    public uint TotalScore { get; set; }

    public uint SessionsDone { get; set; }

    public bool CalibrationDone { get; set; }
}
