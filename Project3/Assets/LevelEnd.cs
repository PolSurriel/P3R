using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{

    private void Start()
    {
        if (GameInfo.instance == null || GameInfo.instance.levelID == 0 || GameInfo.instance.levelID == 3)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("end");

        if(collision.GetComponent<PlayerController>() != null || (collision.GetComponent<AIController>() != null && GameInfo.instance.levelID == 2))
        {
            GameObject.FindObjectOfType<MapController>().ResetMap();
            MatchController.instance.Quit();
        }
    }

}
