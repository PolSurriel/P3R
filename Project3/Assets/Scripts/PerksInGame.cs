using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerksInGame : MonoBehaviour
{
    [SerializeField]
    bool isPulse = false;
    float pulseProb = 1.0f;
    const float PulseForceMagnitude = 5f;

    [SerializeField]
    bool isTransparent = false;
    float transparentProb = 1.0f;
    float transparentTime = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        if(this.gameObject.GetComponent<PlayerController>() != null)        // Just use perks in Player, not IA
        {
            foreach (ScriptablePerk perk in GameInfo.equippedPerks)
            {
                switch (perk.myName)
                {
                    case "Default":
                        break;
                    case "Pulse":
                        isPulse = true;
                        break;
                    case "Transparent":
                        // Ghost parameters are 65, 157
                        isTransparent = true;
                        break;
                    default:
                        break;
                }
            }
        }
        foreach (Transform child in this.transform.GetChild(1))
        {
            child.GetComponent<SpriteRenderer>().material.SetFloat("_GhostTransparency", 0.0f);
            child.GetComponent<SpriteRenderer>().material.SetFloat("_GhostBlend", 0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckJumpPerks(Runner player)
    {
        if(player.jumpCounter > 1)         // Comprobar todas las perks que puedan triggear en el segundo salto
        {

            if(Random.Range(0.0f, 1.0f) < transparentProb && isTransparent)
            {
                StartCoroutine(TransparentEffect());
            }
            if (isPulse)
            {
                bool flagPulse = Random.Range(0.0f, 1.0f) < pulseProb;
                foreach (Runner enemy in GameObject.FindObjectsOfType<Runner>())
                {
                    if (player.gameObject != enemy.gameObject && flagPulse)
                    {
                        enemy.rb.AddForce(new Vector2(      // Calculates Vector to Add Force
                            enemy.transform.position.x - player.transform.position.x,
                            enemy.transform.position.y - player.transform.position.y
                            ).normalized * PulseForceMagnitude);
                    }
                }
            }
        }
    }

    IEnumerator TransparentEffect()
    {
        // Sets effect
        foreach(Transform child in this.transform.GetChild(1))
        {
            child.GetComponent<SpriteRenderer>().material.SetFloat("_GhostTransparency", 0.5f);
            child.GetComponent<SpriteRenderer>().material.SetFloat("_GhostBlend", 0.5f);
        }

        // FALTA DESACTIVAR LA COLISION CON LOS ENEMIGOS

        float tc = 0f;
        do { yield return null; }
        while ((tc += Time.deltaTime) < transparentTime);

        // Resets effect
        foreach (Transform child in this.transform.GetChild(1))
        {
            child.GetComponent<SpriteRenderer>().material.SetFloat("_GhostTransparency", 0.0f);
            child.GetComponent<SpriteRenderer>().material.SetFloat("_GhostBlend", 0.0f);
        }

    }
}
