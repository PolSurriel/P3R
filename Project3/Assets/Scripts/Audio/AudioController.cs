using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioController : MonoBehaviour
{
    public static AudioController instance;

    public class Sounds
    {
        public SurrealBoost.Audio jump = new FMODAudio("event:/FOLEY/jump");
        public SurrealBoost.Audio doubleJump = new FMODAudio("event:/FOLEY/doublejump");
        public SurrealBoost.Audio gameplaySoundtrack = new FMODAudio("event:/MUSIC/ingame music");
    }

    public Sounds sounds;

    private void Awake()
    {
        instance = this;
        sounds = new Sounds();
        sounds.gameplaySoundtrack.Set("volume", 100f);

    }

    private void Start()
    {
        
    }

   
}
