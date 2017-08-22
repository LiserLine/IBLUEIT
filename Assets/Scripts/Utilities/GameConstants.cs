using System.IO;

public class GameConstants
{
#if UNITY_ANDROID
    public static readonly string SaveDataPath = UnityEngine.Application.persistentDataPath + Path.AltDirectorySeparatorChar + "savedata" + Path.AltDirectorySeparatorChar;
#else
    public static readonly string SaveDataPath = "savedata" + Path.AltDirectorySeparatorChar;
#endif
    public static readonly string LocalizationPath = SaveDataPath + "localization.dat";
    public static readonly string PacientsPath = SaveDataPath + "pacients";
    public static readonly string SummaryCsvPath = PacientsPath + "_summary.csv";
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
