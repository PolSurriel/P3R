using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraJumpZone : MonoBehaviour
{
    public ParticleSystem pickEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Runner>().isPlayer)
        {
            AudioController.instance.sounds.pickupPowerup.Play();
            pickEffect.transform.position = collision.transform.position;
            pickEffect.Play();
        }else
        {
            AudioController.instance.sounds.pickupPowerup.Play();
        }
    }

}
