using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Music
{
    public string name;
    public float bpm;
    public List<Note> notes;
    public AudioClip audioClip;
    
    public Music(string name, float bpm, List<Note> notes, AudioClip audioClip)
    {
        this.name = name;
        this.bpm = bpm;
        this.notes = notes;
        this.audioClip = audioClip;
    }
}
