using UnityEngine;

public static class MusicFunctions
{
    public static float GetSyncTiming(float time, int bpm)
    {
        float baseNoteSize = 60f / bpm;
        int tick = (int)(time / baseNoteSize);
        float syncTime = tick * baseNoteSize;

        return syncTime;
    }

    public static int GetTickFromTime(float time, int bpm)
    {
        float baseNoteSize = 60f / bpm;
        return (int)(time / baseNoteSize);
    }

    public static float GetTimeFromTick(int tick, int bpm)
    {
        float baseNoteSize = 60f / bpm;
        return (int)(tick * baseNoteSize);
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

    public static int GetNoteSpriteIndex(int noteIndex)
    {
        if (noteIndex % 2 == 0)
            return 0;
        else
            return 1;
    }
}
