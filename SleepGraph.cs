using UnityEngine;
using System.Collections.Generic;

public class SleepGraph : MonoBehaviour {
    private TimeOfDay startTime;
    private TimeOfDay endTime;
    [SerializeField] private GameObject linePrefab;
    private List<SleepDepth> sleepCycle = new List<SleepDepth>();
    private float yAxisGap;
    private float xAxisGap;
    private const int MaxSleepLevel = 3;
    private const int MinuteInterval = 60;
    private Transform lineParent;
    private Vector3[] corners = new Vector3[4];

    public void SetCycles() {
        startTime = GameManager.Instance.StartTime;
        endTime = GameManager.Instance.EndTime;
        SetFirstSleepDepth();
        SetSleepDepthEveryInterval();
        SetLastSleepDepth();
    }

    public void DrawAt(RectTransform container) {
        container.GetWorldCorners(corners);
        MakeLineParent(container);
        SetXYAxisGap(container);
        DrawGraph();
    }

    private void DrawGraph() {
        List<Vector2> points = new List<Vector2>();
        SetPoints(ref points);
        DrawLines(points);
    }

    private void CreateLine(Vector2 start, Vector2 end) {
        GameObject lineObject = Instantiate(linePrefab, lineParent);
        Line line = lineObject.GetComponent<Line>();
        line.Draw(start, end, 1.5f, Color.white);
    }

    private void DrawLines(List<Vector2> points) {
        for (int i = 0; i < points.Count - 1; i++) {
            CreateLine(points[i], points[i + 1]);
        }
    }

    private void SetPoints(ref List<Vector2> points) {
        Vector2 currentPoint = (Vector2)corners[0] - new Vector2(30f / MinuteInterval * xAxisGap, 0);
        for(int i = 0; i < sleepCycle.Count; ++i) {
            Vector2 targetPoint = currentPoint;
            int yIntervalCount = (i == 0) ? (MaxSleepLevel - sleepCycle[i].Level) : (sleepCycle[i - 1].Level - sleepCycle[i].Level);
            targetPoint.y += yIntervalCount *  yAxisGap;
            points.Add(targetPoint);
            float xIntervalCount = (sleepCycle[i].EndTime.ToSeconds() - sleepCycle[i].StartTime.ToSeconds()) / (MinuteInterval * 60f);
            targetPoint.x += xIntervalCount * xAxisGap;
            points.Add(targetPoint);
            currentPoint = targetPoint;
        }
    }

    private void MakeLineParent(RectTransform container) {
        GameObject emptyObject = new GameObject("Lines");
        emptyObject.AddComponent<RectTransform>();
        lineParent = emptyObject.transform;
        lineParent.SetParent(container, false);
        lineParent.SetSiblingIndex(0);
    }

    private void SetXYAxisGap(RectTransform container) {
        xAxisGap = container.rect.width / (sleepCycle.Count - 2);
        yAxisGap = container.rect.height / (MaxSleepLevel + 1);
    }

    private void SetFirstSleepDepth() {
        TimeOfDay sleepBeforeTime = startTime - new TimeOfDay(0, 30);
        TimeOfDay sleepStartTime = new TimeOfDay(startTime);
        SleepDepth firstSleepDepth = new SleepDepth(-1, sleepBeforeTime, sleepStartTime);
        sleepCycle.Add(firstSleepDepth);
    }

    private void SetSleepDepthEveryInterval() {
        int sleepSeconds = endTime.ToSeconds() - startTime.ToSeconds();
        int depthCount = sleepSeconds / (MinuteInterval * 60);
        TimeOfDay currentTime = startTime;
        TimeOfDay nextTime = currentTime + new TimeOfDay(0, MinuteInterval);
        for(int i = 0; i < depthCount; ++i) {
            int randomLevel = Random.Range(0, MaxSleepLevel + 1);
            SleepDepth sleepDepth = new SleepDepth(randomLevel, currentTime, nextTime);
            sleepCycle.Add(sleepDepth);
            currentTime = nextTime;
            nextTime = currentTime + new TimeOfDay(0, MinuteInterval);
            if(nextTime > endTime) {
                nextTime = endTime;
            }
        }
    }

    private void SetLastSleepDepth() {
        TimeOfDay sleepEndTime = new TimeOfDay(endTime);
        TimeOfDay sleepAfterTime = endTime + new TimeOfDay(0, 30);
        SleepDepth lastSleepDepth = new SleepDepth(-1, sleepEndTime, sleepAfterTime);
        sleepCycle.Add(lastSleepDepth);
    }
}
