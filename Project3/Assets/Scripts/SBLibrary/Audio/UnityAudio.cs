using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityAudio : SurrealBoost.Audio
{
    public AudioClip audioclip;

    public UnityAudio(string path)
    {
        audioclip = Resources.Load<AudioClip>(path);
    }
    public UnityAudio(AudioClip clip)
    {
        audioclip = clip;
    }
    public override void Clean()
    {
        throw new System.NotImplementedException();
    }

    public override void Play()
    {
        
        AudioController.myAudioSource.PlayOneShot(audioclip);
    }

    public override void Set(string parameterName, float value)
    {
        switch (parameterName)
        {
            case "volume":
                AudioController.myAudioSource.volume = value;
                break;
            case "pitch":
                AudioController.myAudioSource.pitch = value;
                break;
            case "pan":
                AudioController.myAudioSource.panStereo = value;
                break;
            case "spatialBlend":
                AudioController.myAudioSource.spatialBlend = value;
                break;
            case "reverb":
                AudioController.myAudioSource.reverbZoneMix = value;
                break;
            case "loop":
                AudioController.myAudioSource.loop = (value != 0);
                break;
            default:
                Debug.LogWarning("Audio Set parameter not recognized");
                break;

        }
    }

    public override void Stop()
    {
        AudioController.myAudioSource.Stop();
    }

    public override void StopFadeout()
    {
        float duration = 2.0f;
        AudioController.instance.StartCoroutine(StartFade(AudioController.myAudioSource, duration));
    }

    public static IEnumerator StartFade(AudioSource audioSource, float duration)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, 0.0f, currentTime / duration);
            yield return null;
        }
        audioSource.Stop();
        yield break;
    }

    public override void SetVolume(float v)
    {
        throw new System.NotImplementedException();
    }
}


// Ahora mi volumen es de 0.5, quero pasar a 0.7 de manera smooth