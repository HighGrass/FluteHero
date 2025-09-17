using UnityEngine;

[System.Serializable]
public class Note
{
    public float time; // Original time in the chart
    public int lane;   // Which lane (0-5)

    [System.NonSerialized] public float actualTime; // Time adjusted for initial delay
    [System.NonSerialized] public float spawnTime; // When the note was actually spawned
    [System.NonSerialized] public bool hit = false;
    [System.NonSerialized] public bool missed = false;
    [System.NonSerialized] public bool active = true;
    [System.NonSerialized] public GameObject noteObject;

    public Note(float time, int lane)
    {
        this.time = time;
        this.lane = lane;
    }
}
