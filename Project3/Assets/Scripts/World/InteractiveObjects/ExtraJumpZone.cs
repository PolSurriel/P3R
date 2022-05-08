using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraJumpZone : MonoBehaviour
{
    public GameObject powerUpParticle;
    public Material material;

    public ParticleSystem pickEffect;
    ParticleSystem bgParticleEffect;
    SpriteRenderer sr;

    const float TIME_TO_RESTORE = 3f;

    [HideInInspector]
    public List<Runner> ignoring = new List<Runner>();

    private void Start()
    {
        
        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        bgParticleEffect = transform.GetChild(1).GetComponent<ParticleSystem>();

    }


    IEnumerator BrilloLata()
    {
        const float duration = 0.3f;
        float t = 0f;

        material.SetFloat("_ShineLocation", 0f);
        do
        {
            yield return null;
            float progress = t / duration;
            material.SetFloat("_ShineLocation", progress);

        } while ((t += Time.deltaTime) < duration);

        material.SetFloat("_ShineLocation", 0f);

    }

    void RestoreMainPlayerFeedback()
    {
        pickEffect.transform.position = transform.position;
        pickEffect.transform.localScale = Vector3.one;

        sr.color = Color.white;
        bgParticleEffect.Play();
        pickEffect.Play();
        StartCoroutine(BrilloLata());
        

    }

    IEnumerator StartRestoreRunnerCooldown(Runner runner)
    {
        float t = 0f;

        yield return null;
        ignoring.Add(runner);
        t += Time.deltaTime;

        do { yield return null; }
        while ((t += Time.deltaTime) < TIME_TO_RESTORE);
        ignoring.Remove(runner);

        if (runner.isPlayer)
        {
            RestoreMainPlayerFeedback();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        var runner = collision.GetComponent<Runner>();
        if (runner == null || ignoring.Contains(runner))
            return;

        
        StartCoroutine(StartRestoreRunnerCooldown(runner));

        if (collision.GetComponent<Runner>().isPlayer)
        {
            AudioController.instance.sounds.pickupPowerup.Play();
            pickEffect.transform.position = collision.transform.position;

            pickEffect.transform.localScale = Vector3.one * 0.6f;

            sr.color = new Color(1f, 1f, 1f, 0.3f);
            bgParticleEffect.Stop();
            pickEffect.Play();

            var particle = Instantiate(powerUpParticle);

            particle.transform.position = collision.transform.position;

            var vel = collision.GetComponent<Rigidbody2D>().velocity;
            vel.x *= -1f;

            const float minParticleXVel = 0.5f;
            if (Mathf.Abs(vel.x) < minParticleXVel) vel.x = vel.x < 0f ? -minParticleXVel : minParticleXVel;
            particle.GetComponent<Rigidbody2D>().velocity = vel;
            particle.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(100f, 400f) * (Random.Range(0f, 100f) < 50f ? 1f:-1f);

        }
        else
        {
            AudioController.instance.sounds.pickupPowerup.Play();
        }
    }

}
