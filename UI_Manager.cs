using UnityEngine;

public class UI_Manager : MonoBehaviour {
    [SerializeField] public GunInfoUI gunInfoUI;
    [SerializeField] public GraphDrawer sleepCycleUI;
    [SerializeField] public StatUI statUI;

    public void ApplyTimeToSleepCycle() {
        sleepCycleUI.ApplyTime();
    }

    public void UpdateHealthStat(int health) {
        statUI.UpdateHealthValue(health);
    }
}
