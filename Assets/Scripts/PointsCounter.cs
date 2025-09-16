using TMPro;
using UnityEngine;

public class PointsCounter : MonoBehaviour
{
    public int Points => (int)points;
    public float Energy => energy;
    private float points;
    private float energy;

    [SerializeField]
    float energyPerNote = 20f;

    [SerializeField]
    float energyLossMultiplier = 0.3f;

    [SerializeField]
    float pointsPerNote = 10f;

    [SerializeField]
    float enegyDecayPerSecond = 2f;

    /*
    ENERGY LOGIC
    
    0-50 => 1x
    51-200 => 2x
    200-500 => 4x
    501 => 8x

    */

    [SerializeField]
    TMP_Text pointsText;

    [SerializeField]
    TMP_Text multiplierText;

    void FixedUpdate()
    {
        energy = Mathf.Clamp(energy - enegyDecayPerSecond * Time.fixedDeltaTime, 0, float.MaxValue);
        UpdateText();
    }

    public void HitNote(Note note)
    {
        int noteDuration = note.NoteDuration;
        float energyGained = noteDuration++ * energyPerNote;
        float pointsGained = noteDuration++ * pointsPerNote;
        energy += energyGained;

        AddPoints(pointsGained);
    }

    public void MissNote() => energy *= energyLossMultiplier;

    private void AddPoints(float value)
    {
        int multiplier = GetMultiplier();
        points += value * multiplier;
    }

    private int GetMultiplier()
    {
        if (energy > 500)
            return 8;
        else if (energy > 200)
            return 4;
        else if (energy > 50)
            return 2;
        else
            return 1;
    }

    void UpdateText()
    {
        pointsText.text = Points.ToString();
        multiplierText.text = GetMultiplier().ToString() + "x";
    }
}
