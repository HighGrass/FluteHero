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
}
