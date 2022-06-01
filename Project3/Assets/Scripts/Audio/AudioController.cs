using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioController : MonoBehaviour
{
    public static AudioController instance;
    public static AudioSource myAudioSource;
    public AudioMixer mixer;
    public AudioMixerGroup sfx_Mixer;
    public AudioMixerGroup music_Mixer;


    public class Sounds
    {
        public SurrealBoost.Audio crazyImpact = new FMODAudio("event:/FOLEY/interactive objects/crazyimpact");
        public SurrealBoost.Audio windmill = new FMODAudio("event:/FOLEY/interactive objects/windmill");
        public SurrealBoost.Audio treadmill = new FMODAudio("event:/FOLEY/interactive objects/treadmill");
        public SurrealBoost.Audio impact = new FMODAudio("event:/FOLEY/interactive objects/impact sound 2");
        
        public SurrealBoost.Audio wallCollision = new FMODAudio("event:/FOLEY/character actions/wallcollision");
        public SurrealBoost.Audio wallCollisionAI = new FMODAudio("event:/FOLEY/character actions/wallcollision");

        public SurrealBoost.Audio door = new FMODAudio("event:/FOLEY/interactive objects/doors");
        public SurrealBoost.Audio doorClose = new FMODAudio("event:/FOLEY/interactive objects/doors");
        public SurrealBoost.Audio reboundSurface = new FMODAudio("event:/FOLEY/interactive objects/mat");
        public SurrealBoost.Audio reboundSurfaceAI = new FMODAudio("event:/FOLEY/interactive objects/mat");
        public SurrealBoost.Audio stain = new FMODAudio("event:/FOLEY/interactive objects/stain");
        public SurrealBoost.Audio stainAI = new FMODAudio("event:/FOLEY/interactive objects/stain");
        

        public SurrealBoost.Audio portal = new FMODAudio("event:/FOLEY/interactive objects/portal");
        public SurrealBoost.Audio portalAI = new FMODAudio("event:/FOLEY/interactive objects/portal");
        
        public SurrealBoost.Audio pickupPowerup = new FMODAudio("event:/FOLEY/interactive objects/power up");
        public SurrealBoost.Audio pickupPowerupAI = new FMODAudio("event:/FOLEY/interactive objects/power up norot");

        public SurrealBoost.Audio player_jump = new FMODAudio("event:/FOLEY/character actions/jump");
        public SurrealBoost.Audio player_doubleJump = new FMODAudio("event:/FOLEY/character actions/doublejump");
        public SurrealBoost.Audio runner_jump = new FMODAudio("event:/FOLEY/character actions/jump");
        public SurrealBoost.Audio runner_doubleJump = new FMODAudio("event:/FOLEY/character actions/doublejump");
        public SurrealBoost.Audio gameplaySoundtrack = new FMODAudio("event:/MUSIC/ingame music");

        // UI Sounds
        public SurrealBoost.Audio buttonSound = new UnityAudio("Audios/buttonSound", instance.sfx_Mixer);
        public SurrealBoost.Audio perkButtonSound = new UnityAudio("Audios/perkButtonSound", instance.sfx_Mixer);
        public SurrealBoost.Audio changeTabSound = new UnityAudio("Audios/changeTabSound", instance.sfx_Mixer);
        public SurrealBoost.Audio errorButtonSound = new UnityAudio("Audios/ErrorSound", instance.sfx_Mixer);
        
        public SurrealBoost.Audio matchSong = new FMODAudio("event:/MUSIC/ingame music");
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
        sounds.runner_doubleJump.Set("character_voice", 0f);
        sounds.runner_doubleJump.Set("character_voice", 0f);

        sounds.matchSong.Set("layering", 4f);
        SetSongLayer0Volume(0.5f);

        sounds.pickupPowerupAI.SetVolume(0.7f);
        sounds.portalAI.SetVolume(0.7f);
        sounds.reboundSurfaceAI.SetVolume(0.7f);
        sounds.stainAI.SetVolume(0.7f);

        sounds.wallCollision.SetVolume(0.4f);
        sounds.wallCollisionAI.SetVolume(0.3f);

        sounds.doorClose.Set("open close",2f);
    }

    public void SetSongLayer0Volume(float v)
    {
        sounds.matchSong.Set("volume layer 1", v * 100f);
    }

    public void SetSongLayer1Volume(float v)
    {
        sounds.matchSong.Set("volume layer 2", v * 100f);
    }
    public void SetSongLayer2Volume(float v)
    {
        sounds.matchSong.Set("volume layer 3", v * 100f);
    }
    public void SetSongLayer3Volume(float v)
    {
        sounds.matchSong.Set("volume layer 4", v * 100f);
    }

    private void Start()
    {
        myAudioSource = this.GetComponent<AudioSource>();
        if (myAudioSource == null)
            myAudioSource = this.gameObject.AddComponent<AudioSource>();
    }

   
}
