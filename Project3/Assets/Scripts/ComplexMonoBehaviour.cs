using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComplexMonoBehaviour : MonoBehaviour
{
    protected abstract string GetDocsLink();
    [PropertyOrder(-999)]
    [BoxGroup("DOCUMENTATION", centerLabel: true)]
    [InfoBox("This component requires complex attribute configuration. Please make sure you understand the documentation before editing it.")]
    [BoxGroup("DOCUMENTATION", centerLabel: true)]
    [Button("Open documentation")]
    void ShowDocumentation()
    {
        Application.OpenURL(GetDocsLink());
    }

}
