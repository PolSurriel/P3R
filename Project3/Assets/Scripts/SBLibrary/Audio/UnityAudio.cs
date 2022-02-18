using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityAudio : SurrealBoost.Audio
{
    private AudioSource audio;

    public UnityAudio(string path)
    {
        //audio.clip = Resources.Load<AudioClip>(path);
    }
    public override void Clean()
    {
        throw new System.NotImplementedException();
    }

    public override void Play()
    {
        audio.Play();
    }

    public override void Set(string parameterName, float value)
    {
        switch (parameterName)
        {
            case "volume":
                audio.volume = value;
                break;
            case "pitch":
                audio.pitch = value;
                break;
            case "pan":
                audio.panStereo = value;
                break;
            case "spatialBlend":
                audio.spatialBlend = value;
                break;
            case "reverb":
                audio.reverbZoneMix = value;
                break;
            case "loop":
                audio.loop = (value != 0);
                break;
            default:
                Debug.LogWarning("Audio Set parameter not recognized");
                break;

        }
    }

    public override void Stop()
    {
        audio.Stop();
    }

    public override void StopFadeout()
    {
        float duration = 2.0f;
        AudioController.instance.StartCoroutine(StartFade(audio, duration));
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
}


// Ahora mi volumen es de 0.5, quero pasar a 0.7 de manera smooth