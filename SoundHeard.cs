using UnityEngine;

public class SoundHeard : MonoBehaviour, IEnemyReactable {
    public EnemyReaction EnemyReaction => EnemyReaction.Doubt;
    public Vector3 Position => transform.position;
    public Transform Transform => transform;
    public Vector3 HitOrigin => Position;

    private void Update() {
        transform.position = Position;
    }
}
