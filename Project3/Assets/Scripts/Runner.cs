using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public class Runner : MonoBehaviour
{
    public Rigidbody2D rb;
    RunnerVFXController vfx;

    private PerksInGame perks;
    public Material myMat;
    
    [HideInInspector]
    public float jumpMagnitude = 10f;

    float playerRadius;
    [HideInInspector]
    public Vector2 contactToSurfaceDirection;
    [HideInInspector]

    public Treadmill treadmill;
    public PlayerAspect aspect;

    public bool onStain;

    bool toExitParent = false;
    public Vector2 lastVelocity;
    public void SetParent(Transform p)
    {
        toExitParent = true;
        transform.SetParent(p);
        transform.localScale = Vector3.one;


    }

    private void FixedUpdate()
    {
        lastVelocity = rb.velocity;
    }

    [HideInInspector]
    public bool onATreadmill = false;
    public void EnterOnATreadmill()
    {
        onATreadmill = true;
        rb.gravityScale = 0f;
        rb.isKinematic = true;
        aspect.SetAnimation(PlayerAspect.State.WALL);
        aspect.SetFlipX(rb.velocity.x < 0f);
    }

    private Vector3 stainCollisionPosition;

    public void EnterOnStain()
    {
        stainJumpsCounter = 0;
        onStain = true;
        aspect.stainParticleSys.Play();

        stainCollisionPosition = transform.position;
    }


    private void Start()
    {
        transform.GetChild(3).position = Vector2.one * 9999f;

        playerRadius = GetComponent<CircleCollider2D>().radius;
        rb = GetComponent<Rigidbody2D>();
        vfx = GetComponent<RunnerVFXController>();
        perks = GetComponent<PerksInGame>();



        aspect.rb = rb;
        aspect.runner = this;

    }

    const int STAIN_TIMES_TO_JUMP = 2;
    int stainJumpsCounter;

    void JumpOnStain(Vector2 direction)
    {

        if (isPlayer)
        {
            AudioController.instance.sounds.stain.Play();

        }else
        {
            AudioController.instance.sounds.stainAI.Play();

        }

        stainJumpsCounter++;

        if (stainJumpsCounter >= STAIN_TIMES_TO_JUMP)
        {
            onStain = false;
        }

        // TODO
    }

    public bool usingPortal;

    public void EnterOnAPortal()
    {
        if (isPlayer)
        {
            AudioController.instance.sounds.portal.Play();

        }else
        {
            AudioController.instance.sounds.portalAI.Play();
        }

        //usingPortal = true;
    }

    void CantJumpFeedback()
    {



    }


    public bool ignoreTreadmill = false;

    IEnumerator IgnoreTreadmill()
    {
        float t = 0f;
        do { yield return null; } while ((t += Time.deltaTime) < 0.3f);
        ignoreTreadmill = false;

    }

    [HideInInspector]
    public bool isPlayer = false;

    public void Jump(Vector2 direction, float forcePercentage = 1f)
    {


        aspect.SetAnimation(PlayerAspect.State.JUMP);

        if (onATreadmill)
        {
            treadmill.playersIn.Remove(this);
            ignoreTreadmill = true;
            StartCoroutine(IgnoreTreadmill());
            onATreadmill = false;
            rb.isKinematic = false;
            rb.gravityScale = 1f;
        }

        switch (jumpCounter)
        {
            case 0:
                if (isPlayer)
                {
                    AudioController.instance.sounds.player_jump.Play();
                }
                else
                {
                    AudioController.instance.sounds.runner_jump.Play();
                }
                break;
            case 1:
                if (isPlayer)
                {
                    AudioController.instance.sounds.player_doubleJump.Play();
                }
                else
                {
                    AudioController.instance.sounds.runner_doubleJump.Play();
                }
                break;
            default:
                CantJumpFeedback();
                return;
        }


        if (onStain)
        {
            float dist = Vector3.Distance(transform.position, stainCollisionPosition);

            // If to check if runner is jumping from the stain
            if (dist < 1f)
            {
                aspect.stainParticleSys.Play();
                JumpOnStain(direction);
                return;
            }
        }

        jumpCounter++;


        if (toExitParent)
        {
            transform.SetParent(null);

            transform.localScale = Vector3.one;
        }


        direction = direction.normalized;

        rb.isKinematic = false;
        rb.velocity = Vector2.zero;
        if(perks == null)
            rb.AddForce(direction.normalized * jumpMagnitude * forcePercentage, ForceMode2D.Impulse);
        else
            rb.AddForce(direction.normalized * jumpMagnitude * forcePercentage * perks.extraVelocityPercentage, ForceMode2D.Impulse);
        
        transform.position = transform.position + (Vector3)(contactToSurfaceDirection * 0.1f);

        jumpDirection = direction;


        //Color c = Color.yellow;
        //c.a = 0.4f;

        //Debug.DrawLine(transform.position, (Vector2)transform.position + direction * 100f, c, 10f);
        if(perks != null)
            perks.CheckJumpPerks(this);

    }

    public float GetImpulseMagnitude()
    {
        return jumpMagnitude;
    }



    Vector2 jumpDirection;
    private void OnDrawGizmos()
    {

#if UNITY_EDITOR
            //Debug.DrawLine(transform.position, (Vector2)transform.position + contactToSurfaceDirection.normalized * 0.5f);
            //Handles.Label((Vector2)transform.position + Vector2.one * 0.5f, "" + jumpCounter);

#endif
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("transitionLine") || collision.collider.CompareTag("floor"))
        {
            jumpCounter = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "extraJumpZone" && !collision.GetComponent<ExtraJumpZone>().ignoring.Contains(this))
        {
            jumpCounter = 0;
        }

        if (!usingPortal)
        {
            if (floorCollisionEnabled && collision.CompareTag("transitionLine"))
            {
                if (rb.velocity.y < 0f)
                    CollideWithFloorTransition(collision);
            }

        }


    }


    

    void CollideWithFloorTransition(Collider2D collider)
    {

        aspect.SetAnimation(PlayerAspect.State.FLOOR);
        jumpCounter = 0;
        ResetFloorCollision();
        Vector2 contact = collider.ClosestPoint(transform.position);
        vfx.OnCollisionWithWall(contact);
        if (contact.y < collider.bounds.max.y)
            contact.y = collider.bounds.max.y;

        contactToSurfaceDirection = Vector2.up;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        


        transform.position = contact + Vector2.up * playerRadius;


    }

    [HideInInspector]
    public bool edgeWallUp = false;

    [HideInInspector]
    public bool edgeWallDown = false;

    [HideInInspector]
    public bool wallup;
    void CollideWithFloor(Collision2D collision)
    {
        if (isPlayer)
        {
            AudioController.instance.sounds.wallCollision.Play();
        }else
        {
            AudioController.instance.sounds.wallCollisionAI.Play();
        }

        vfx.OnCollisionWithWall(collision.contacts[0].point);
        wallup = false;
        jumpCounter = 0;
        ResetFloorCollision();
        contactToSurfaceDirection = collision.contacts[0].normal.normalized;
        Vector2 contact = collision.contacts[0].point;

        if(contactToSurfaceDirection.y == -1f)
        {
            wallup = true;
            aspect.SetAnimation(PlayerAspect.State.WALLUP);

        } else if (contactToSurfaceDirection.y == 1f)
        {
            aspect.SetAnimation(PlayerAspect.State.FLOOR);

        }else
        {
            float offset = 0.3f;

            Vector2 posUp = (Vector2)transform.position + Vector2.up * 0.1f;
            edgeWallUp = !Physics2D.Linecast(posUp + Vector2.left * offset, posUp + Vector2.right * offset, LayerMask.GetMask("floor"));

            Vector2 posDown = (Vector2)transform.position + Vector2.down * 0.1f;
            edgeWallDown = !Physics2D.Linecast(posDown + Vector2.left * offset, posDown + Vector2.right * offset, LayerMask.GetMask("floor"));

            if (edgeWallDown)
            {
                aspect.SetAnimation(PlayerAspect.State.WALLUP);
                
            }else
            {
                aspect.SetAnimation(PlayerAspect.State.WALL);
            }

            aspect.SetFlipX(contactToSurfaceDirection.x > 0f);

        }

        transform.position = contact + contactToSurfaceDirection * playerRadius;

        rb.isKinematic = true;
        rb.velocity = Vector2.zero;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (!usingPortal)
        {
            if (floorCollisionEnabled && collision.collider.CompareTag("floor"))
            {
                CollideWithFloor(collision);
            }
            



        }


    }



}
