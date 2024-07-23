using System;
using UnityEngine;
using UnityEngine.UI;

public class HitMark : PooledObject {
    //private static PlayerController player;
    [SerializeField] private float holdingTime = 2;
    [SerializeField] private float fadeoutTime = 1;
    public Vector3 HitOrigin { private get; set;} = Vector3.zero;
    public float RemainedTime { get; private set; }
    private Image image;
    private Color markColor;
    
    protected override void Awake() {
        base.Awake();
        image = GetComponent<Image>();
        markColor = Color.red;
    }

    private void Start() {
        //player = GameManager.Instance.stageManager.Player;
    }

    private void OnEnable() {
        RemainedTime = holdingTime;
        markColor.a = 1;
        image.color = markColor;
    }

    private void FixedUpdate() {
        RemainedTime -= Time.fixedDeltaTime;
        LookOriginDirection();
        if(RemainedTime <= fadeoutTime) {
            markColor.a = RemainedTime / fadeoutTime;
            image.color = markColor;
        }
        else if(RemainedTime <= 0) {
            Destroy();
        }
    }

    private void LookOriginDirection() {
        Vector3 directionToOrigin = (HitOrigin - Player.Instance.Position).normalized;
        float angle = Vector3.SignedAngle(Player.Instance.Transform.forward, directionToOrigin, -Vector3.up);
        Vector3 targetRot = new Vector3(0, 0, angle + 16.5f);
        transform.rotation = Quaternion.Euler(targetRot);
    }

    public void LookOriginDirection(Vector3 originPos) {
        Vector3 directionToOrigin = (originPos - Player.Instance.Position).normalized;
        float angle = Vector3.SignedAngle(Player.Instance.Transform.forward, directionToOrigin, -Vector3.up);
        Vector3 targetRot = new Vector3(0, 0, angle + 16.5f);
        transform.rotation = Quaternion.Euler(targetRot);
    }
}
