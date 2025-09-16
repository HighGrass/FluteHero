using System.Collections.Generic;
using System.IO;

public class MusicDetails
{
    public string MusicName { get; }
    public string MusicPath { get; }
    public int MusicDuration { get; }
    public int MusicBPM { get; }
    public float InitialDelay { get; }

    public List<Note> Notes { get; }

    public MusicDetails(
        string name,
        string path,
        int duration,
        int bpm,
        float initialDelay,
        List<Note> notes
    )
    {
        MusicName = name;
        MusicPath = path;

        MusicBPM = bpm;
        InitialDelay = initialDelay;

        Notes = notes;
    }
}
