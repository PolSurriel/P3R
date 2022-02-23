using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public int numberOfBackgrounds, numberOfMids, numberOfFronts;

    private float camLength, prevPosition;
    [SerializeField] private GameObject camera;

    [Header("Multiplicadores para el efecto Parallax")]
    [SerializeField] private float parallaxBackgroundMult;
    [SerializeField] private float parallaxMidMult;
    [SerializeField] private float parallaxFrontMult;

    [SerializeField] private GameObject backgrounds;
    [SerializeField] private GameObject mids;
    [SerializeField] private GameObject fronts;

    [Header("Setear el Sprite de más arriba")]
    [SerializeField] private GameObject lastBackgroundChanged;
    [SerializeField] private GameObject lastMidChanged;
    [SerializeField] private GameObject lastFrontChanged;

    private Sprite[] allBackgroundsSprites;
    private Sprite[] allMidsSprites;
    private Sprite[] allFrontsSprites;
    // Start is called before the first frame update
    void Start()
    {
        LoadBackgrounds();
        InitializeVariables();
    }

    // Update is called once per frame
    void Update()
    {   
        float cameraDistance = camera.transform.position.y - prevPosition;
        prevPosition = camera.transform.position.y;
        // dist es la distancia que retrocede cada una de las imagenes
        // dependiendo de cuanto parallax se les haya assignado
        float distBackground = cameraDistance * parallaxBackgroundMult;
        float distMid = cameraDistance * parallaxMidMult;
        float distFront = cameraDistance * parallaxFrontMult;

        MoveParallax(distBackground, distMid, distFront);
        CheckParallax();
    }

    void CalculateHeight()
    {
        this.transform.position = Vector3.up * totalHeightAcumulated;
        this.transform.SetParent(this.transform);

        totalHeightAcumulated += this.GetComponent<Sprite>().border.y;
    }

    void InitializeVariables()
    {
        camLength = camera.orthographicSize * 2;
    }

    void LoadBackgrounds()
    {
        allBackgroundsSprites = new Sprite[numberOfBackgrounds];
        allMidsSprites = new Sprite[numberOfMids];
        allFrontsSprites = new Sprite[numberOfFronts];
        for (int i = 0; i < numberOfBackgrounds; i++)
        {
            ProjectUtils.LoadTilemap(i);
            allBackgroundsSprites[i] = ProjectUtils.LoadBackground(i);
        }
        for (int i = 0; i < numberOfMids; i++)
        {
            ProjectUtils.LoadTilemap(i);
            allMidsSprites[i] = ProjectUtils.LoadBackground(i);
        }
        for (int i = 0; i < numberOfFronts; i++)
        {
            ProjectUtils.LoadTilemap(i);
            allFrontsSprites[i] = ProjectUtils.LoadBackground(i);
        }
    }

    // Mueve los Sprites para dar el efecto Parallax
    void MoveParallax(float _distBackground, float _distMid, float _distFront)
    {
        foreach(Transform child in backgrounds)
        {
            child.position = new Vector3(child.position.x, child.position.y + _distBackground, child.position.z);
        }
        foreach(Transform child in mids)
        {
            child.position = new Vector3(child.position.x, child.position.y + _distMid, child.position.z);
        }
        foreach(Transform child in fronts)
        {
            child.position = new Vector3(child.position.x, child.position.y + _distFront, child.position.z);
        }
    }

    void CheckParallax()
    {
        foreach(Transform child in backgrounds)
        {
            float maxDistance = camera.transform.position.y - camLength - 2* child.GetComponent<Sprite>().border.y;
            if(child.position.y < maxDistance)
            {
                // Cambiar Sprite y recolocarlo
                child.transform.position = lastBackgroundChanged.transform.position + new Vector3(0,lastBackgroundChanged.GetComponent<Sprite>().border.y / 2 + child.GetComponent<Sprite>().border.y /2,0);
                lastBackgroundChanged = child;
            }
        }
        foreach(Transform child in mids)
        {
            float maxDistance = camera.transform.position.y - camLength - 2* child.GetComponent<Sprite>().border.y;
            if(child.position.y < maxDistance)
            {
                // Cambiar Sprite y recolocarlo
                child.transform.position = lastMidChanged.transform.position + new Vector3(0,lastBackgroundChanged.GetComponent<Sprite>().border.y / 2 + child.GetComponent<Sprite>().border.y /2,0);
                lastMidChanged = child;
            }
        }
        foreach(Transform child in fronts)
        {
            float maxDistance = camera.transform.position.y - camLength - 2* child.GetComponent<Sprite>().border.y;
            if(child.position.y < maxDistance)
            {
                // Cambiar Sprite y recolocarlo
                child.transform.position = lastFrontChanged.transform.position + new Vector3(0,lastBackgroundChanged.GetComponent<Sprite>().border.y / 2 + child.GetComponent<Sprite>().border.y /2,0);
                lastFrontChanged = child;
            }
        }
    }
}
