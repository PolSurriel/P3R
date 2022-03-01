using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioController : MonoBehaviour
{
    public static AudioController instance;

    public class Sounds
    {
        public SurrealBoost.Audio player_jump = new FMODAudio("event:/FOLEY/character actions/jump");
        public SurrealBoost.Audio player_doubleJump = new FMODAudio("event:/FOLEY/character actions/doublejump");
        public SurrealBoost.Audio runner_jump = new FMODAudio("event:/FOLEY/character actions/jump");
        public SurrealBoost.Audio runner_doubleJump = new FMODAudio("event:/FOLEY/character actions/doublejump");
        public SurrealBoost.Audio gameplaySoundtrack = new FMODAudio("event:/MUSIC/ingame music");
        //public SurrealBoost.Audio intro = new UnityAudio("Audios/buttonSound.wav");
    }

    public Sounds sounds;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance != null)
            Destroy(this);
        else
            instance = this;
        sounds = new Sounds();
        sounds.gameplaySoundtrack.Set("volume", 100f);

        sounds.player_doubleJump.Set("character_voice", 2f);
        sounds.player_doubleJump.Set("character_voice", 2f);
        sounds.runner_doubleJump.Set("character_voice", 2f);
        sounds.runner_doubleJump.Set("character_voice", 2f);

    }

    private void Start()
    {
        
    }

   
}
