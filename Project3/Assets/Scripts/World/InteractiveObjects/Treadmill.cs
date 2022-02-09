using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treadmill : MonoBehaviour
{
    List<Runner> playersIn = new List<Runner>();
    public Vector2 direction = Vector2.up;
    public Vector2 normal = Vector2.right;
    public float speed = 20f;
    public List<Transform> exits = new List<Transform>();


    private void Start()
    {
        direction = direction.normalized;
    }

    private void Update()
    {
        
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

        var ai = runner.GetComponent<AIController>();

        if(ai != null)
        {
            ai.EnterOnATreadMille(ref exits, direction);
        }

        playersIn.Add(runner);
    }
    
    IEnumerator ExitPlayer(Runner player)
    {
        float t = 0f;

        do
        {
            yield return null;
        } while ((t+= Time.deltaTime)<0.3f);

        playersIn.Remove(player);

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var runner = collision.collider.GetComponent<Runner>();

        if (runner != null)
            StartCoroutine(ExitPlayer(runner));

    }
}
