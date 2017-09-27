using System.IO;

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
    
    public static string GetSessionsPath(Player plr)
    {
        return PacientsPath + Path.AltDirectorySeparatorChar + plr.Id;
    }

    public const float UserPowerMercy = 60f/100f;

    public static float ParseSerialMessage(string msg) => float.Parse(msg.Replace('.', ','));
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
