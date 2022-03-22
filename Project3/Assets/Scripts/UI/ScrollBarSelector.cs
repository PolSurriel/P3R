using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBarSelector : MonoBehaviour
{
    const int NUMBER_OF_MENUS = 4;
    //horizontalNormalizedPosition
    public ScrollRect scrollbar;
    private float scroll_pos = 0;
    private int indexMenu = 0;
    public float[] pos;
    public float distanceBetweenObjects;
    [SerializeField] Text menuText;

    private bool flagChanging = false;


    private void Update()
    {
        //scrollbar.horizontalNormalizedPosition = Mathf.Clamp(scrollbar.horizontalNormalizedPosition, 0.085f, 0.91f);
        Debug.Log(indexMenu);
        
        if(!flagChanging)
            for(int i = 0; i < pos.Length; i++)
            {
                if (scrollbar.horizontalNormalizedPosition < pos[i] + (distanceBetweenObjects / 2) && scrollbar.horizontalNormalizedPosition > pos[i] - (distanceBetweenObjects / 2))
                {
                    indexMenu = i;
                    ChangeMenuText();
                }
            }
        if (!Input.GetMouseButton(0))
        scrollbar.horizontalNormalizedPosition = Mathf.Lerp(scrollbar.horizontalNormalizedPosition, pos[indexMenu], 0.1f);
    }

    public void IndexMenuInc()
    {
        Debug.Log("Pressed");
        indexMenu++;
        indexMenu = Mathf.Clamp(indexMenu, 0, NUMBER_OF_MENUS);
        ChangeMenuText();
    }

    public void IndexMenuDec()
    {
        indexMenu--;
        indexMenu = Mathf.Clamp(indexMenu, 0, NUMBER_OF_MENUS);
        ChangeMenuText();
    }

    private void ChangeMenuText()
    {
        flagChanging = true;
        StartCoroutine(IsChanging());
        switch (indexMenu)
        {
            case 0:
                menuText.text = "BASE";
                break;
            case 1:
                menuText.text = "SUIT";
                break;
            case 2:
                menuText.text = "ACCESSORY 1";
                break;
            case 3:
                menuText.text = "ACCESSORY 2";
                break;
            case 4:
                menuText.text = "TRAIL";
                break;
            default:
                Debug.LogError("IndexMenu in ScrollBarSelector out of Range");
                break;
        }
    }

    IEnumerator IsChanging()
    {
        yield return new WaitForSeconds(0.5f);
        flagChanging = false;
    }
}
