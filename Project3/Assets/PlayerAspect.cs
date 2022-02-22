using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAspect : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody2D rb;

    public Sprite floor;
    public Sprite floor2;
    public Sprite jump;
    public Sprite jump2;
    public Sprite jump3;
    public Sprite wall;

   
    public enum State
    {
        FLOOR,
        JUMP,
        WALL
    }

    public void SetFlipX(bool flip)
    {
        sr.flipX = flip;
    }

    SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    State state;

    int jumpSelected;

    public void SetAnimation(State state)
    {
        this.state = state;

        switch (state)
        {
            case State.FLOOR:

                if(transform.up.y < 0f && lastFrameVelocity.normalized.y < -0.5f)
                {
                    sr.sprite = floor2;

                }else
                {
                    sr.sprite = floor;

                }

                
                break;
            case State.JUMP:

                jumpSelected = Random.Range(0,3);

                switch (jumpSelected)
                {
                    case 0:
                        sr.sprite = jump;
                        break;
                    case 1:
                        sr.sprite = jump2;
                        transform.up = rb.velocity.normalized;
                        break;
                    case 2:
                        sr.sprite = jump3;
                        break;
                }
                break;
            case State.WALL:
                sr.sprite = wall;
                break;
            default:
                break;
        }
    }

    Vector2 lastFrameVelocity;

    private void LateUpdate()
    {
        lastFrameVelocity = rb.velocity;
    }

    private void Update()
    {
        

        if (state == State.JUMP)
        {
            switch (jumpSelected)
            {
                case 0:
                    transform.up = ((Vector2)transform.up + rb.velocity * Time.deltaTime).normalized;
                    sr.flipX = rb.velocity.x > 0f;
                    break;
                case 1:
                    float sense = rb.velocity.x > 0f ? -1f : 1f;
                    transform.Rotate(Vector3.forward * Time.deltaTime * rb.velocity.magnitude * 40f * sense);
                    sr.flipX = rb.velocity.x > 0f;
                    break;
                case 2:
                    transform.up = ((Vector2)transform.up + rb.velocity * Time.deltaTime * 2f).normalized;
                    sr.flipX = rb.velocity.x < 0f;
                    break;
            }

        }else
        {
            transform.right = Vector3.right;
        }
    }

}
