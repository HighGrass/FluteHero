using System.IO;
using UnityEngine;

public static class FileManager
{
    public static string CreateFile(string fileName, string directory = null)
    {
        if (directory == null)
        {
            directory = Application.persistentDataPath;
        }

        string filePath = Path.Combine(directory, fileName);

        if (!File.Exists(filePath))
        {
            File.Create(filePath).Dispose();
        }

        return filePath;
    }

    public static void WriteToFile(string filePath, string text)
    {
        File.WriteAllText(filePath, text);
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

    public static void AppendToFile(string filePath, string text)
    {
        if (File.Exists(filePath))
        {
            File.AppendAllText(filePath, text);
        }
        else
        {
            WriteToFile(filePath, text);
        }
    }
}
