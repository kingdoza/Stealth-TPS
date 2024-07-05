using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GraphDrawer : MonoBehaviour {
    private TimeOfDay startTime;
    private TimeOfDay endTime;
    private RectTransform graphContainer;
    [SerializeField] private GameObject linePrefab;
    private List<SleepDepth> sleepCycle = new List<SleepDepth>();
    private float yAxisGap;
    private float xAxisGap;
    private const int MaxSleepLevel = 4;
    private const int MinuteInterval = 60;
    private Transform lineParent;
    [SerializeField] private TextMeshProUGUI timeUI;
    [SerializeField] private TextMeshProUGUI ampmUI;
    [SerializeField] private RectTransform timeLine;
    private StageManager stageManager;
    private Vector3[] corners = new Vector3[4];

    private void Awake() {
        graphContainer = GetComponent<RectTransform>();
        graphContainer.GetWorldCorners(corners);
        startTime = GameManager.Instance.StartTime;
        endTime = GameManager.Instance.EndTime;
        lineParent = transform.GetChild(0);
        SetFirstSleepDepth();
        SetSleepDepthEveryInterval();
        SetLastSleepDepth();

        xAxisGap = graphContainer.rect.width / (sleepCycle.Count - 2);
        yAxisGap = graphContainer.rect.height / (MaxSleepLevel + 1);
        DrawGraph();
        timeUI.text = "AM";
    }

    private void Start() {
        stageManager = GameManager.Instance.stageManager;
        StartCoroutine(KeepProcessingTimeLine());
    }

    private IEnumerator KeepProcessingTimeLine() {
        Vector2 startPos = corners[0];
        Vector2 endPos = corners[3];
        float totalSecond = stageManager.PlayTimeMinute * 60;
        float elapsedSecond = 0;
        while(stageManager.IsStage) {
            timeLine.position = Vector3.Lerp(startPos, endPos, elapsedSecond / totalSecond);
            elapsedSecond += Time.deltaTime;
            yield return null;
        }
        timeLine.position = endPos;
    }

    public void ApplyTime() {
        TimeOfDay currentTime = stageManager.CurrentTime;
        ampmUI.text = currentTime.GetAMPM();
        timeUI.text = currentTime.ToStringAMPM();
    }

    private void DrawGraph() {
        List<Vector2> points = new List<Vector2>();
        SetPoints(ref points);
        DrawLines(points);
    }

    private void SetPoints(ref List<Vector2> points) {
        //Vector3[] corners = new Vector3[4];
        //graphContainer.GetWorldCorners(corners);
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

    private void DrawLines(List<Vector2> points) {
        for (int i = 0; i < points.Count - 1; i++) {
            CreateLine(points[i], points[i + 1]);
        }
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

    private void CreateLine(Vector2 start, Vector2 end) {
        GameObject lineObject = Instantiate(linePrefab, lineParent);
        Line line = lineObject.GetComponent<Line>();
        line.Draw(start, end, 1.5f, Color.white);
    }
}