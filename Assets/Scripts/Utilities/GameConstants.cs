using System.IO;

public class GameConstants
{
#if UNITY_ANDROID
    public static readonly string SaveDataPath = UnityEngine.Application.persistentDataPath + Path.AltDirectorySeparatorChar + "savedata";
#else
    public static readonly string SaveDataPath = "savedata";
#endif
    public static readonly string LocalizationPath = SaveDataPath + Path.AltDirectorySeparatorChar + "localization.dat";
    public static readonly string PacientsPath = SaveDataPath + Path.AltDirectorySeparatorChar + "pacients";
    public static readonly string SummaryCsvPath = PacientsPath + Path.AltDirectorySeparatorChar + "_summary.csv";
}

public enum Disfunctions
{
    Normal,
    Restrictive,
    Obstructive
}

public enum ControlBehaviour
{
    Absolute,
    Relative
}
