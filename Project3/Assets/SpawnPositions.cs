using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPositions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameInfo.instance == null)
            return;


        List<Transform> positions = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            positions.Add(transform.GetChild(i));
        }

        int selectedIndex =Random.Range(0, positions.Count);
        GameInfo.instance.player.transform.position = positions[selectedIndex].position;
        positions.RemoveAt(selectedIndex);

        if (GameInfo.instance.ai_players == null)
            return;

        foreach(var ai in GameInfo.instance.ai_players)
        {
            selectedIndex = Random.Range(0, positions.Count);
            ai.transform.position = positions[selectedIndex].position;
            positions.RemoveAt(selectedIndex);

        }



    }

}
