using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public int numberOfBackgrounds, numberOfMids, numberOfFronts;

    private float length, startPos;
    [SerializeField] private GameObject camera;
    public float parallaxEffect;

    private float totalHeightAcumulated;

    Sprite[] allBackgrounds;
    Sprite[] allMids;
    Sprite[] allFronts;
    // Start is called before the first frame update
    void Start()
    {
        LoadBackgrounds();
        InitializeVariables();
        Debug.Log(this.name + ": " + length);
    }

    // Update is called once per frame
    void Update()
    {
        float temp = camera.transform.position.y * (1 - parallaxEffect);
        float dist = camera.transform.position.y * parallaxEffect;
        transform.position = new Vector3(transform.position.x, startPos + dist, transform.position.z);

        if (temp > startPos + length)
        {
            startPos += length;
        }
        else if (temp < startPos - length) 
            startPos -= length;
    }

    void CalculateHeight()
    {
        this.transform.position = Vector3.up * totalHeightAcumulated;
        this.transform.SetParent(this.transform);

        totalHeightAcumulated += this.GetComponent<Sprite>().border.y;
    }

    void InitializeVariables()
    {
        startPos = this.transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void LoadBackgrounds()
    {
        allBackgrounds = new Sprite[numberOfBackgrounds];
        allMids = new Sprite[numberOfMids];
        allFronts = new Sprite[numberOfFronts];
        for (int i = 0; i < numberOfBackgrounds; i++)
        {
            ProjectUtils.LoadTilemap(i);
            allBackgrounds[i] = ProjectUtils.LoadBackground(i);
        }
        for (int i = 0; i < numberOfMids; i++)
        {
            ProjectUtils.LoadTilemap(i);
            allMids[i] = ProjectUtils.LoadBackground(i);
        }
        for (int i = 0; i < numberOfFronts; i++)
        {
            ProjectUtils.LoadTilemap(i);
            allFronts[i] = ProjectUtils.LoadBackground(i);
        }
    }
}
