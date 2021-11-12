using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AIController : MonoBehaviour
{
    bool onATreadmill = false;
    Vector2 treadMillTarget;

    void TreadmilleUpdate()
    {

    }


    public void EnterOnATreadMille(ref List<Transform> exits, Vector2 direction)
    {

        onATreadmill = true;

        // if dot pos -> exit, dir is ok, then choose

    }

}
