using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour {
    [SerializeField] private string mainGameScene; // 감지하고자 하는 특정 씬의 이름

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log(scene.name);
        Debug.Log(mainGameScene);
        if (scene.name == mainGameScene) {
            GameManager.Instance.MainGameStart();
        }
    }
}
