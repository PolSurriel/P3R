using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartMatchCountDown : MonoBehaviour
{


    TextMeshPro text;
    
    float timeCounter = 0f;
    float duration = 5;


    public AudioClip second5;
    public AudioClip second4;
    public AudioClip second3;
    public AudioClip second2;
    public AudioClip second1;
    public AudioClip gosound;

    UnityAudio audiosecond5;
    UnityAudio audiosecond4;
    UnityAudio audiosecond3;
    UnityAudio audiosecond2;
    UnityAudio audiosecond1;
    UnityAudio audiogosound;


    bool audiosecond5played;
    bool audiosecond4played;
    bool audiosecond3played;
    bool audiosecond2played;
    bool audiosecond1played;
    bool audiogosoundplayed;


    public ParticleSystem fir1;
    public ParticleSystem fir2;
    public ParticleSystem fir3;
    public ParticleSystem fir4;
    public ParticleSystem fir5;


    bool fir1played;
    bool fir2played;
    bool fir3played;
    bool fir4played;
    bool fir5played;



    private void OnDestroy()
    {

        if (GameInfo.instance != null)
            GameInfo.instance.OnMatchSceneClosed();
    }

    private void Start()
    {


        audiosecond5 = new UnityAudio(second5, AudioController.instance.sfx_Mixer);
        audiosecond4 = new UnityAudio(second4, AudioController.instance.sfx_Mixer);
        audiosecond3 = new UnityAudio(second3, AudioController.instance.sfx_Mixer);
        audiosecond2 = new UnityAudio(second2, AudioController.instance.sfx_Mixer);
        audiosecond1 = new UnityAudio(second1, AudioController.instance.sfx_Mixer);
        audiogosound = new UnityAudio(gosound, AudioController.instance.sfx_Mixer);

        text = GetComponent<TextMeshPro>();
        AudioController.instance.sounds.matchSong.Play();

    }

    public static bool matchStarted = false;



    // Update is called once per frame
    void Update()
    {
        timeCounter += Time.deltaTime;

        if (timeCounter > 4.5f && !fir1played)
        {
            fir1played = true;
            fir1.Play();
        }

        if (timeCounter > 4.6f && !fir2played)
        {
            fir2played = true;
            fir2.Play();
        }

        if (timeCounter > 4.7f && !fir3played)
        {
            fir3played = true;
            fir3.Play();
        }

        if (timeCounter > 5f && !fir4played)
        {
            fir4played = true;
            fir4.Play();
        }

        if (timeCounter > 5.1f && !fir5played)
        {
            fir5played = true;
            fir5.Play();
        }

        if (timeCounter <= (duration - 0.5f))
        {

            if (!audiosecond5played && timeCounter > 0.5f)
            {
                audiosecond5played = true;
                audiosecond5.Play();

            }
            else if (!audiosecond4played && timeCounter > 1.5f)
            {
                audiosecond4played = true;
                audiosecond4.Play();

            }
            else if (!audiosecond3played && timeCounter > 2.5f)
            {
                audiosecond3played = true;
                audiosecond3.Play();

            }
            else if (!audiosecond2played && timeCounter > 3.5f)
            {
                audiosecond2played = true;
                audiosecond2.Play();

            }
            else if (!audiosecond1played && timeCounter > 4.5f)
            {
                audiosecond1played = true;
                audiosecond1.Play();

            }

            text.text = (duration - timeCounter).ToString("0");
        }else
        {
            if (!audiogosoundplayed)
            {
                audiogosoundplayed = true;
                audiogosound.Play();
            }

            text.fontSize = 25f;
            text.text = "GO!";
            matchStarted = true;
            

            
            if (GameInfo.instance == null || GameInfo.instance.ai_players == null)
                return;

            foreach (var ai in GameInfo.instance.ai_players)
            {
                ai.GetComponent<AIController>().OnMatchStarts();
            }

        }
    }

    public void ResetCountDown()
    {
        timeCounter = 0;
        duration = 5;
    }
}
