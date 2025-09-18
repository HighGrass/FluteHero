using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FluteHeroGame : MonoBehaviour
{
    public MusicController musicController;

    [Header("UI Elements")]
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text statusText;
    public TMP_Text countdownText;
    public GameObject notePrefabEven;
    public GameObject notePrefabOdd;
    public RectTransform[] hitZones; // Array of 6 hit zones, one for each lane

    [Header("Game Settings")]
    public float travelTime = 2f; // Time for note to travel from spawn to hit
    public float hitRange = 10f; // Pixel range for hit detection
    public float spawnX = 1000f; // Local X position where notes spawn
    public float hitX = 0f; // Local X position where notes should be hit

    [Header("Timing")]
    public float initialDelay = 5f; // Delay before music starts

    [Header("Music Settings")]
    public string musicName = "example_song";

    private int score = 0;
    private List<GameObject> activeNoteObjects = new List<GameObject>();
    private Color[] laneColors = new Color[]
    {
        Color.red,
        new Color(1f, 0.65f, 0f), // Orange
        Color.yellow,
        Color.green,
        Color.blue,
        new Color(0.5f, 0f, 0.5f) // Purple
    };

    private float musicStartTime;
    private bool gameStarted = false;
    private MusicLoader musicLoader;
    private int nextNoteIndex = 0; // Track the next note to spawn
    [SerializeField]
    private Animator helathBarAnimator;
    private int hp = 10;
    public int HP
    {
        get
        {
            return hp;
        }
        set
        {
            hp = Mathf.Clamp(value, 0, 10);
            helathBarAnimator.SetInteger("HP", hp);
            if (hp == 0)
            {
                statusText.text = "Game Over! Press R to restart";
                musicController.Stop();
                gameStarted = false;
                nextNoteIndex = 0;
                // Reset all notes
                if (musicController.currentMusic != null)
                {
                    foreach (Note note in musicController.currentMusic.notes)
                    {
                        note.hit = false;
                        note.missed = false;
                        note.active = true;
                        if (note.noteObject != null)
                        {
                            Destroy(note.noteObject);
                        }
                    }
                }
            }
        }
    }

    private Coroutine startGameCoroutine;

    private void Start()
    {
        musicName = GameData.musicName;
        musicLoader = gameObject.AddComponent<MusicLoader>();
        InitializeHitZones();
        StartGameWithDelay();
    }

    private void InitializeHitZones()
    {
        // Ensure we have exactly 6 hit zones
        if (hitZones.Length != 6)
        {
            Debug.LogError("Need exactly 6 hit zones!");
            return;
        }

        for (int i = 0; i < hitZones.Length; i++)
        {
            if (hitZones[i] != null)
            {
                Image zoneImage = hitZones[i].GetComponentInChildren<Image>();
                if (zoneImage != null)
                {
                    zoneImage.color = new Color(laneColors[i].r, laneColors[i].g, laneColors[i].b, 1);
                }
            }
        }
    }

    private void StartGameWithDelay()
    {
        LoadMusic();
        startGameCoroutine = StartCoroutine(GameStartRoutine());
    }

    private IEnumerator GameStartRoutine()
    {
        // Set music start time to account for the countdown
        musicStartTime = Time.time;
        gameStarted = true;

        // Show countdown
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        float countdownEndTime = musicStartTime + initialDelay;

        while (Time.time < countdownEndTime)
        {
            float countdown = countdownEndTime - Time.time;
            if (countdownText != null)
                countdownText.text = $"Starting in: {countdown:F1}s";

            // Update notes during countdown
            UpdateNotes();

            yield return null;
        }

        // Hide countdown
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        // Start music - DON'T reset musicStartTime!
        musicController.Play();
        Debug.Log($"Music started in time: {musicController.GetCurrentTime()}s");
    }

    private void LoadMusic()
    {
        Music music = musicLoader.LoadMusic(musicName);

        if (music == null)
        {
            Debug.LogWarning("Failed to load music from AssetBundle, creating demo notes with dummy audio");
        }
        else if (music.audioClip == null)
        {
            Debug.LogWarning("Music loaded but audio clip is null, creating demo audio");
        }

        musicController.LoadMusic(music);

        // Verify audio clip was loaded
        if (musicController.currentMusic == null || musicController.currentMusic.audioClip == null)
        {
            Debug.LogError("Completely failed to load any audio.");
        }
    }


    private void Update()
    {
        // Always check for restart, even when game is not started/over
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
            return; // Restart after checking input
        }

        if (!gameStarted) return;

        // Debug: Press D to show note info
        if (Input.GetKeyDown(KeyCode.D))
        {
            DebugNoteInfo();
        }

        UpdateUI();
        UpdateNotes();

        // Only check input after music starts
        if (musicController.IsPlaying())
        {
            CheckInput();
        }

        if (musicController.IsFinished())
        {
            if (statusText != null)
                statusText.text = "Finished! Press R to restart";
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";

        if (timeText != null)
        {
            float currentTime = GetCurrentMusicTime();
            if (currentTime < 0)
                timeText.text = $"Time: -{Mathf.Abs(currentTime):F1}s";
            else
                timeText.text = $"Time: {Mathf.FloorToInt(currentTime)}s";
        }

        if (statusText != null)
        {
            if (GetCurrentMusicTime() < 0)
                statusText.text = "Countdown";
            else if (musicController.IsPlaying())
                statusText.text = "Playing";
            else
                statusText.text = "Paused";
        }
    }
    public float GetCurrentMusicTime()
    {
        if (!gameStarted) return -initialDelay;

        // Time starts from -initialDelay and counts up to 0 during countdown
        // After countdown, continues counting up positively
        return Time.time - musicStartTime - initialDelay;
    }

    private void UpdateNotes()
    {
        if (musicController.currentMusic == null || musicController.currentMusic.notes.Count == 0)
            return;

        float currentMusicTime = GetCurrentMusicTime();

        // Check if we have more notes to spawn
        while (nextNoteIndex < musicController.currentMusic.notes.Count)
        {
            Note nextNote = musicController.currentMusic.notes[nextNoteIndex];

            // Calculate when this note should spawn (travelTime seconds before it should be hit)
            float spawnTime = nextNote.time - travelTime;

            // If it's time to spawn this note
            if (currentMusicTime >= spawnTime)
            {
                CreateNoteObject(nextNote);
                nextNoteIndex++;
            }
            else
            {
                // Notes are sorted by time, so we can break early
                break;
            }
        }

        // Update all active notes
        for (int i = 0; i < nextNoteIndex; i++)
        {
            Note note = musicController.currentMusic.notes[i];
            if (note.active && note.noteObject != null)
            {
                UpdateNotePosition(note, currentMusicTime);
            }
        }
    }

    private void CreateNoteObject(Note note)
    {
        if (notePrefabEven == null || notePrefabOdd == null || hitZones.Length <= note.lane || hitZones[note.lane] == null)
            return;

        // Get the correct hit zone for this lane
        RectTransform hitZone = hitZones[note.lane];

        GameObject prefab = note.lane == 0 || note.lane % 2 == 0 ? notePrefabEven : notePrefabOdd;

        GameObject noteObj = Instantiate(prefab, hitZone);

        // Set note color based on lane
        Image noteImage = noteObj.GetComponent<Image>();
        if (noteImage != null)
        {
            noteImage.color = laneColors[note.lane];
        }

        // Calculate initial position based on current time
        float currentMusicTime = GetCurrentMusicTime();
        float spawnTime = note.time - travelTime;
        float initialProgress = (currentMusicTime - spawnTime) / travelTime;

        // Clamp progress to reasonable values
        initialProgress = Mathf.Clamp(initialProgress, -1f, 1f);

        float initialXPos = Mathf.Lerp(spawnX, hitX, initialProgress);

        // Position note at calculated initial position
        noteObj.transform.localPosition = new Vector3(initialXPos, 0, 0);

        note.noteObject = noteObj;
        activeNoteObjects.Add(noteObj);

        // Debug.Log($"Spawned note for lane {note.lane} at time {note.time:F2}. " +
        //           $"Current music time: {currentMusicTime:F2}, " +
        //           $"Spawn time: {spawnTime:F2}, " +
        //           $"Initial progress: {initialProgress:F2}, " +
        //           $"Initial X: {initialXPos:F2}");
    }

    private void DebugNoteInfo()
    {
        if (musicController.currentMusic == null) return;

        float currentMusicTime = GetCurrentMusicTime();
        Debug.Log($"Current music time: {currentMusicTime:F2}s");
        Debug.Log($"Next note index: {nextNoteIndex}");

        if (nextNoteIndex < musicController.currentMusic.notes.Count)
        {
            Note nextNote = musicController.currentMusic.notes[nextNoteIndex];
            float spawnTime = nextNote.time - travelTime;
            Debug.Log($"Next note: lane {nextNote.lane}, time {nextNote.time:F2}, spawn time {spawnTime:F2}");
        }

        for (int i = 0; i < Mathf.Min(3, musicController.currentMusic.notes.Count); i++)
        {
            Note note = musicController.currentMusic.notes[i];
            Debug.Log($"Note {i}: lane {note.lane}, time {note.time:F2}, active {note.active}");
        }
    }

    private void UpdateNotePosition(Note note, float currentMusicTime)
    {
        if (note.noteObject == null) return;

        // Calculate progress
        float spawnTime = note.time - travelTime;
        float progress = (currentMusicTime - spawnTime) / travelTime;

        // Clamp progress to avoid extreme values
        progress = Mathf.Clamp(progress, -1f, 1.5f);

        // Calculate X position from spawnX to hitX
        float xPos = Mathf.Lerp(spawnX, hitX, progress);

        // Update note position (keep Y at 0 since it's in the hit zone)
        note.noteObject.transform.localPosition = new Vector3(xPos, 0, 0);


        if (!note.hit && !note.missed && currentMusicTime > note.time + 0.05f)
        {
            note.noteObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        }

        if (!note.hit && !note.missed && currentMusicTime > note.time + hitRange + 0.05f)
        {
            note.missed = true;
            note.active = false;
            if (note.noteObject != null)
            {
                Destroy(note.noteObject);
            }
            HP -= 1;
            Debug.Log($"Missed note in lane {note.lane} at time {note.time}");
        }
    }

    private void CheckInput()
    {
        float currentMusicTime = GetCurrentMusicTime();

        // Only check input after music starts (positive time)
        if (currentMusicTime < 0) return;

        // Pause/Resume game
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePause();
        }
    }

    public void CheckNoteHit(int lane, float currentMusicTime)
    {
        hitZones[lane]?.GetComponentInChildren<Animator>()?.SetTrigger("Hit");

        if (musicController.currentMusic == null) return;

        // Check all notes in the current lane
        for (int i = 0; i < nextNoteIndex; i++)
        {
            Note note = musicController.currentMusic.notes[i];

            if (note.active && !note.hit && !note.missed && note.lane == lane && note.noteObject != null)
            {
                float timeDifference = Mathf.Abs(note.time - currentMusicTime);

                // Check if note is in hit range (both position and time)
                if (timeDifference < hitRange)
                {
                    // Successful hit!
                    note.hit = true;
                    note.active = false;

                    // Calculate score based on accuracy
                    int points = CalculateScore(timeDifference);
                    score += points;

                    if (note.noteObject != null)
                    {
                        Destroy(note.noteObject);
                    }

                    if (HP < 10)
                        HP += 1;
                    Debug.Log($"Hit note in lane {lane} at time {note.time} with accuracy: {timeDifference:F2}s, Score: +{points}");
                    return;
                }
            }
        }
    }

    private int CalculateScore(float timeDifference)
    {
        // Perfect hit (within 0.05 seconds)
        if (timeDifference < 0.05f)
            return 100;
        // Good hit (within 0.1 seconds)
        else if (timeDifference < 0.1f)
            return 75;
        // Okay hit (within 0.2 seconds)
        else if (timeDifference < 0.2f)
            return 50;
        // Barely hit (within 0.3 seconds)
        else if (timeDifference < 0.3f)
            return 25;

        return 0;
    }

    public void TogglePause()
    {
        if (musicController.IsPlaying())
        {
            musicController.Pause();
            statusText.text = "Paused";
        }
        else
        {
            musicController.Resume();
            statusText.text = "Playing";
        }
    }

    public void Restart()
    {
        HP = 10;
        musicController.Stop();
        score = 0;
        gameStarted = false;
        nextNoteIndex = 0;
        StopCoroutine(startGameCoroutine);
        startGameCoroutine = null;

        // Reset all notes
        if (musicController.currentMusic != null)
        {
            foreach (Note note in musicController.currentMusic.notes)
            {
                note.hit = false;
                note.missed = false;
                note.active = true;
                if (note.noteObject != null)
                {
                    Destroy(note.noteObject);
                }
            }
        }

        // Clean up any remaining note objects
        foreach (GameObject noteObj in activeNoteObjects)
        {
            if (noteObj != null)
            {
                Destroy(noteObj);
            }
        }
        activeNoteObjects.Clear();

        StartGameWithDelay();
    }

    private void OnDestroy()
    {
        // Clean up
        foreach (GameObject noteObj in activeNoteObjects)
        {
            if (noteObj != null)
            {
                Destroy(noteObj);
            }
        }
    }
}
