using System.IO;
using System.Text;
using UnityEngine;

public class FileReader
{
    public static void WriteAllText(string filepath, string contents)
    {
        var directory = Path.GetDirectoryName(filepath);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(filepath, contents, Encoding.UTF8);

        Debug.LogFormat("File saved: {0}", filepath);
    }

    public static string ReadAllText(string filepath) => File.ReadAllText(filepath, Encoding.UTF8);

    public static void AppendAllText(string path, string contents) => File.AppendAllText(path, contents, Encoding.UTF8);

    public static string ReadCsv(string filepath)
    {
        var text = ReadAllText(filepath);

        if (text.Split('\t').Length > 0)
            text = text.Replace('\t', ';');

        return text;
    }
}
