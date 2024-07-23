using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager> {
    [HideInInspector] public SoundManager soundManager;
    [HideInInspector] public UI_Manager uiManager;
    [HideInInspector] public StageManager stageManager;
    [HideInInspector] public ClientManager clientManager;
    [HideInInspector] public SceneLoadManager sceneLoadManager;
    [HideInInspector] public Canvas canvas;
    //private List<Enemy> enemyList = new List<Enemy>();
    //public IReadOnlyList<Enemy> EnemyList => enemyList;
    private TimeOfDay startTime = new TimeOfDay(22, 0);
    private TimeOfDay endTime = new TimeOfDay(30, 0);
    public TimeOfDay StartTime => startTime;
    public TimeOfDay EndTime => endTime;

    protected override void Awake() {
        base.Awake();
        soundManager = GetComponentInChildren<SoundManager>();
        uiManager = GetComponentInChildren<UI_Manager>();
        stageManager = GetComponentInChildren<StageManager>();
        clientManager = GetComponentInChildren<ClientManager>();
        if(clientManager != null)
            DontDestroyOnLoad(clientManager.gameObject);
        sceneLoadManager = GetComponentInChildren<SceneLoadManager>();
        //loadingSceneManager = GetComponentInChildren<LoadingSceneController>();
        canvas = FindObjectOfType<Canvas>();
        //enemyList = FindObjectsOfType<Enemy>().ToList();
        //Debug.Log(enemyList.Count);
    }

    public void MainGameStart() {
        TryChangeChildMgr(ref uiManager);
        stageManager.SetMainGame();
    }

    private void TryChangeChildMgr<T>(ref T childMgr) where T : MonoBehaviour {
        T newChildMgr = Utility.FindNonChildObjectOfType<T>();
        if(newChildMgr != null)
            childMgr = newChildMgr;
    }
}
