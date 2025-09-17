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

    private string path;

    [SerializeField] private AudioSource audioSource;

    /*
    Mokalamity| (name)
    167| (duration)
    120| (bpm)
    1.56| (intial delay)
    
    !Notes!
    5-1-0
    tick-indexbutton-duration
    */

    private void Awake()
    {
        path = Path.Combine(Application.persistentDataPath, "Musics");
    }

    private void Start()
    {
        ReadDetails();
    }

    private void ReadDetails()
    {
        string musicPath = Path.Combine(Path.Combine(Application.persistentDataPath, "Musics"), MusicName);
        string chart = File.ReadAllText(Path.Combine(musicPath, "Chart"));
        AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(musicPath, "assetbundle"));

        // === READ CHART ===
        string[] details = chart.Split('|');
        int musicDuration = int.Parse(details[1]);
        int musicBPM = int.Parse(details[2]);
        float initialDelay = float.Parse(details[3]);
        string notesString = chart.Split("!Notes!")[1];

        string[] notes = notesString.Split('|');

        List<Note> musicNotes = new List<Note>();

        foreach (string note in notes)
        {
            // 5-1-0
            if (note.Length <= 1)
                continue;
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

        // === LOAD AUDIO ===
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
}
