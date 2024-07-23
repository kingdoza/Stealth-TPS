using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour {
    private static string nextScene;
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI percentage;

    public static void LoadScene(string nextSceneName) {
        SceneManager.LoadScene("LoadingScene");
        nextScene = nextSceneName;
    }

    private void Start() {
        Debug.Log("Load staart");
        StartCoroutine(KeepLoadingScene());
    }

    private IEnumerator KeepLoadingScene() {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        
        while (!op.isDone) {
            Debug.Log(op.progress);
            timer += Time.deltaTime;
            if (op.progress < 0.9f) {
                float progress = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                percentage.text = progress.ToPercentage();
                progressBar.fillAmount = progress;
                if (progressBar.fillAmount >= op.progress) {
                    timer = 0f;
                }
            }
            else {
                float progress = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                percentage.text = progress.ToPercentage();
                progressBar.fillAmount = progress;
                if (progressBar.fillAmount == 1.0f) {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
            yield return null;
        }
    }
}
