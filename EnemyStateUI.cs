using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStateUI : MonoBehaviour, IStageRun {
    private IReadOnlyCollection<Enemy> enemyList;
    private List<RectTransform> stateList = new List<RectTransform>();
    [SerializeField] private GameObject statePrefab;
    private Camera mainCamera;
    [SerializeField] private float stateOffset = 50;
    private Dictionary<EnemyReaction, Color> reactionColors = new Dictionary<EnemyReaction, Color> {
        { EnemyReaction.None, Color.white },
        { EnemyReaction.Doubt, Color.yellow },
        { EnemyReaction.Search, new Color(1, 0.5f, 0) },
        { EnemyReaction.Alert, Color.red }
    };
    public bool IsRunning { get; set; } = false;

    private void Awake() {
        mainCamera = Camera.main;
        //MakeEnemyStates();
    }

    // private void Start() {
    //     MakeEnemyStates();
    // }

    public void Run() {
        IsRunning = true;
        MakeEnemyStates();
    }

    private void Update() {
        if(IsRunning == false)
            return;
        enemyList = GameManager.Instance.stageManager.EnemyList;
        FollowEnemy();
        ShowReactionState();
    }

    private void FollowEnemy() {
        int i = 0;
        foreach (Enemy enemy in enemyList) {
            if(stateList[i].gameObject.activeSelf == false) {
                ++i;
                continue;
            }
            SetStateUIPosition(enemy, i);
            ++i;
        }
    }

    private void ShowReactionState() {
        int i = 0;
        foreach (Enemy enemy in enemyList) {
            if(enemy.Reaction == EnemyReaction.None) {
                stateList[i++].gameObject.SetActive(false);
                continue;
            }
            SetStateUIPosition(enemy, i);
            stateList[i].gameObject.SetActive(true);
            stateList[i].GetChild(0).GetComponent<Image>().color = reactionColors[enemy.Reaction];
            ++i;
        }
    }

    private void MakeEnemyStates() {
        enemyList = GameManager.Instance.stageManager.EnemyList;
        for(int i = 0; i < enemyList.Count; ++i) {
            GameObject stateInstance = Instantiate(statePrefab, Vector3.zero, statePrefab.GetComponent<RectTransform>().rotation);
            stateList.Add(stateInstance.GetComponent<RectTransform>());
            stateInstance.transform.SetParent(transform);
            stateInstance.SetActive(false);
        }
    }

    private void SetStateUIPosition(Enemy enemy, int index) {
        Vector3 desiredPos = mainCamera.WorldToScreenPoint(enemy.Head.position);
        desiredPos += Vector3.up * stateOffset;
        ClampToScreenEdges(stateList[index], ref desiredPos);
        stateList[index].position = desiredPos;
    }

    private void ClampToScreenEdges(RectTransform uiElem, ref Vector3 desiredPos) {
        Vector3[] corners = new Vector3[4];
        uiElem.GetWorldCorners(corners);

        Vector3 leftBotRelativePos = corners[0] - uiElem.position;
        Vector3 rightTopRelativePos = corners[2] - uiElem.position;

        Vector3 min = desiredPos + leftBotRelativePos;
        Vector3 max = desiredPos + rightTopRelativePos;

        Vector2 screenMin = Vector2.zero;
        Vector2 screenMax = new Vector2(Screen.width, Screen.height);

        // 최소, 최대 값이 화면 경계를 벗어나는지 확인하고 조정합니다.
        if (min.x < screenMin.x) {
            desiredPos.x += screenMin.x - min.x;
        }
        if (max.x > screenMax.x) {
            desiredPos.x -= max.x - screenMax.x;
        }
        if (min.y < screenMin.y) {
            desiredPos.y += screenMin.y - min.y;
        }
        if (max.y > screenMax.y) {
            desiredPos.y -= max.y - screenMax.y;
        }
    }
}
