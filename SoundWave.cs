using UnityEngine;

public class SoundWave : MonoBehaviour, IEnemyReactable {
    public EnemyReaction EnemyReaction => EnemyReaction.Doubt;
    public Vector3 Position => transform.position;
    public Vector3 HitOrigin => Position;
    public Transform Transform => transform;


    public float GetSpreadDistance() {
        float waveColliderRadius = GetComponent<SphereCollider>().radius;
        return waveColliderRadius * transform.localScale.x;
    }
}
