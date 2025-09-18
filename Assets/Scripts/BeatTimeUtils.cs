using System;
using UnityEngine;

public static class BeatTimeUtils
{
    // Convert beat notation "M.S" (measure.subdivision, both 1-based) to seconds.
    // beatsPerMeasure: e.g. 3 for 3/4
    // subdivisionsPerBeat: e.g. 4 for sixteenth-note resolution
    public static float BeatToSeconds(string beatNotation, int beatsPerMeasure, int subdivisionsPerBeat, float bpm)
    {
        if (string.IsNullOrEmpty(beatNotation)) throw new ArgumentException("beatNotation is null/empty");
        var parts = beatNotation.Split('.');
        if (parts.Length != 2) throw new FormatException("beatNotation must be in 'M.S' format (e.g. '1.3')");

        if (!int.TryParse(parts[0], out int measure) || measure <= 0)
            throw new FormatException("Invalid measure (must be positive integer).");

        if (!int.TryParse(parts[1], out int subIndex) || subIndex <= 0)
            throw new FormatException("Invalid subdivision index (must be positive integer).");

        int subsPerMeasure = beatsPerMeasure * subdivisionsPerBeat;
        // Allow subIndex up to subsPerMeasure (1..subsPerMeasure)
        if (subIndex > subsPerMeasure)
            throw new ArgumentOutOfRangeException($"subIndex must be in 1..{subsPerMeasure} for the given time signature and subdivisions");

        // total number of subdivisions before this position:
        int totalSubsBefore = (measure - 1) * subsPerMeasure + (subIndex - 1);

        float secondsPerSub = (60f / bpm) / subdivisionsPerBeat;
        return totalSubsBefore * secondsPerSub;
    }

    // Convert seconds to "M.S" using the same resolution. Will round to nearest subdivision.
    public static string SecondsToBeatNotation(float seconds, int beatsPerMeasure, int subdivisionsPerBeat, float bpm)
    {
        if (seconds < 0f) seconds = 0f;
        int subsPerMeasure = beatsPerMeasure * subdivisionsPerBeat;
        float secondsPerSub = (60f / bpm) / subdivisionsPerBeat;

        // nearest subdivision index starting from 0
        int totalSubs = Mathf.RoundToInt(seconds / secondsPerSub);

        int measure = totalSubs / subsPerMeasure + 1;
        int subInMeasure = totalSubs % subsPerMeasure + 1; // 1-based

        return $"{measure}.{subInMeasure}";
    }
}
