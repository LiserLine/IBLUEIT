using System;

public class Player
{
    private float _ipf, _epf, _ift, _eft, _rf;

    public uint Id { get; set; }

    public string Name { get; set; }

    public DateTime Birthday { get; set; }

    public string Observations { get; set; }

    public Disfunctions Disfunction { get; set; }

    public float InspiratoryPeakFlow
    {
        get { return _ipf * GameConstants.UserPowerMercy; }
        set { _ipf = value; }
    }

    public float ExpiratoryPeakFlow
    {
        get { return _epf * GameConstants.UserPowerMercy; }
        set { _epf = value; }
    }

    public float InspiratoryFlowTime
    {
        get { return _ift * GameConstants.UserPowerMercy; }
        set { _ift = value; }
    }

    public float ExpiratoryFlowTime
    {
        get { return _eft * GameConstants.UserPowerMercy; }
        set { _eft = value; }
    }

    public float RespirationFrequency
    {
        get { return _rf * GameConstants.UserPowerMercy; }
        set { _rf = value; }
    }

    public byte LastLevel { get; set; }

    public byte OpenLevel { get; set; }

    public uint TotalScore { get; set; }

    public uint SessionsDone { get; set; }

    public bool TutorialDone { get; set; }

    public Player()
    {
        //ToDo
    }
}
