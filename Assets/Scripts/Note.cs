public class Note
{
    public int TickSpawner { get; set; }
    public int NoteIndex { get; set; }
    public int NoteDuration { get; set; }

    public bool isDragNote() => NoteDuration > 0;

    public Note(int tickSpawner, int noteIndex, int noteDuration)
    {
        TickSpawner = tickSpawner;
        NoteIndex = noteIndex;
        NoteDuration = noteDuration;
    }
}
