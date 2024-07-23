using System.Collections;
using UnityEngine;

public class TargetEnemy : Enemy {
    private new ParticleSystem particleSystem;
    [SerializeField] private float maxParticleEmission = 40;
    [SerializeField] private float fadeDelta = 0.05f;
    protected override void Awake() {
        base.Awake();
        particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    protected override void SetFeelingColor() {
        base.SetFeelingColor();
        Color feelingColor = transform.GetChild(1).GetComponent<Renderer>().material.color;
        if(particleSystem == null)
            particleSystem = GetComponentInChildren<ParticleSystem>();
        var main = particleSystem.main;
        main.startColor = feelingColor;
    }

    public override void Die() {
        base.Die();
        StopAllCoroutines();
        StartCoroutine(FadeOutParticle());
    }

    public void ActiveTargetEffect() {
        if(IsAlive == false)
            return;
        StopAllCoroutines();
        StartCoroutine(FadeInParticle());
    }

    public void DeactiveTargetEffect() {
        StopAllCoroutines();
        StartCoroutine(FadeOutParticle());
    }

    private IEnumerator FadeInParticle() {
        var emissionModule = particleSystem.emission;
        float rateOverTime = emissionModule.rateOverTime.constant;
        while(rateOverTime < maxParticleEmission) {
            rateOverTime += fadeDelta;
            emissionModule.rateOverTime = rateOverTime;
            yield return Time.fixedDeltaTime;
        }
        emissionModule.rateOverTime = maxParticleEmission;
    }

    private IEnumerator FadeOutParticle() {
        var emissionModule = particleSystem.emission;
        float rateOverTime = emissionModule.rateOverTime.constant;
        while(rateOverTime > 0) {
            Debug.Log("fadeout : " + emissionModule.rateOverTime.constant);
            rateOverTime -= fadeDelta;
            emissionModule.rateOverTime = rateOverTime;
            yield return Time.fixedDeltaTime;
        }
        emissionModule.rateOverTime = 0;
    }
}
