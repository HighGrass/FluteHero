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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            OnPlayNote(KeyCode.Alpha1);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            OnPlayNote(KeyCode.Alpha2);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            OnPlayNote(KeyCode.Alpha3);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            OnPlayNote(KeyCode.Alpha4);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            OnPlayNote(KeyCode.Alpha5);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            OnPlayNote(KeyCode.Alpha6);
    }

    void SpawnNote(Note note)
    {
        int noteIndex = note.NoteIndex - 1;
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
        Dictionary<Note, GameObject> spawnedNotesCopy = new Dictionary<Note, GameObject>(
            spawnedNotes
        );
        foreach (KeyValuePair<Note, GameObject> note in spawnedNotesCopy)
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

            if (progress >= 1)
            {
                spawnedNotes.Remove(note.Key);
                Destroy(obj);
            }
            else
            {
                obj.transform.localPosition = Vector3.Lerp(
                    new Vector3(-boardSize / 2, 0, 0), // início fora do tabuleiro
                    new Vector3(boardSize / 2, 0, 0), // fim do tabuleiro
                    progress
                );
            }
        }
    }

    void OnPlayNote(KeyCode key)
    {
        int noteIndex = MusicFunctions.ConvertKeyToNumber(key);

        foreach (KeyValuePair<Note, GameObject> note in spawnedNotes)
        {
            int thisNoteIndex = note.Key.NoteIndex;
            GameObject noteObj = note.Value;

            float progress =
                (float)(
                    Time.time - MusicFunctions.GetTimeFromTick(note.Key.TickSpawner, music.MusicBPM)
                )
                / (
                    MusicFunctions.GetTimeFromTick(note.Key.TickSpawner + 10, music.MusicBPM)
                    - MusicFunctions.GetTimeFromTick(note.Key.TickSpawner, music.MusicBPM)
                );
            progress = Mathf.Clamp01(progress);

            if (
                progress >= idealProgress - progressOffset
                && progress <= idealProgress + progressOffset
            )
            {
                if (thisNoteIndex == noteIndex)
                {
                    Debug.Log("Obj name: " + noteObj.name);
                    noteObj.GetComponentInChildren<Image>().color = Color.green;
                }
            }
        }
    }
}
