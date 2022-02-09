using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Por el momento los molinos han sido descartados como interactive objects.

 */

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
