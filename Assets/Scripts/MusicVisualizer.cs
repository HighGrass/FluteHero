using System.Collections;
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
    private float idealProgress = 0.88f;

    [SerializeField]
    private float progressOffset = 0.025f;

    [SerializeField]
    float dragNoteTimeout = 0.1f;

    [SerializeField]
    int ticksToHit = 20;

    bool[] paintingLine = new bool[6];

    public int CurrentTick =>
        MusicFunctions.GetTickFromTime(Time.time - initialTime, music.MusicBPM, music.TicksPerBeat);

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

        if (Input.GetKeyUp(KeyCode.Alpha1))
            OnReleaseNote(1);
        if (Input.GetKeyUp(KeyCode.Alpha2))
            OnReleaseNote(2);
        if (Input.GetKeyUp(KeyCode.Alpha3))
            OnReleaseNote(3);
        if (Input.GetKeyUp(KeyCode.Alpha4))
            OnReleaseNote(4);
        if (Input.GetKeyUp(KeyCode.Alpha5))
            OnReleaseNote(5);
        if (Input.GetKeyUp(KeyCode.Alpha6))
            OnReleaseNote(6);
    }

    void SpawnNote(Note note)
    {
        int noteIndex = note.NoteIndex - 1;

        int spriteIndex = MusicFunctions.GetNoteSpriteIndex(noteIndex, note.IsDragNote);

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

            float now = Time.time - initialTime; // tempo desde o início da música
            float spawnTime = MusicFunctions.GetTimeFromTick(
                spawnTick,
                music.MusicBPM,
                music.TicksPerBeat
            );
            float hitTime = MusicFunctions.GetTimeFromTick(
                hitTick,
                music.MusicBPM,
                music.TicksPerBeat
            );

            float progress = (now - spawnTime) / (hitTime - spawnTime);
            progress = Mathf.Clamp01(progress);

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
                {
                    MissNote(note.Key);
                    ignoreNotes.Add(note.Key);
                    // Image[] images = obj.GetComponentsInChildren<Image>();
                    // foreach (Image image in images)
                    //     image.color = Color.red;
                }
                else if (
                    ignoreNotes.Contains(note.Key)
                    && progress > idealProgress + dragNoteTimeout
                    && pressingNotes.Contains(note.Key)
                )
                {
                    pressingNotes.Remove(note.Key);
                    HitNote(note.Key);
                    Debug.Log("Hit long note");
                }
                else if (
                    ignoreNotes.Contains(note.Key)
                    && progress > idealProgress + dragNoteTimeout
                    && !pressingNotes.Contains(note.Key)
                    && note.Key.IsDragNote
                )
                {
                    MissNote(note.Key);
                }
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

            float now = Time.time - initialTime;
            float spawnTime = MusicFunctions.GetTimeFromTick(
                note.Key.TickSpawner,
                music.MusicBPM,
                music.TicksPerBeat
            );
            float hitTime = MusicFunctions.GetTimeFromTick(
                note.Key.TickSpawner + ticksToHit,
                music.MusicBPM,
                music.TicksPerBeat
            );
            float progress = (now - spawnTime) / (hitTime - spawnTime);
            progress = Mathf.Clamp01(progress);

            if (
                progress >= idealProgress - progressOffset
                && progress <= idealProgress + progressOffset
            )
            {
                if (thisNoteIndex == noteIndex)
                {
                    ignoreNotes.Add(note.Key);

                    if (note.Key.IsDragNote)
                    {
                        pressingNotes.Add(note.Key);
                        HoldNote(note.Key);
                    }
                    else
                    {
                        HitNote(note.Key);
                        // Image[] images = note.Value.GetComponentsInChildren<Image>();
                        // foreach (Image image in images)
                        //     image.color = Color.green;
                    }
                }
            }
        }
    }

    void OnReleaseNote(int noteIndex)
    {
        List<Note> pressingNotesCopy = new List<Note>(pressingNotes);
        foreach (Note note in pressingNotesCopy)
        {
            if (note.NoteIndex == noteIndex)
                pressingNotes.Remove(note);
        }
    }

    void HitNote(Note note)
    {
        int noteIndex = note.NoteIndex - 1;
        SetBoardLineColor(noteIndex, Color.green);
        gameManager.pointsCounter.HitNote(note);
    }

    void MissNote(Note note)
    {
        int noteIndex = note.NoteIndex - 1;
        SetBoardLineColor(noteIndex, Color.red);
    }

    void HoldNote(Note note)
    {
        int noteIndex = note.NoteIndex - 1;
        SetBoardLineColor(noteIndex, Color.yellow, 0.3f);
    }

    void SetBoardLineColor(int lineIndex, Color color, float delay = 0.2f)
    {
        if (paintingLine[lineIndex])
            return;

        paintingLine[lineIndex] = true;
        StartCoroutine(ChangeBoardLineColor(lineIndex, color, delay));
    }

    IEnumerator ChangeBoardLineColor(int lineIndex, Color color, float delay)
    {
        Color defaultColor = gameManager.NoteBoardLines[lineIndex].GetComponent<Image>().color;

        gameManager.NoteBoardLines[lineIndex].GetComponent<Image>().color = color;
        yield return new WaitForSeconds(delay);
        gameManager.NoteBoardLines[lineIndex].GetComponent<Image>().color = defaultColor;
        paintingLine[lineIndex] = false;
        yield break;
    }
}
