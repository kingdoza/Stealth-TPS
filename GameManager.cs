using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager> {
    [HideInInspector] public SoundManager soundManager;
    [HideInInspector] public UI_Manager uiManager;
    [HideInInspector] public StageManager stageManager;
    [HideInInspector] public Canvas canvas;
    private List<Enemy> enemyList = new List<Enemy>();
    public IReadOnlyList<Enemy> EnemyList => enemyList;
    private TimeOfDay startTime = new TimeOfDay(22, 0);
    private TimeOfDay endTime = new TimeOfDay(30, 0);
    public TimeOfDay StartTime => startTime;
    public TimeOfDay EndTime => endTime;

    private void Awake() {
        soundManager = GetComponentInChildren<SoundManager>();
        uiManager = GetComponentInChildren<UI_Manager>();
        stageManager = GetComponentInChildren<StageManager>();
        canvas = FindObjectOfType<Canvas>();
        enemyList = FindObjectsOfType<Enemy>().ToList();
        //Debug.Log(enemyList.Count);
    }
}
