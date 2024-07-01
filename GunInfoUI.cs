using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunInfoUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI currentBulletUI;
    [SerializeField] private TextMeshProUGUI remainedBulletUI;

    public void UpdateBulletUI(int currentBullets, int remainedBullets) {
        currentBulletUI.text = currentBullets.ToString();
        remainedBulletUI.text = "/ " + remainedBullets.ToString();
    }
}
