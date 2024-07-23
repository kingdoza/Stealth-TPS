using UnityEngine;

public class TotemController : MonoBehaviour, IStageRun {
    private TotemSlot[] totemSlots;
    private TotemSlot activedTotem = null;
    [SerializeField] private Transform navigationArrow;
    public bool IsRunning { get; set; } = false;

    private void Start() {
        navigationArrow.gameObject.SetActive(false);
        totemSlots = GameManager.Instance.uiManager.TotemSlots.totemSlots;
    }

    public void Run() {
        IsRunning = true;
    }

    // Update is called once per frame
    private void Update() {
        if(IsRunning == false)
            return;
        for (int i = 0; i < totemSlots.Length; i++) {
            if(totemSlots[i].gameObject.activeSelf == false)
                continue;
            if(totemSlots[i].Totem.TargetEnemy.IsAlive == false) {
                if(activedTotem == totemSlots[i]) {
                    activedTotem.SetOff();
                    activedTotem = null;
                }
                continue;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                TotemSlot selectedTotem = totemSlots[i];
                ActiveTotem(selectedTotem);
                break;
            }
        }
        if(activedTotem != null) {
            Enemy targetEnemy = activedTotem.Totem.TargetEnemy;
            Vector3 targetDir = targetEnemy.Position - transform.position;
            targetDir.y = 0.05f;
            Quaternion rotationToTarget = Quaternion.LookRotation(targetDir);
            rotationToTarget.eulerAngles += new Vector3(-90, -90, 0);
            navigationArrow.rotation = rotationToTarget;
            Vector3 offsetFromPlayer = targetDir.normalized * 1.5f;
            navigationArrow.position = transform.position + offsetFromPlayer;
            navigationArrow.gameObject.SetActive(true);
        }
        else {
            navigationArrow.gameObject.SetActive(false);
        }
    }

    private void ActiveTotem(TotemSlot selectedTotem) {
        for(int i = 0; i < totemSlots.Length; ++i) {
            if(totemSlots[i].gameObject.activeSelf == false)
                continue;
            totemSlots[i].Deactive();
        }
        if(selectedTotem == activedTotem) {
            activedTotem = null;
            return;
        }
        activedTotem = selectedTotem;
        activedTotem.Active();
    }
}
