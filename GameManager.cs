using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager> {
    [HideInInspector] public SoundManager soundManager;
    [HideInInspector] public UI_Manager uiManager;
    private List<Enemy> enemyList = new List<Enemy>();
    public IReadOnlyList<Enemy> EnemyList => enemyList;

    private void Awake() {
        soundManager = GetComponentInChildren<SoundManager>();
        uiManager = GetComponentInChildren<UI_Manager>();
        enemyList = FindObjectsOfType<Enemy>().ToList();
        //Debug.Log(enemyList.Count);
    }
}
