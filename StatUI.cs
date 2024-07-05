using TMPro;
using UnityEngine;

public class StatUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI healthValue;

    public void UpdateHealthValue(int health) {
        healthValue.text = health.ToString();
    }
}
