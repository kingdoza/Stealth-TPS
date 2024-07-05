using System;
using UnityEngine;

[Serializable]
public class TimeOfDay : IComparable<TimeOfDay> {
    [SerializeField] private int hour;
    [SerializeField] private int minute;
    [SerializeField] private int second;
    public int Hour => hour;
    public int Minute => minute;
    public int Second => second;

    public TimeOfDay(int hour, int minute, int second) {
        this.hour = hour;
        this.minute = minute;
        this.second = second;
    }

    public TimeOfDay(int hour, int minute) :this(hour, minute, 0) { }

    public TimeOfDay(TimeOfDay copy) {
        this.hour = copy.hour;
        this.minute = copy.minute;
        this.second = copy.second;
    }

    public static TimeOfDay operator +(TimeOfDay time1, TimeOfDay time2) {
        int totalSeconds = time1.ToSeconds() + time2.ToSeconds();
        return MakeTimeFromSeconds(totalSeconds);
    }

    public static TimeOfDay operator -(TimeOfDay time1, TimeOfDay time2) {
        int totalSeconds = time1.ToSeconds() - time2.ToSeconds();
        return MakeTimeFromSeconds(totalSeconds);
    }

    public int ToSeconds() {
        return (hour * 3600) + (minute * 60) + second;
    }

    private static TimeOfDay MakeTimeFromSeconds(int totalSeconds) {
        int hours = (totalSeconds / 3600);
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;
        return new TimeOfDay(hours, minutes, seconds);
    }

    public static bool operator <(TimeOfDay a, TimeOfDay b) {
        return a.ToSeconds() < b.ToSeconds();
    }

    public static bool operator >(TimeOfDay a, TimeOfDay b) {
        return a.ToSeconds() > b.ToSeconds(); }

    public static bool operator <=(TimeOfDay a, TimeOfDay b) {
        return a.ToSeconds() <= b.ToSeconds();
    }

    public static bool operator >=(TimeOfDay a, TimeOfDay b) {
        return a.ToSeconds() >= b.ToSeconds();
    }

    public int CompareTo(TimeOfDay other) {
        return ToSeconds().CompareTo(other.ToSeconds());
    }

    public override string ToString() {
        return string.Format("{0:D2} : {1:D2}", hour, minute);
    }

    public string ToStringAMPM() {
        TimeOfDay timeOfDayAMPM  = new TimeOfDay(this);
        timeOfDayAMPM.hour = timeOfDayAMPM.hour % 12;
        return string.Format("{0:D2} : {1:D2}", timeOfDayAMPM.hour, timeOfDayAMPM.minute);
    }

    public string GetAMPM() {
        string ampm = "AM";
        TimeOfDay timeOfDayAMPM  = new TimeOfDay(this);
        while(timeOfDayAMPM.hour >= 12) {
            timeOfDayAMPM -= new TimeOfDay(12, 0);
            if(ampm == "AM")
                ampm = "PM";
            else
                ampm = "AM";
        }
        return ampm;
    }
}
