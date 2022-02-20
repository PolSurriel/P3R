using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treadmill : MonoBehaviour
{
    [HideInInspector]
    public List<Runner> playersIn = new List<Runner>();
    public Vector2 direction = Vector2.up;
    public Vector2 normal = Vector2.right;
    public float speed = 20f;
    public List<Transform> exits = new List<Transform>();


    BoxCollider2D boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        direction = direction.normalized;
    }

  
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var runner = collision.collider.GetComponent<Runner>();

        if (runner == null || runner.ignoreTreadmill)
            return;

        if (playersIn.Contains(runner))
            return;

        runner.jumpCounter = 0;
        runner.EnterOnATreadmill();
        runner.rb.velocity = direction * speed;
        runner.treadmill = this;

        var ai = runner.GetComponent<AIController>();

        if(ai != null)
        {
            ai.EnterOnATreadMille(ref exits, direction);
        }


        var dir = (Vector2)runner.transform.position - boxCollider.ClosestPoint(runner.transform.position);
        dir.Normalize();
        runner.contactToSurfaceDirection = dir;

        playersIn.Add(runner);
        StartCoroutine(CheckPlayerY(runner));
    }


    IEnumerator CheckPlayerY(Runner player)
    {
        while (player.onATreadmill)
        {
            

            const float margen = 0.01f;
            if (player.transform.position.y-margen > boxCollider.bounds.max.y || player.transform.position.y +margen < boxCollider.bounds.min.y)
            {
                playersIn.Remove(player);

                var dir = (Vector2)player.transform.position - boxCollider.ClosestPoint(player.transform.position);
                dir.Normalize();
                player.Jump(dir, 0.2f);

                var ai = player.GetComponent<AIController>();
                if (ai != null)
                {
                    ai.onATreadmill = false;
                }

                break;

            }

            yield return null;
        }

    }


}
