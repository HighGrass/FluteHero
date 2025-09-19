using UnityEngine;
using UnityEngine.Audio;

public class VolumeSlider : MonoBehaviour
{
    public GameObject[] volumeFill;
    public AudioMixer audioMixer;
    int volumeLevel = 5;

    private void Start()
    {
        if (PlayerPrefs.HasKey("VolumeLevel"))
            volumeLevel = PlayerPrefs.GetInt("VolumeLevel", 5);
        else
            volumeLevel = 5;

        UpdateMixer();

        UpdateUI();
    }

    public void AddVolume()
    {
        volumeLevel++;

        if (volumeLevel > 9)
            volumeLevel = 9;

        UpdateMixer();

        UpdateUI();

    }

    public void SubtractVolume()
    {
        volumeLevel--;

        if (volumeLevel < 0)
            volumeLevel = 0;

        UpdateMixer();

        UpdateUI();
    }

    private void UpdateMixer()
    {
        float value = volumeLevel;

        // [0-10] to [0-2]
        value *= 0.2f;

        // Ensure the volume is not too small or negative to avoid invalid calculations
        if (value <= 1e-5f)
            value = 1e-5f; // Clamp to the lower bound

        // Convert the linear volume (0.0 to 1.0) to a decibel scale using a logarithmic function
        // Unity’s audio mixer expects decibel values. `20 * Mathf.Log10(v)` converts:
        // - Linear input of 1.0 to 0 dB (no attenuation).
        // - Linear input < 1.0 to negative decibels (attenuated volume).
        // - Values near 0 are clamped to approximately -80 dB.
        audioMixer.SetFloat("Volume", Mathf.Log10(value) * 20);
    }

    private void UpdateUI()
    {
        foreach (GameObject fill in volumeFill)
        {
            fill.SetActive(false);
        }
        if (volumeLevel > 0)
            volumeFill[volumeLevel - 1].SetActive(true);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("VolumeLevel", volumeLevel);
    }
}
