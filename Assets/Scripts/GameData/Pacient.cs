public static class Pacient
{
    private const float _pacientValueModifier = 0.75f;

    /// <summary>
    /// The pacient's name
    /// </summary>
    public static string Name { get; set; }

    /// <summary>
    /// PacientID on save file
    /// </summary>
    public static int ID { get; set; }

    /// <summary>
    /// Expiratory Pressure Peak (score)
    /// </summary>
    public static float EPP
    {
        get
        {
            return _epp * _pacientValueModifier;
        }
        set
        {
            _epp = value;
        }
    }

    /// <summary>
    /// Inspiratory Pressure Peak
    /// </summary>
    public static float IPP
    {
        get
        {
            return _ipp * _pacientValueModifier;
        }
        set
        {
            _ipp = value;
        }
    }

    /// <summary>
    /// Max Respiratory Frequency
    /// </summary>
    public static float MaxRespiratoryFrequency
    {
        get
        {
            return _mrf * _pacientValueModifier;
        }
        set
        {
            _mrf = value;
        }
    }


    private static float _epp;
    private static float _ipp;
    private static float _mrf;
}