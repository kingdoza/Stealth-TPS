using UnityEngine;
using UnityEngine.UI;

public class Line : MonoBehaviour {
    private RectTransform rectTransform;
    private Image image;

    private void Awake() {
        rectTransform = gameObject.GetComponent<RectTransform>();
        image = gameObject.GetComponent<Image>();
    }

    public void Draw(Vector2 start, Vector2 end, float width, Color color) {
        image.color = color;
        rectTransform.sizeDelta = new Vector2(Vector2.Distance(start, end), width);
        rectTransform.position = (start + end) / 2;
        rectTransform.rotation = Quaternion.FromToRotation(Vector3.right, end - start);
    }
}
