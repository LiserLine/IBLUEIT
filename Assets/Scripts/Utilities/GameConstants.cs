﻿using System.IO;

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

    public const float UserPowerMercy = 60f / 100f;
    public const float PitacoThreshold = 20f;

    public static float ParseSerialMessage(string msg) => float.Parse(msg.Replace('.', ','));

    public static string GetSessionsPath(int plrId)
    {
        return PacientsPath + Path.AltDirectorySeparatorChar + plrId + ".csv";
    }

    public static string GetSessionsPath(Player plr)
    {
        return GetSessionsPath(plr.Id);
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
