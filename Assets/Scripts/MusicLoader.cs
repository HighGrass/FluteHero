using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicLoader : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

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

        // Try to load audio clip
        AudioClip audioClip = null;
        string[] possiblePaths = new string[]
        {
            "assets/music.wav",
            "assets/music.ogg",
            "assets/music.mp3",
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

        if (audioSource == null)
            audioSource = FindAnyObjectByType<AudioSource>();

        if (audioSource != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
            audioSource.Pause();
            audioSource.time = 0f;
        }

        // === Convert note data into Note objects (seconds only) ===
        List<Note> notes = new List<Note>();

        int beatsPerMeasure = 4;
        int subdivisionsPerBeat = 4;
        if (!string.IsNullOrEmpty(musicData.signature))
        {
            string[] sigParts = musicData.signature.Split('/');
            if (sigParts.Length == 2 && int.TryParse(sigParts[0], out int numerator))
            {
                beatsPerMeasure = numerator;
            }
        }

        foreach (NoteData noteData in musicData.notes)
        {
            float timeInSeconds = -1f;

            if (!string.IsNullOrEmpty(noteData.beat))
            {
                try
                {
                    timeInSeconds = BeatTimeUtils.BeatToSeconds(
                        noteData.beat,
                        beatsPerMeasure,
                        subdivisionsPerBeat,
                        musicData.bpm
                    );
                }
                catch (Exception e)
                {
                    Debug.LogError($"Invalid beat notation {noteData.beat}: {e.Message}");
                }
            }
            else if (noteData.time >= 0f)
            {
                timeInSeconds = noteData.time; // fallback to seconds if provided
            }

            if (timeInSeconds >= 0f)
            {
                notes.Add(new Note(timeInSeconds, noteData.lane));
            }
        }

        // Sort notes by time
        notes.Sort((a, b) => a.time.CompareTo(b.time));

        // Unload AssetBundle (audio clip stays in memory)
        assetBundle.Unload(false);

        return new Music(musicName, musicData.bpm, notes, audioClip);
    }
}

// === Helpers ===
[Serializable]
public class MusicData
{
    public float bpm;
    public string signature; // e.g. "4/4"
    public NoteData[] notes;
}

[Serializable]
public class NoteData
{
    public float time = -1f; // optional fallback
    public string beat;      // "M.S" format
    public int lane;
}
