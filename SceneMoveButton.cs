using UnityEngine;
using UnityEngine.EventSystems;

public class SceneMoveButton : MonoBehaviour, IPointerClickHandler {
    [SerializeField] private string nextSceneName;

    public void OnPointerClick(PointerEventData eventData) {
        LoadingSceneController.LoadScene(nextSceneName);
    }
}
