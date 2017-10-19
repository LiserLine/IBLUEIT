using System.IO;

//ToDo - Load this class from CSV file
public class GameConstants
{
#if UNITY_ANDROID
    public static readonly string SaveDataPath = UnityEngine.Application.persistentDataPath + Path.AltDirectorySeparatorChar + "savedata" + Path.AltDirectorySeparatorChar;
#else
    public static readonly string SaveDataPath = "savedata" + Path.AltDirectorySeparatorChar;
#endif

    public static readonly string PacientsPath = SaveDataPath + "pacients" + Path.AltDirectorySeparatorChar;
    public static readonly string LocalizationPath = SaveDataPath + "localization.dat";
    public static readonly string PacientListFile = PacientsPath + "_pacientsList.csv";

    public const float UserPowerMercy = 0.6f;

    //ToDo - Threshold based on disfunction ?
    //ToDo - test if this threshold is good
    public const float RespiratoryFrequencyThreshold = 1000f; //In Milliseconds

    public const float PitacoThreshold = 10f;
    
    public static string GetSessionsPath(Player plr)
    {
        return PacientsPath + plr.Id + Path.AltDirectorySeparatorChar;
    }
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
