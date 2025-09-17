using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicLoader : MonoBehaviour
{
    public Music LoadMusic(string musicName)
    {
        string musicFolderPath = Path.Combine(Application.streamingAssetsPath, "Musics", musicName);
        string chartPath = Path.Combine(musicFolderPath, "chart.json");
        string assetBundlePath = Path.Combine(musicFolderPath, "assetbundle");

        Debug.Log($"Loading music from: {musicFolderPath}");
        Debug.Log($"Chart path: {chartPath}");
        Debug.Log($"AssetBundle path: {assetBundlePath}");

        // Load chart data
        if (!File.Exists(chartPath))
        {
            Debug.LogError($"Chart file not found at: {chartPath}");
            return null;
        }

        string jsonData = File.ReadAllText(chartPath);
        MusicData musicData = JsonUtility.FromJson<MusicData>(jsonData);

        // Load AssetBundle
        if (!File.Exists(assetBundlePath))
        {
            Debug.LogError($"AssetBundle not found at: {assetBundlePath}");
            return null;
        }

        AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
        if (assetBundle == null)
        {
            Debug.LogError($"Failed to load AssetBundle from: {assetBundlePath}");
            return null;
        }

        Debug.Log($"AssetBundle loaded successfully. Contains assets:");
        string[] assetNames = assetBundle.GetAllAssetNames();
        foreach (string assetName in assetNames)
        {
            Debug.Log($"  - {assetName}");
        }

        // Try to load audio clip from AssetBundle with different possible paths
        AudioClip audioClip = null;
        string[] possiblePaths = new string[]
        {
            "assets/music.wav",
            "assets/music.ogg",
            "assets/music.mp3",
            "music.wav",
            "music.ogg",
            "music.mp3",
            "music"
        };

        foreach (string path in possiblePaths)
        {
            audioClip = assetBundle.LoadAsset<AudioClip>(path);
            if (audioClip != null)
            {
                Debug.Log($"Successfully loaded audio clip from: {path}");
                break;
            }
        }

        if (audioClip == null)
        {
            Debug.LogError($"Audio clip not found in AssetBundle. Tried paths: {string.Join(", ", possiblePaths)}");
            assetBundle.Unload(false);
            return null;
        }

        // Convert note data to Note objects
        List<Note> notes = new List<Note>();
        foreach (NoteData noteData in musicData.notes)
        {
            Note note = new Note(noteData.time, noteData.lane);
            notes.Add(note);
        }

        // Sort notes by time
        notes.Sort((a, b) => a.time.CompareTo(b.time));

        // Unload AssetBundle (the audio clip is already loaded into memory)
        assetBundle.Unload(false);

        return new Music(musicName, musicData.bpm, notes, audioClip);
    }
}

// Helper classes for JSON deserialization
[System.Serializable]
public class MusicData
{
    public float bpm;
    public NoteData[] notes;
}

[System.Serializable]
public class NoteData
{
    public float time;
    public int lane;
}
