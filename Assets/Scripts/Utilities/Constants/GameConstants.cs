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
    public const int RespiratoryFrequencyThreshold = 1000; //In Milliseconds

    public const float PitacoThreshold = 10f;

    public static string GetSessionsPath(Player plr)
    {
        return PacientsPath + plr.Id + Path.AltDirectorySeparatorChar;
    }

    public static readonly PlataformConstants Plataform = new PlataformConstants();
}

public enum Disfunctions
{
    Restrictive = 1,
    Normal = 2,
    Obstructive = 3
}

public enum PlataformElements
{
    Targets,
    Obstacles,
    TargetAndObstacles
}

public enum ControlBehaviour
{
    Absolute,
    Relative
}
