using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public Music currentMusic;

    [SerializeField] private AudioSource audioSource;
    
    private float startTime;
    private float pausedTime;
    private bool isPlaying = false;
    private bool audioLoadedSuccessfully = false;

    public void LoadMusic(Music music)
    {
        currentMusic = music;
        audioLoadedSuccessfully = false;

        if (music != null && music.audioClip != null)
        {
            // Validate the audio clip
            if (IsAudioClipValid(music.audioClip))
            {
                audioSource.clip = music.audioClip;
                Debug.Log($"Successfully loaded audio clip: {music.audioClip.name}, length: {music.audioClip.length}s, channels: {music.audioClip.channels}, frequency: {music.audioClip.frequency}");
                audioLoadedSuccessfully = true;
            }
        }
    }

    private bool IsAudioClipValid(AudioClip clip)
    {
        // Basic validation checks
        if (clip == null) return false;
        if (clip.length <= 0) return false;
        if (clip.channels <= 0) return false;
        if (clip.frequency <= 0) return false;

        return true;
    }

    public void Play()
    {
        if (!audioLoadedSuccessfully)
        {
            Debug.LogError("Cannot play - audio not loaded successfully");
            return;
        }

        if (!isPlaying)
        {
            try
            {
                audioSource.time = pausedTime;

                if (audioSource.clip != null)
                    audioSource.Play();

                startTime = Time.time - pausedTime;
                isPlaying = true;
                pausedTime = 0f;
                Debug.Log($"Music started playing at time: {audioSource.time}s");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error playing music: {e.Message}");
                isPlaying = false;
            }
        }
    }

    public void Pause()
    {
        if (isPlaying)
        {
            pausedTime = GetCurrentTime();
            audioSource.Pause();
            isPlaying = false;
        }
    }

    public void Resume()
    {
        if (!isPlaying)
        {
            Play();
        }
    }

    public void Stop()
    {
        audioSource.Stop();
        isPlaying = false;
        startTime = 0f;
        pausedTime = 0f;
    }

    public float GetCurrentTime()
    {
        if (isPlaying)
        {
            return Time.time - startTime;
        }
        else
        {
            return pausedTime;
        }
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    public bool IsFinished()
    {
        if (currentMusic == null || currentMusic.notes.Count == 0)
            return true;

        float currentTime = GetCurrentTime();
        float lastNoteTime = currentMusic.notes[currentMusic.notes.Count - 1].time;

        return currentTime > lastNoteTime + 5f; // 5 seconds after last note
    }
}
