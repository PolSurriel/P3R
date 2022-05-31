using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAspect : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody2D rb;

    [HideInInspector]
    public Runner runner;


    public ParticleSystem stainParticleSys;

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
        animationController.suitSR.flipX = flip;
        animationController.accessory1SR.flipX = flip;
        animationController.accessory2SR.flipX = flip;
        animationController.baseSR.flipX = flip;


    }

    [HideInInspector]
    public SpriteRenderer sr;

    public PlayerAnimationController animationController;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if(animationController.baseSkin == "")
        {
            animationController.baseSkin = "Yellow";
            animationController.suitSkin = "Default";
            animationController.accessory1Skin = "Default";
            animationController.accessory2Skin = "Default";
        }
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
                if (Vector2.Dot(animationController.aspectContainer.up, Vector2.right) >= 0.25f)
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
                else if(Vector2.Dot(animationController.aspectContainer.up, Vector2.up) >= 0.25f)
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
                else if(Vector2.Dot(animationController.aspectContainer.up, Vector2.down) >= 0.25f)
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
                        animationController.aspectContainer.up = rb.velocity.normalized;
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
        //lastFrameVelocity = rb.velocity;
        transform.localRotation = Quaternion.identity;
    }

    private void Update()
    {
        

        if (state == State.JUMP)
        {
            switch (jumpSelected)
            {
                case 0:
                    animationController.aspectContainer.up = ((Vector2)animationController.aspectContainer.up + rb.velocity * Time.deltaTime).normalized;
                    SetFlipX(rb.velocity.x > 0f);

                    float speed = rb.velocity.magnitude;

                    if(speed > 2f)
                    {
                        animationController.SetJump1Speed(1f);
                    }else
                    {
                        animationController.SetJump1Speed(speed* (1f/2f));
                    }

                    float dot = Vector2.Dot(rb.velocity.normalized, animationController.aspectContainer.up);
                    if (dot < 0.1f)
                    {
                        var prev = animationController.aspectContainer.up;
                        // Rotate faster
                        animationController.aspectContainer.up = ((Vector2)animationController.aspectContainer.up + rb.velocity*3f * Time.deltaTime).normalized;
                        
                        if(prev == animationController.aspectContainer.up)
                        {
                            animationController.aspectContainer.up = ((Vector2)animationController.aspectContainer.up + (sr.flipX ? Vector2.right: Vector2.left) * 3f * Time.deltaTime).normalized;
                        }

                    }

                    break;
                case 1:
                    float sense = rb.velocity.x > 0f ? -1f : 1f;
                    animationController.aspectContainer.Rotate(Vector3.forward * Time.deltaTime * rb.velocity.magnitude * 40f * sense);
                    SetFlipX(rb.velocity.x > 0f);
                    break;
                case 2:
                    animationController.aspectContainer.up = ((Vector2)animationController.aspectContainer.up + rb.velocity * Time.deltaTime * 2f).normalized;
                    SetFlipX(rb.velocity.x < 0f);
                    break;
            }

        }else
        {
            animationController.aspectContainer.right = Vector3.right;
        }

    }

}
