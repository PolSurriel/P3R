using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerksInGame : MonoBehaviour
{
    [SerializeField]
    bool isPulse = false;
    float pulseProb = 1.0f;
    const float PulseForceMagnitude = 5f;
    // Start is called before the first frame update
    void Start()
    {
        foreach(ScriptablePerk perk in GameInfo.equippedPerks)
        {
            switch (perk.myName)
            {
                case "Default":
                    break;
                case "Pulse":
                    isPulse = true;
                    break;
                default:
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckJumpPerks(Runner player)
    {
        if(player.jumpCounter >= 1)         // Comprobar todas las perks que puedan triggear en el segundo salto
        {
            foreach(Runner enemy in GameObject.FindObjectsOfType<Runner>())
            {
                if (player.gameObject != enemy.gameObject && Random.Range(0.0f, 1.0f) < pulseProb)
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
