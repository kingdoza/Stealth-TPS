using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour {
    [SerializeField] public GunInfoUI gunInfoUI;
    [SerializeField] public GraphDrawer sleepCycleUI;
    [SerializeField] public StatUI statUI;
    [SerializeField] private HitMarkController hitMarkPool;
    [SerializeField] private TotemSlots totemSlots;
    public TotemSlots TotemSlots => totemSlots;

    public void SetTotemSlots(Totem[] totems) {
        totemSlots.SetSlots(totems);
    }

    public void ApplyTimeToSleepCycle() {
        sleepCycleUI.ApplyTime();
    }

    public void UpdateHealthStat(int health) {
        statUI.UpdateHealthValue(health);
    }

    public void ShowHitMark(Vector3 hitOrigin) {
        hitMarkPool.GenerateHitMark(hitOrigin);
    }
}
