using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicReader : MonoBehaviour
{
    [SerializeField]
    string MusicName;

    [SerializeField]
    GameManager gameManager;
    MusicDetails musicDetails;

    /*
    Mokalamity| (name)
    167| (duration)
    120| (bpm)
    1.56| (intial delay)
    
    !Notes!
    5-1-0
    tick-indexbutton-duration
    */

    void Start() => ReadDetails();

    private void ReadDetails()
    {
        string path = Path.Combine(Application.persistentDataPath, MusicName);
        string content = File.ReadAllText(path);

        string[] details = content.Split('|');
        int musicDuration = int.Parse(details[1]);
        int musicBPM = int.Parse(details[2]);
        float initialDelay = float.Parse(details[3]);
        string notesString = content.Split("!Notes!")[1];

        string[] notes = notesString.Split('|');

        List<Note> musicNotes = new List<Note>();

        foreach (string note in notes)
        {
            // 5-1-0
            Debug.Log(note);
            int tick = int.Parse(note.Split('-')[0]);
            int index = int.Parse(note.Split('-')[1]);
            int duration = int.Parse(note.Split('-')[2]);

            Note newNote = new Note(tick, index, duration);
            musicNotes.Add(newNote);
        }

        musicDetails = new MusicDetails(
            MusicName,
            path,
            musicDuration,
            musicBPM,
            initialDelay,
            musicNotes
        );
        gameManager.musicVisualizer.SetMusic(musicDetails);
    }
}
