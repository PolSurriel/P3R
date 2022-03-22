using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SurrealBoost
{
    public abstract class Audio
    {
        public abstract void SetVolume(float v);
        public abstract void Play();
        public abstract void Set(string parameterName, float value);
        public abstract void Stop();
        public abstract void StopFadeout();
        public abstract void Clean();
    }

}

