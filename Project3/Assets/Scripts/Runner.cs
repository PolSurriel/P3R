using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour
{
    public Rigidbody2D rb;
    float jumpMagnitude = 10f;

    float playerRadius; 
    Vector2 contactToSurfaceDirection;


    bool onStain;

    bool toExitParent = false;
    public void SetParent(Transform p)
    {
        toExitParent = true;
        transform.SetParent(p);
    }

    public void EnterOnStain()
    {
        stainJumpsCounter = 0;
        onStain = true;
    }

    private void Start()
    {
        playerRadius = GetComponent<CircleCollider2D>().radius;
        rb = GetComponent<Rigidbody2D>();
        
    }

    const int STAIN_TIMES_TO_JUMP = 2;
    int stainJumpsCounter;

    void JumpOnStain(Vector2 direction)
    {
        stainJumpsCounter++;

        if(stainJumpsCounter >= STAIN_TIMES_TO_JUMP)
        {
            onStain = false;
        }

        // TODO
    }

    void CantJumpFeedback()
    {

    }

    public void Jump(Vector2 direction, float forcePercentage = 1f)
    {
        if( jumpCounter >= 2)
        {
            CantJumpFeedback();
            return;
        }


        if (onStain)
        {
            JumpOnStain(direction);
            return;
        }

        if (toExitParent)
        {
            transform.SetParent(null);
        }


        direction = direction.normalized;

        rb.isKinematic = false;
        rb.velocity = Vector2.zero;
        rb.AddForce(direction.normalized * jumpMagnitude * forcePercentage, ForceMode2D.Impulse);

        transform.position = transform.position + (Vector3)(contactToSurfaceDirection*0.1f);

        jumpDirection = direction;

        //Color c = Color.yellow;
        //c.a = 0.4f;

        //Debug.DrawLine(transform.position, (Vector2)transform.position + direction * 100f, c, 10f);

    }

    public float GetImpulseMagnitude()
    {
        return jumpMagnitude;
    }

    Vector2 jumpDirection;
    private void OnDrawGizmos()
    {
        //Debug.DrawLine(transform.position, transform.position + (Vector3) jumpDirection * 100f, new Color(0f, 1f, 0f, 0.3f));
        //Debug.DrawLine(transform.position, transform.position + (Vector3) GetComponent<Rigidbody2D>().velocity, new Color(1f, 0f, 0f, 0.3f));
    }

    bool floorCollisionEnabled = true;

    IEnumerator WaitAndEnableFloorCollision(float time)
    {
        float tc = 0f;
        do { yield return null; } 
        while ((tc += Time.deltaTime) < time);

        floorCollisionEnabled = true;

    }

    public void ResetFloorCollision()
    {
        floorCollisionEnabled = false;
        StartCoroutine(WaitAndEnableFloorCollision(0.3f));
    }

    
    [HideInInspector]
    public int jumpCounter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "extraJumpZone")
        {
            jumpCounter = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (floorCollisionEnabled && collision.collider.CompareTag("floor"))
        {
            jumpCounter = 0;
            ResetFloorCollision();
            contactToSurfaceDirection = collision.contacts[0].normal.normalized;
            Vector2 contact = collision.contacts[0].point; 

            transform.position = contact + contactToSurfaceDirection * playerRadius;

            rb.isKinematic = true;
            rb.velocity = Vector2.zero;

        }


    }



}
