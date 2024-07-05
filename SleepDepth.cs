using System;

public class SleepDepth {
    private int level;
    public int Level => level;
    private TimeOfDay startTime;
    public TimeOfDay StartTime => startTime;
    private TimeOfDay endTime;
    public TimeOfDay EndTime => endTime;

    public SleepDepth(int level, TimeOfDay startTime, TimeOfDay endTime) {
        this.level = level;
        this.startTime = startTime;
        this.endTime = endTime;
    }

    public override string ToString() {
        return "SleepDepth, " + level + ", " + startTime.ToString() + ", " + endTime.ToString();
    }
}
