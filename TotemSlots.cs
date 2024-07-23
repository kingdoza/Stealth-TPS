using TMPro;
using UnityEngine;

public class TotemSlots : MonoBehaviour {
    public TotemSlot[] totemSlots { get; private set; }
    [SerializeField] private TextMeshProUGUI totemNameUI;
    [SerializeField] private TextMeshProUGUI totemInfoUI;

    public void SetSlots(Totem[] totems, bool isClearInfos = false) {
        if(isClearInfos)
            ClearTotemInfos();
        totemSlots = GetComponentsInChildren<TotemSlot>();
        for(int i = 0; i < totemSlots.Length; ++i) {
            if(i < totems.Length) {
                //totemSlots[i].Show();
                totemSlots[i].onMouseEvent.AddListener(OnSlotMouseEnter);
                totemSlots[i].onMouseExit.AddListener(OnSlotMouseExit);
                totemSlots[i].SetTotem(totems[i]);
            }
            else {
                //totemSlots[i].Hide();
                totemSlots[i].gameObject.SetActive(false);
            }
        }
    }

    private void ClearTotemInfos() {
        totemNameUI.text = string.Empty;
        totemInfoUI.text = string.Empty;
    }

    private void FillTotemInfoes(Totem totem) {
        totemNameUI.text = totem.Name;
        totemInfoUI.text = totem.Info;
    }

    private void OnSlotMouseEnter(TotemSlot slot) {
        FillTotemInfoes(slot.Totem);
    }

    private void OnSlotMouseExit() {
        ClearTotemInfos();
    }
}
