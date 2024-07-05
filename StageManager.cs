using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour {
    public TimeOfDay CurrentTime { get; private set; }
    [SerializeField] private float playTimeMinute = 16;
    public float PlayTimeMinute => playTimeMinute;
    private float realTimeScale;
    public bool IsStage { get; private set; } = true;

    private void Awake() {
        CurrentTime = GameManager.Instance.StartTime;
        SetRealTimeScale();
        StartCoroutine(KeepCalculatingGameTime());
    }

    private void Start() {
        GameManager.Instance.uiManager.ApplyTimeToSleepCycle();
    }

    private IEnumerator KeepCalculatingGameTime() {
        while (CurrentTime < GameManager.Instance.EndTime) {
            yield return new WaitForSeconds(1);
            CurrentTime += new TimeOfDay(0, 0, (int)realTimeScale);
            if(CurrentTime >= GameManager.Instance.EndTime) {
                CurrentTime = GameManager.Instance.EndTime;
            }
            GameManager.Instance.uiManager.ApplyTimeToSleepCycle();
        }
        IsStage = false;
    }

    private void SetRealTimeScale() {
        float totalSeconds = GameManager.Instance.EndTime.ToSeconds() - GameManager.Instance.StartTime.ToSeconds();
        realTimeScale = totalSeconds / 60 / playTimeMinute;
    }

    public float GetProgress() {
        float elapsedSeconds = (CurrentTime - GameManager.Instance.StartTime).ToSeconds();
        float totalSeconds = (GameManager.Instance.EndTime - GameManager.Instance.StartTime).ToSeconds();
        return elapsedSeconds / totalSeconds;
    }
}
