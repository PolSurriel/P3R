﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreadmillAnimation : MonoBehaviour
{

    public bool goesup;

    Material mat;

    float currentOffset = -1f;

    float textureHeight;


    float convertedSpeed = 0f;

    private void Awake()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        mat = spriteRenderer.material;

        // En un speed de 1, la animación de los uv se desplaza toda la textura 1 sola vez, es decir, la cantidad exacta de pixeles por height.
        // Para poder igualar las unidades de la animación (pixels) a las unidades de unity (transform.position) tenemos que convertir el speed en relación a unidades de mundo de unity.
        // En este proyecto, 1 unidad de mundo = a 100 pixels, por lo que la fórmula deseada es: speed = 1f / (height / 100f) => (simplificado) speed = 100f / height

        // Encontramos el valor de height desde el sprite porque desde el material no reconoce la textura asignada, o almenos, no en su mainTexture.
        // mat.mainTexture.height => 256px NO
        // spriteRenderer.texture.height => 26 YES 
        textureHeight = spriteRenderer.sprite.texture.height;
    }

    public void SetSpeed(float worldSpeed)
    {
        convertedSpeed = 100f * worldSpeed / textureHeight;
    }


    // Update is called once per frame
    void Update()
    {
        mat.SetFloat("_OffsetUvY", currentOffset += Time.deltaTime * (goesup ? -1f : 1f) * convertedSpeed);
    }
}