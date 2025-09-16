using System.Collections.Generic;
using UnityEngine;

public class MusicVisualizer : MonoBehaviour
{
    MusicDetails music = null;
    float initialTime;

    [SerializeField]
    GameManager gameManager;

    public int CurrentTick =>
        MusicFunctions.GetTickFromTime(Time.time - initialTime, music.MusicBPM);

    Dictionary<Note, GameObject> spawnedNotes = new Dictionary<Note, GameObject>();

    public void SetMusic(MusicDetails music) => this.music = music;

    public void StartMusic() => initialTime = Time.time;

    void FixedUpdate()
    {
        if (music == null)
            return;

        List<Note> notesCopy = new List<Note>(music.Notes);

        int currentTick = CurrentTick;
        foreach (Note note in notesCopy)
        {
            int noteTick = note.TickSpawner;
            if (noteTick <= currentTick)
                SpawnNote(note);
        }
        UpdateNotes();
    }

    void SpawnNote(Note note)
    {
        int noteIndex = note.NoteIndex;
        int noteDuration = note.NoteDuration;

        GameObject newNote = Instantiate(
            gameManager.NotePrefabs[noteDuration].gameObject,
            gameManager.NoteBoardLines[noteIndex]
        );
        newNote.transform.localPosition = Vector3.zero;
        music.Notes.Remove(note);

        spawnedNotes.Add(note, newNote);
    }

    void UpdateNotes()
    {
        int currentTick = CurrentTick;
        foreach (KeyValuePair<Note, GameObject> note in spawnedNotes)
        {
            int spawnTick = note.Key.TickSpawner; // quando apareceu
            int hitTick = spawnTick + 10; // quando deve chegar ao alvo
            GameObject obj = note.Value;

            RectTransform boardRect = obj.transform.parent.GetComponent<RectTransform>();
            float boardSize = boardRect.rect.width;

            float progress =
                (float)(Time.time - MusicFunctions.GetTimeFromTick(spawnTick, music.MusicBPM))
                / (
                    MusicFunctions.GetTimeFromTick(hitTick, music.MusicBPM)
                    - MusicFunctions.GetTimeFromTick(spawnTick, music.MusicBPM)
                );
            progress = Mathf.Clamp01(progress); // para não sair do range

            obj.transform.localPosition = Vector3.Lerp(
                new Vector3(-boardSize / 2, 0, 0), // início fora do tabuleiro
                new Vector3(boardSize / 2, 0, 0), // fim do tabuleiro
                progress
            );
        }
    }
}
