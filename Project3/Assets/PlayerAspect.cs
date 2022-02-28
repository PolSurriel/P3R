using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAspect : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody2D rb;

    [HideInInspector]
    public Runner runner;

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
        WALL,
        WALLUP
    }

    public void SetFlipX(bool flip)
    {
        sr.flipX = flip;
    }

    SpriteRenderer sr;

    PlayerAnimationController animationController;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animationController = GetComponent<PlayerAnimationController>();

        animationController.baseSkin = "Yellow";
        animationController.suitSkin = "Default";
        animationController.accessory1Skin = "Default";
        animationController.accessory2Skin = "Default";
        animationController.LoadAnimations();


    }

    State state;

    int jumpSelected;

    public void SetAnimation(State state)
    {
        this.state = state;


        switch (state)
        {

            case State.WALLUP:
                animationController.Play_wallup();
                break;

            case State.FLOOR:


                
                // RIGHT
                if (Vector2.Dot(transform.up, Vector2.right) >= 0.25f)
                {
                    if (sr.flipX)
                    {
                        animationController.Play_floor4();

                    }
                    else
                    {
                        animationController.Play_floor5();

                    }

                }
                // UP
                else if(Vector2.Dot(transform.up, Vector2.up) >= 0.25f)
                {
                    if(Random.RandomRange(0f, 1f)< 0.5f)
                    {
                        animationController.Play_floor1();

                    }else
                    {
                        animationController.Play_floor3();
                    }
                }

                // DONW
                else if(Vector2.Dot(transform.up, Vector2.down) >= 0.25f)
                {
                    animationController.Play_floor2();

                }

                
                else
                {
                    if (!sr.flipX)
                    {
                        animationController.Play_floor4();

                    }
                    else
                    {
                        animationController.Play_floor5();

                    }

                }


                break;
            case State.JUMP:

                if (Mathf.Abs(lastFrameVelocity.x) > Mathf.Abs(lastFrameVelocity.y))
                {
                    jumpSelected = Random.Range(0,3);

                }else
                {
                    jumpSelected = Random.Range(0, 2);
                }


                switch (jumpSelected)
                {
                    case 0:
                        animationController.Play_jump1();
                        //sr.sprite = jump;
                        break;
                    case 1:
                        animationController.Play_jump2();
                        //sr.sprite = jump2;
                        transform.up = rb.velocity.normalized;
                        break;
                    case 2:
                        animationController.Play_jump3();
                        //sr.sprite = jump3;
                        break;
                }
                break;
            case State.WALL:

                
                if (runner.edgeWallUp)
                {
                    animationController.Play_wall_edge();
                }
                else
                {
                    animationController.Play_wall();

                }
                //sr.sprite = wall;
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

                    float speed = rb.velocity.magnitude;

                    if(speed > 2f)
                    {
                        animationController.SetJump1Speed(1f);
                    }else
                    {
                        animationController.SetJump1Speed(speed* (1f/2f));
                    }

                    float dot = Vector2.Dot(rb.velocity.normalized, transform.up);
                    if (dot < 0.1f)
                    {
                        var prev = transform.up;
                        // Rotate faster
                        transform.up = ((Vector2)transform.up + rb.velocity*3f * Time.deltaTime).normalized;
                        
                        if(prev == transform.up)
                        {
                            transform.up = ((Vector2)transform.up + (sr.flipX ? Vector2.right: Vector2.left) * 3f * Time.deltaTime).normalized;
                        }

                    }

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
