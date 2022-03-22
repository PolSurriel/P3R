using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FMODAudio : SurrealBoost.Audio
{

    FMOD.Studio.EventInstance instance;

    
    public FMODAudio(string path)
    {
        instance = FMODUnity.RuntimeManager.CreateInstance(path);
    }



    public override void Play()
    {
        instance.start();
    }

    public override void Set(string parameterName, float value)
    {
        instance.setParameterByName(parameterName, value);
    }

    public override void Stop()
    {
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public override void StopFadeout()
    {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public override void Clean()
    {
        instance.release();
    }

    public override void SetVolume(float v)
    {
        instance.setVolume(v);
    }
}

