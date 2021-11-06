using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectUtils : MonoBehaviour
{
    public static GameObject LoadTilemap(int index)
    {
        return (Resources.Load("Tilemap" + (index + 1), typeof(GameObject)) as GameObject);
    }

}
