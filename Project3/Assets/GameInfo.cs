using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInfo : MonoBehaviour
{

    public static GameInfo instance;

    //Esto es temporal para la pre-alpha. De hecho los niveles dejaran de existir despues de la entrega.
    public int levelID;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
            return;

        }

        instance = this;
        DontDestroyOnLoad(instance.gameObject);

    }

    


}
