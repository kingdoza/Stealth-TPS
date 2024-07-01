using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStateUI : MonoBehaviour {
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

    private void Awake() {
        mainCamera = Camera.main;
        MakeEnemyStates();
    }

    private void Start() {
        
    }

    private void Update() {
        enemyList = GameManager.Instance.EnemyList;
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
            Vector3 targetPos = mainCamera.WorldToScreenPoint(enemy.Head.position);
            targetPos += Vector3.up * stateOffset;
            stateList[i].position = targetPos;
            ++i;
        }
    }

    private void ShowReactionState() {
        int i = 0;
        foreach (Enemy enemy in enemyList) {
            if(enemy.Reaction == EnemyReaction.None)
                stateList[i].gameObject.SetActive(false);
            else
                stateList[i].gameObject.SetActive(true);
            stateList[i].GetComponent<Image>().color = reactionColors[enemy.Reaction];
            ++i;
        }
    }

    private void MakeEnemyStates() {
        enemyList = GameManager.Instance.EnemyList;
        for(int i = 0; i < enemyList.Count; ++i) {
            GameObject stateInstance = Instantiate(statePrefab, Vector3.zero, statePrefab.GetComponent<RectTransform>().rotation);
            stateList.Add(stateInstance.GetComponent<RectTransform>());
            stateInstance.transform.SetParent(transform);
            stateInstance.SetActive(false);
        }
    }
}
