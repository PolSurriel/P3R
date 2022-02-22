﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectUtils : MonoBehaviour
{
    public static GameObject LoadTilemap(int index)
    {
        return (Resources.Load("Tilemaps/Tilemap" + (index + 1), typeof(GameObject)) as GameObject);
    }


    // Load Parallax
    public static Sprite LoadBackground(int index)
    {
        return (Resources.Load("Backgrounds/Back/Back" + (index + 1), typeof(Sprite)) as Sprite);
    }
    public static Sprite LoadMids(int index)
    {
        return (Resources.Load("Backgrounds/Mid/Mid" + (index + 1), typeof(Sprite)) as Sprite);
    }
    public static Sprite LoadFronts(int index)
    {
        return (Resources.Load("Backgrounds/Front/Front" + (index + 1), typeof(Sprite)) as Sprite);
    }
}
