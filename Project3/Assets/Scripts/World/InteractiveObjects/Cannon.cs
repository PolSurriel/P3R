using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{

    public GameObject projectilePrefab;     
    float frecuency = 3f;
    public Vector2 shootDirection;
    public float forceMagnitude;

    float timeCounter = 0f;

    private void Start()
    {
        shootDirection.Normalize();
    }

    void Shoot()
    {
        var obj = Instantiate(projectilePrefab);
        obj.transform.position = transform.position;
        obj.GetComponent<Rigidbody2D>().velocity = shootDirection * forceMagnitude;
    }

    private void Update()
    {
        timeCounter += Time.deltaTime;

        if(timeCounter >= frecuency)
        {
            timeCounter = 0f;
            Shoot();
        }
    }



}
