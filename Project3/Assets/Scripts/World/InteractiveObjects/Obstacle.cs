using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    float scaleStart;
    float deltaScale = 1.5f;
    float rescaleSpeed = 10f;

    float maxScale;
    SpriteRenderer sr;

    Color spriteRendererStartColor;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        spriteRendererStartColor = sr.color;
        scaleStart = transform.localScale.x;
        maxScale = scaleStart + deltaScale;
    }

    float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }


    private void Update()
    {
        if(transform.localScale.x != scaleStart)
        {
            float newScale = transform.localScale.x - rescaleSpeed * Time.deltaTime;

            float scalar1 = Remap(newScale, scaleStart, maxScale, 0f, 1f);
            float scalar2 = 1f - scalar1;

            sr.color = Color.white * scalar1 + spriteRendererStartColor * scalar2;
            
            if (newScale < scaleStart)
            {
                newScale = scaleStart;
                sr.color = spriteRendererStartColor;

            }

            transform.localScale = new Vector3(newScale, newScale, newScale);



        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Player")
        {
            var runner = collision.collider.GetComponent<Runner>();

            Vector2 dir = runner.transform.position - transform.position;
            dir.Normalize();

            sr.color = Color.white;

            runner.rb.velocity = dir * 10f;
            transform.localScale = new Vector3(scaleStart + deltaScale, scaleStart + deltaScale, scaleStart + deltaScale);
            Debug.LogError("alert");
        }
    }



}
