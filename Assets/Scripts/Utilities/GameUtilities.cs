using System.IO;
using UnityEngine;

public static class GameUtilities
{
    public static void WriteAllText(string filepath, string contents)
    {
        var directory = filepath.Split('/')[0];
        if (!Directory.Exists(Path.GetDirectoryName(directory)))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filepath, contents);

        Debug.LogFormat("File saved: {0}", filepath);
    }

    public static string ReadAllText(string filepath)
    {
        return File.ReadAllText(filepath);
    }
}
