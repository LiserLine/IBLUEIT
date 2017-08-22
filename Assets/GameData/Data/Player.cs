using System;

public class Player
{
    public uint Id { get; set; }
    public string Name { get; set; }
    public DateTime Birthday { get; set; }
    public string Observations { get; set; }
    public Disfunctions Disfunction { get; set; }
    public float InspiratoryPeakFlow { get; set; }
    public float ExpiratoryPeakFlow { get; set; }
    public float InspiratoryFlowTime { get; set; }
    public float ExpiratoryFlowTime { get; set; }
    public float RespirationFrequency { get; set; }
    public byte LastLevel { get; set; }
    public byte OpenLevel { get; set; }
    public uint TotalScore { get; set; }
    public uint SessionsDone { get; set; }
    public bool TutorialDone { get; set; }

    public Player()
    {
        
    }
}
