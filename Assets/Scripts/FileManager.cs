using System.IO;
using UnityEngine;

public static class FileManager
{
    public static void WriteToFile(string filePath, string musicName, string text)
    {
        string musicFolderPath = Path.Combine(Path.GetDirectoryName(filePath), "Musics", musicName);
        Debug.Log(musicFolderPath);
        if (!Directory.Exists(musicFolderPath))
        {
            Directory.CreateDirectory(musicFolderPath);
        }
        string chartFilePath = Path.Combine(musicFolderPath, "chart");
        File.WriteAllText(chartFilePath, text);
    }

    public static string ReadFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }
        else
        {
            return "";
        }
    }

    public static void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public static void RenameFile(string oldFilePath, string newFileName)
    {
        if (File.Exists(oldFilePath))
        {
            string newFilePath = Path.Combine(Path.GetDirectoryName(oldFilePath), newFileName);
            File.Move(oldFilePath, newFilePath);
        }
    }
}
