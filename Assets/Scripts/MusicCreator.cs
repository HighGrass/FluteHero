using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicCreator : MonoBehaviour
{
    [SerializeField]
    string musicName;

    [SerializeField]
    int musicBPM;

    [SerializeField]
    float musicTime;

    [SerializeField]
    float initialDelay;

    string musicNotes;

    [SerializeField]
    private AudioSource audioSource;

    private int MAX_NOTE_DURATION = 5;

    private static Dictionary<KeyCode, float> keysMap = new Dictionary<KeyCode, float>
    { //key, time (start)
        { KeyCode.Alpha1, 0f },
        { KeyCode.Alpha2, 0f },
        { KeyCode.Alpha3, 0f },
        { KeyCode.Alpha4, 0f },
        { KeyCode.Alpha5, 0f },
        { KeyCode.Alpha6, 0f },
    };

    private void Start()
    {
        string musicPath = Path.Combine(Path.Combine(Application.persistentDataPath, "Musics"), musicName);
        AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(musicPath, "assetbundle"));
        
        if (bundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
            return;
        }

        AudioClip clip = bundle.LoadAsset<AudioClip>("assets/music.wav");

        if (clip == null)
        {
            Debug.LogError("Failed to load AudioClip from AssetBundle!");
            return;
        }

        audioSource.clip = clip;
        audioSource.Play();

        bundle.Unload(false);
    }

    void Update()
    {
        Dictionary<KeyCode, float> keysMapTempCopy = new Dictionary<KeyCode, float>(keysMap);

        foreach (KeyCode key in keysMapTempCopy.Keys)
        {
            if (keysMap[key] <= 0f && Input.GetKey(key))
                keysMap[key] = Time.time;
            else if (keysMap[key] != 0f && !Input.GetKey(key))
                CreateNote(key);
        }
    }

    void CreateNote(KeyCode key)
    {
        // |key-startTime-endTime|

        int tick = MusicFunctions.GetTickFromTime(
            MusicFunctions.GetSyncTiming(Time.time, musicBPM),
            musicBPM
        );

        int duration =
            MusicFunctions.GetTickFromTime(Time.time, musicBPM)
            - MusicFunctions.GetTickFromTime(keysMap[key], musicBPM);

        string thisNote =
            $"{tick}-{MusicFunctions.ConvertKeyToNumber(key)}-{Mathf.Clamp(duration, 0, MAX_NOTE_DURATION)}|\n";
        musicNotes += thisNote;
        keysMap[key] = 0f;
    }

    void OnApplicationQuit()
    {
        string file = FormatMusicFile();
        FileManager.WriteToFile(Path.Combine(Application.persistentDataPath, "Musics"), musicName, file);
    }

    string FormatMusicFile()
    {
        string file =
            musicName
            + "|\n"
            + musicTime
            + "|\n"
            + musicBPM
            + "|\n"
            + initialDelay
            + "|\n"
            + "!Notes!\n"
            + musicNotes;
        return file;
    }
}
