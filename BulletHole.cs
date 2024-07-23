using System.Collections;
using UnityEngine;

public class BulletHole : PooledObject, IEnemyReactable {
    private EnemyReaction enemyReaction;
    public EnemyReaction EnemyReaction => enemyReaction;
    public Vector3 Position => transform.position;
    [SerializeField] private Sprite[] holeSprite;
    [SerializeField] private AudioClip[] personhitClips;
    [SerializeField] private AudioClip[] nonPersonhitClips;
    private SpriteRenderer spriteRenderer;
    private float rotationAngleWhenHit = 0;
    private Vector3 hitDirection;
    public Vector3 HitOrigin { 
        set { 
            hitDirection = value - transform.position;
            rotationAngleWhenHit = transform.rotation.eulerAngles.y;
        } 
        get {
            float rotationDelta = rotationAngleWhenHit = transform.root.rotation.eulerAngles.y;
            Vector3 rotatedHitDir = Quaternion.AngleAxis(rotationDelta, new Vector3(0, 1, 0)) * hitDirection;
            enemyReaction = EnemyReaction.None;
            return transform.position + rotatedHitDir; 
        } 
    }

    private AudioSource audioSource;
    public Transform Transform => transform;

    protected override void Awake() {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable() {
        enemyReaction = EnemyReaction.Search;
        SetRandomHoleSprite();
        StartCoroutine(LowerEnemyReactionAfterDelay());
    }

    private IEnumerator LowerEnemyReactionAfterDelay() {
        yield return new WaitForSeconds(0.05f);
        if(enemyReaction != EnemyReaction.None)
            enemyReaction = EnemyReaction.Doubt;
    }

    private void SetRandomHoleSprite() {
        int randomIndex = Random.Range(0, holeSprite.Length);
        spriteRenderer.sprite = holeSprite[randomIndex];
    }

    public void ShowPersonHitEffects() {
        spriteRenderer.color = Color.red;
        transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
        int randomIndex = Random.Range(0, personhitClips.Length);
        audioSource.clip = personhitClips[randomIndex];
        audioSource.volume = 0.1f;
        GameManager.Instance.soundManager.PlayAudioSource3DWithSoundWave(audioSource, transform.position);
    }

    public void ShowNonPersonHitEffects() {
        spriteRenderer.color = Color.white;
        transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
        int randomIndex = Random.Range(0, nonPersonhitClips.Length);
        audioSource.clip = nonPersonhitClips[randomIndex];
        audioSource.volume = 0.18f;
        GameManager.Instance.soundManager.PlayAudioSource3DWithSoundWave(audioSource, transform.position);
    }
}
