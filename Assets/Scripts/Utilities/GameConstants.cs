using System.IO;
using UnityEngine;

public class GameConstants
{
    public static readonly string SaveDataPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "savedata";
    public static readonly string LocalizationPath = SaveDataPath + Path.AltDirectorySeparatorChar + "localization.dat";
}

public enum ControlBehaviour
{
    Absolute,
    Relative
}
