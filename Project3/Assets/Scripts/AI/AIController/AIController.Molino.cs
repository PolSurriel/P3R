using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AIController: MonoBehaviour
{
    bool onAMolino;

    public void EnterOnMolino()
    {
        onAMolino = true;
    }

    public void ExitMolino()
    {
        onAMolino = false;
    }


}
