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

    private static Dictionary<KeyCode, float> keysMap = new Dictionary<KeyCode, float>
    { //key, time (start)
        { KeyCode.Alpha1, 0f },
        { KeyCode.Alpha2, 0f },
        { KeyCode.Alpha3, 0f },
        { KeyCode.Alpha4, 0f },
        { KeyCode.Alpha5, 0f },
        { KeyCode.Alpha6, 0f },
    };

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
        musicNotes +=
            $"{key}-{MusicFunctions.GetSyncTiming(keysMap[key], musicBPM)}-{MusicFunctions.GetSyncTiming(Time.time, musicBPM)}|";
        Debug.Log(
            "Node: "
                + key
                + " | Duration: "
                + (
                    MusicFunctions.GetSyncTiming(Time.time, musicBPM)
                    - MusicFunctions.GetSyncTiming(keysMap[key], musicBPM)
                )
                + " | Tick : "
                + MusicFunctions.GetTickFromTime(
                    MusicFunctions.GetSyncTiming(keysMap[key], musicBPM),
                    musicBPM
                )
        );
        keysMap[key] = 0f;
    }

    void OnApplicationQuit()
    {
        string filePath = Path.Combine(Application.persistentDataPath, musicName);
        Debug.Log(filePath);
        FileManager.WriteToFile(filePath, musicNotes);
    }
}
