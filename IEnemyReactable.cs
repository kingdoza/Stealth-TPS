using UnityEngine;

public enum EnemyReaction {
    Alert = 1, Search, Doubt, None
}

public interface IEnemyReactable {
    public EnemyReaction EnemyReaction { get; }
    public Vector3 Position { get; }
    public Transform Transform { get; }
    public Vector3 HitOrigin { get; }
}
