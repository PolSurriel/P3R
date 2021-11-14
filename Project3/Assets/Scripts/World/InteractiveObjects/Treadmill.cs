using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treadmill : MonoBehaviour
{
    List<Runner> playersIn = new List<Runner>();
    public Vector2 direction = Vector2.up;
    public float speed = 20f;
    public List<Transform> exits = new List<Transform>();
    float distanceToExit = 0.5f;


    private void Start()
    {
        direction = direction.normalized;
    }

    private void Update()
    {
        foreach (var player in playersIn)
        {
            player.transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var runner = collision.collider.GetComponent<Runner>();

        playersIn.Add(runner);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var runner = collision.collider.GetComponent<Runner>();

        playersIn.Remove(runner);

        var ai = runner.GetComponent<AIController>();

        if(ai != null)
        {
            ai.EnterOnATreadMille(ref exits, direction);
        }
    }
}
