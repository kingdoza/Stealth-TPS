using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class StageManager : MonoBehaviour {
    public TimeOfDay CurrentTime { get; private set; }
    //public Player Player { get; private set; }
    public int Resistance { get; private set; } = 0;
    [SerializeField] private float playTimeMinute = 16;
    public float PlayTimeMinute => playTimeMinute;
    private float realTimeScale;
    public bool IsStage { get; private set; } = true;
    [SerializeField] private GameObject targetEnemyPrefab;
    [SerializeField] private GameObject normalEnemyPrefab;
    [SerializeField] private GameObject playerPrefab;
    private List<Enemy> enemyList = new List<Enemy>();
    public IReadOnlyList<Enemy> EnemyList => enemyList;
    private Client currentClient;

    // private void Awake() {
    //     Player = FindObjectOfType<PlayerController>();
    //     CurrentTime = GameManager.Instance.StartTime;
    //     SetRealTimeScale();
    //     StartCoroutine(KeepCalculatingGameTime());

    //     for(int i = 0; i < 5; ++i) {
    //         TargetEnemy targetEnemy = Instantiate(targetEnemyPrefab, Player.Position, Quaternion.identity).GetComponentInChildren<TargetEnemy>();
    //         targetEnemy.Feeling = Random.Range(0, 100);
    //         totemSlots[i].Totem = new Totem {
    //             targetEnemy = targetEnemy,
    //             Activation = false
    //         };
    //     }
    // }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            SetMainGame();
        }
    }

    public void SetMainGame() {
        currentClient = GameManager.Instance.clientManager.CurrentClient;
        MakePlayer();
        CurrentTime = GameManager.Instance.StartTime;
        SetRealTimeScale();
        StartCoroutine(KeepCalculatingGameTime());
        GameManager.Instance.uiManager.SetTotemSlots(currentClient.Totems);
        MakeEnemies();
        GameManager.Instance.uiManager.ApplyTimeToSleepCycle();
        IStageRun[] stageRuns = FindObjectsOfType<MonoBehaviour>().OfType<IStageRun>().ToArray();
        foreach (var stageRun in stageRuns) {
            stageRun.Run();
        }
    }

    private void MakePlayer() {
        Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
    }

    private void MakeEnemies() {
        MakeTargetEnemies();
        MakeNormalEnemies();
    }

    private void MakeTargetEnemies() {
        for(int i = 0; i < currentClient.Totems.Length; ++i) {
            Enemy enemy = MakeEnemy(targetEnemyPrefab, currentClient.Totems[i].EnemyVariables);
            //TargetEnemy targetEnemy = Instantiate(targetEnemyPrefab, Vector3.zero, Quaternion.identity).GetComponentInChildren<TargetEnemy>();
            //targetEnemy.Feeling = currentClient.Totems[i].EnemyVariables.Feeling;
            //targetEnemy.Importance = currentClient.Totems[i].EnemyVariables.Importance;
            currentClient.Totems[i].TargetEnemy = enemy as TargetEnemy;
            currentClient.Totems[i].Activation = false;
            //enemyList.Add(targetEnemy);
        }
    }

    private void MakeNormalEnemies() {
        for(int i = 0; i < 0; ++i) {
            int randomFeeling = currentClient.GetRandomFeeling();
            EnemyVariables enemyVariables = new EnemyVariables(randomFeeling, 50);
            Enemy enemy = MakeEnemy(normalEnemyPrefab, enemyVariables);
        }
    }

    private Enemy MakeEnemy(GameObject enemyPrefab, EnemyVariables enemyVariables) {
        GameObject enemyObj = Instantiate(enemyPrefab, GetRandomNavMeshPos(20), GetRandomRotation());
        Enemy enemy = enemyObj.GetComponentInChildren<Enemy>();
        enemy.Feeling = enemyVariables.Feeling;
        enemy.Importance = enemyVariables.Importance;
        enemyList.Add(enemy);
        return enemy;
    }

    private Vector3 GetRandomNavMeshPos(float radius) {
        Vector2 randomPos2D = UnityEngine.Random.insideUnitCircle * radius;
        Vector3 randomPos = new Vector3(transform.position.x + randomPos2D.x, 0, transform.position.z + randomPos2D.y);
        NavMeshHit hit;
        while(!NavMesh.SamplePosition(randomPos, out hit, 1.0f, NavMesh.AllAreas)) {
            randomPos2D = UnityEngine.Random.insideUnitCircle * radius;
            randomPos = new Vector3(transform.position.x + randomPos2D.x, 0, transform.position.z + randomPos2D.y);
        }
        return hit.position;
    }

    private Quaternion GetRandomRotation() {
        float randomYRotation = Random.Range(0f, 360f);
        return Quaternion.Euler(0f, randomYRotation, 0f);
    }

    // private void Start() {
    //     GameManager.Instance.uiManager.ApplyTimeToSleepCycle();
    // }

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
