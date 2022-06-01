using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePlatform : MonoBehaviour
{


    bool isGreen = false;


    const float GREEN_TIME_TO_ADD = 7f;
    const float RED_TIME_TO_SUBSTRACT = 3f;

    ParticleSystem particles;
    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        particles = GetComponent<ParticleSystem>();
    }

    public void SetRed()
    {
        isGreen = false;
        particles.startColor = Color.red;
        spriteRenderer.color = Color.red;

    }

    public void SetGreen()
    {
        isGreen = true;
        particles.startColor = Color.green;
        spriteRenderer.color = Color.green;

    }

	private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (isGreen)
            {
                GameInfo.instance.platformsCountDown += GREEN_TIME_TO_ADD;
                //PLAY SOUND
            }
            else
            {
                GameInfo.instance.platformsCountDown -= RED_TIME_TO_SUBSTRACT;
                // PLAY SOUND
            }

            Destroy(gameObject);
        }
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
        if (collision.transform.tag == "Player")
        {
            if (isGreen)
            {
                GameInfo.instance.platformsCountDown += GREEN_TIME_TO_ADD;
                //PLAY SOUND
            }
            else
            {
                GameInfo.instance.platformsCountDown -= RED_TIME_TO_SUBSTRACT;
                // PLAY SOUND
            }

            Destroy(gameObject);
        }
    }

}
