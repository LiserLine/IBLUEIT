using System.IO;
using System.Text;
using UnityEngine;

public class Utils
{
    public static float ParseFloat(string value) => float.Parse(value.Replace('.', ','));

    public static float Clip(float value, float min, float max)
    {
        if (value < min) { return min; }
        return value > max ? max : value;
    }

    public static void WriteAllText(string filepath, string contents)
    {
        var directory = Path.GetDirectoryName(filepath);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(filepath, contents, Encoding.UTF8);

        Debug.LogFormat("File saved: {0}", filepath);
    }

    public static string ReadAllText(string filepath)
    {
        return File.ReadAllText(filepath, Encoding.UTF8);
    }

}
