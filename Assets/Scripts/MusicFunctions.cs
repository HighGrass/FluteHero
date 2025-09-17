using UnityEngine;

public static class MusicFunctions
{
    public static float GetTickDuration(int bpm, int ticksPerBeat)
    {
        return 60f / (bpm * (float)ticksPerBeat); // duração de 1 tick em segundos
    }

    public static float GetTimeFromTick(int tick, int bpm, int ticksPerBeat)
    {
        float tickDuration = GetTickDuration(bpm, ticksPerBeat);
        return tick * tickDuration;
    }

    public static int GetTickFromTime(float time, int bpm, int ticksPerBeat)
    {
        float tickDuration = GetTickDuration(bpm, ticksPerBeat);
        return Mathf.FloorToInt(time / tickDuration);
    }

    public static float GetSyncTiming(float time, int bpm, int ticksPerBeat)
    {
        float tickDuration = GetTickDuration(bpm, ticksPerBeat);
        int tick = Mathf.FloorToInt(time / tickDuration);
        return tick * tickDuration;
    }

    public static int ConvertKeyToNumber(KeyCode key)
    {
        int number = -1;
        switch (key)
        {
            case KeyCode.Alpha1:
                number = 1;
                break;
            case KeyCode.Alpha2:
                number = 2;
                break;
            case KeyCode.Alpha3:
                number = 3;
                break;
            case KeyCode.Alpha4:
                number = 4;
                break;
            case KeyCode.Alpha5:
                number = 5;
                break;
            case KeyCode.Alpha6:
                number = 6;
                break;
        }
        return number;
    }

    public static int GetNoteSpriteIndex(int noteIndex, bool pressNote = false)
    {
        if (pressNote)
        {
            if (noteIndex % 2 == 0)
                return 2;
            else
                return 3;
        }
        else
        {
            if (noteIndex % 2 == 0)
                return 0;
            else
                return 1;
        }
    }
}
