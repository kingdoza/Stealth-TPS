using System.Collections;
using UnityEngine;

public class AudioObject : PooledObject {
    [SerializeField] private float soundExtendSpeed;
    //private SoundWave soundWave;
    private Transform soundWave;
    private AudioSource audioSource;


    protected override void Awake() {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        //soundWave = transform.GetComponentInChildren<SoundWave>();
        soundWave = transform.GetChild(0);
        soundWave.gameObject.SetActive(false);
    }

    public void CopyAudioSourceToPlay(AudioSource sourceToPlay) {
        audioSource.clip = sourceToPlay.clip;
        audioSource.volume = sourceToPlay.volume;
        audioSource.pitch = sourceToPlay.pitch;
        audioSource.loop = sourceToPlay.loop;
        audioSource.mute = sourceToPlay.mute;
        audioSource.playOnAwake = sourceToPlay.playOnAwake;
        audioSource.spatialBlend = sourceToPlay.spatialBlend;
        audioSource.reverbZoneMix = sourceToPlay.reverbZoneMix;
        audioSource.bypassEffects = sourceToPlay.bypassEffects;
        audioSource.bypassListenerEffects = sourceToPlay.bypassListenerEffects;
        audioSource.bypassReverbZones = sourceToPlay.bypassReverbZones;
        audioSource.dopplerLevel = sourceToPlay.dopplerLevel;
        audioSource.spread = sourceToPlay.spread;
        audioSource.priority = sourceToPlay.priority;
        audioSource.minDistance = sourceToPlay.minDistance;
        audioSource.maxDistance = sourceToPlay.maxDistance;
        audioSource.panStereo = sourceToPlay.panStereo;
        audioSource.rolloffMode = sourceToPlay.rolloffMode;
    }

    public void Play() {
        audioSource.Play();
        StartCoroutine(DelayDestroy());
    }

    public void Play3DWithSoundWave() {
        audioSource.spatialBlend = 1;
        audioSource.Play();
        StartCoroutine(DelayDestroy());
        soundWave.position = transform.position + Vector3.up * 0.01f;
        soundWave.localScale = Vector3.zero;
        soundWave.gameObject.SetActive(true);
        StartCoroutine(KeepExtendingSoundWave());
    }

    public void Play3D() {
        audioSource.spatialBlend = 1;
        audioSource.Play();
        StartCoroutine(DelayDestroy());
    }

    private IEnumerator KeepExtendingSoundWave() {
        while(gameObject.activeSelf) {
            soundWave.localScale += Vector3.one * audioSource.volume * soundExtendSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator DelayDestroy() {
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy();
    }

    protected override void Destroy() {
        soundWave.gameObject.SetActive(false);
        base.Destroy();
    }
}
