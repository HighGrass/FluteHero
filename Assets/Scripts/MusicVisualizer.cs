using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicVisualizer : MonoBehaviour
{
    MusicDetails music = null;
    float initialTime;

    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    private float idealProgress = 0.9f;

    [SerializeField]
    private float progressOffset = 0.05f;

    [SerializeField]
    int ticksToHit = 10;

    public int CurrentTick =>
        MusicFunctions.GetTickFromTime(Time.time - initialTime, music.MusicBPM);

    Dictionary<Note, GameObject> spawnedNotes = new Dictionary<Note, GameObject>();
    List<Note> pressingNotes = new List<Note>();
    List<Note> ignoreNotes = new List<Note>();

    // note => pressed tick

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            OnPlayNote(1);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            OnPlayNote(2);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            OnPlayNote(3);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            OnPlayNote(4);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            OnPlayNote(5);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            OnPlayNote(6);
    }

    void SpawnNote(Note note)
    {
        int noteIndex = note.NoteIndex - 1;

        int spriteIndex = MusicFunctions.GetNoteSpriteIndex(noteIndex);

        GameObject newNote = Instantiate(
            gameManager.NotePrefabs[spriteIndex].gameObject,
            gameManager.NoteBoardLines[noteIndex]
        );
        newNote.transform.localPosition = Vector3.zero;
        music.Notes.Remove(note);

        spawnedNotes.Add(note, newNote);
    }

    void UpdateNotes()
    {
        int currentTick = CurrentTick;
        Dictionary<Note, GameObject> spawnedNotesCopy = new Dictionary<Note, GameObject>(
            spawnedNotes
        );
        foreach (KeyValuePair<Note, GameObject> note in spawnedNotesCopy)
        {
            int spawnTick = note.Key.TickSpawner; // quando apareceu
            int hitTick = spawnTick + ticksToHit; // quando deve chegar ao alvo
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

            if (progress >= 1)
            {
                spawnedNotes.Remove(note.Key);
                if (ignoreNotes.Contains(note.Key))
                    ignoreNotes.Remove(note.Key);

                if (!pressingNotes.Contains(note.Key))
                    Destroy(obj);
            }
            else
            {
                if (
                    !ignoreNotes.Contains(note.Key)
                    && progress > idealProgress + progressOffset
                    && !pressingNotes.Contains(note.Key)
                )
                    obj.GetComponentInChildren<Image>().color = Color.red;

                obj.transform.localPosition = Vector3.Lerp(
                    new Vector3(-boardSize / 2, 0, 0), // início fora do tabuleiro
                    new Vector3(boardSize / 2, 0, 0), // fim do tabuleiro
                    progress
                );
            }
        }
    }

    void OnPlayNote(int noteIndex)
    {
        foreach (KeyValuePair<Note, GameObject> note in spawnedNotes)
        {
            int thisNoteIndex = note.Key.NoteIndex;
            GameObject noteObj = note.Value;

            if (ignoreNotes.Contains(note.Key))
                continue; // para ter a certeza que não acertamos a mesma nota 2 vezes

            float progress =
                (float)(
                    Time.time - MusicFunctions.GetTimeFromTick(note.Key.TickSpawner, music.MusicBPM)
                )
                / (
                    MusicFunctions.GetTimeFromTick(
                        note.Key.TickSpawner + ticksToHit,
                        music.MusicBPM
                    ) - MusicFunctions.GetTimeFromTick(note.Key.TickSpawner, music.MusicBPM)
                );
            progress = Mathf.Clamp01(progress);

            if (
                progress >= idealProgress - progressOffset
                && progress <= idealProgress + progressOffset
            )
            {
                if (thisNoteIndex == noteIndex)
                {
                    noteObj.GetComponentInChildren<Image>().color = Color.green;

                    gameManager.pointsCounter.HitNote(note.Key);
                    ignoreNotes.Add(note.Key);

                    if (note.Key.isDragNote())
                        pressingNotes.Add(note.Key);
                }
            }
        }
    }

    void OnReleaseNote(int noteIndex) { }
}
