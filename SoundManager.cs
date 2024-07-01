using UnityEngine;

public class SoundManager : ObjectPool {
    public void PlayAudioSource(AudioSource audioSource) {
        AudioObject audioObject = PullOut().GetComponent<AudioObject>();
        audioObject.CopyAudioSourceToPlay(audioSource);
        audioObject.Play();
    }

    public void PlayAudioSource3DWithSoundWave(AudioSource audioSource, Vector3 audioPos) {
        AudioObject audioObject = PullOut(audioPos).GetComponent<AudioObject>();
        audioObject.CopyAudioSourceToPlay(audioSource);
        audioObject.Play3DWithSoundWave();
    }

    public void PlayAudioSource3D(AudioSource audioSource, Vector3 audioPos) {
        AudioObject audioObject = PullOut(audioPos).GetComponent<AudioObject>();
        audioObject.CopyAudioSourceToPlay(audioSource);
        audioObject.Play3D();
    }
}
