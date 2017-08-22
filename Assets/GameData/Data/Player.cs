public class Player
{
    public uint Id { get; set; }
    public string Name { get; set; }

    public float InspiratoryPeakFlow { get; set; }
    public float ExpiratoryPeakFlow { get; set; }

    public float InspiratoryFlowTime { get; set; }
    public float ExpiratoryFlowTime { get; set; }

    public float RespirationFrequency { get; set; }

    public byte LastPhase { get; set; }
    public byte OpenPhases { get; set; }

    public uint TotalScore { get; set; }
}
