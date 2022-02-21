using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAspect : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody2D rb;

    public Sprite floor;
    public Sprite jump;
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

    public void SetAnimation(State state)
    {
        this.state = state;

        switch (state)
        {
            case State.FLOOR:
                sr.sprite = floor;
                break;
            case State.JUMP:
                sr.sprite = jump;
                break;
            case State.WALL:
                sr.sprite = wall;
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if(state == State.JUMP)
        {
            transform.up = ((Vector2)transform.up + rb.velocity * Time.deltaTime).normalized;
            sr.flipX = rb.velocity.x > 0f;
        }else
        {
            transform.right = Vector3.right;
        }
    }

}
