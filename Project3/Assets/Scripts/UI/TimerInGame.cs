using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerInGame : MonoBehaviour
{
    bool flagLevelEnd = false;
    [SerializeField]
    GameObject levelEnd;
    [SerializeField]
    GameObject rankingSlot;
    // Start is called before the first frame update
    void Start()
    {
        if (GameInfo.instance.levelID != 3)
            this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
		if (!flagLevelEnd)
		{
            this.GetComponent<TextMeshProUGUI>().text = "TIME: " + GameInfo.instance.platformsCountDown.ToString("f2");
            if (GameInfo.instance.platformsCountDown <= 0f)
            {
                int score = (int)GameObject.FindObjectOfType<PlayerController>().transform.position.y;
                Debug.Log("Score: " + score);
                levelEnd.GetComponent<LevelEnd>().SetRankingMenuPlayer((int)GameInfo.matchTimeCounter * 5 + score * 5);
                GameObject _rankingSlot = Instantiate(rankingSlot, levelEnd.GetComponent<LevelEnd>().rankingMenu.transform.GetChild(0).GetChild(1));
                _rankingSlot.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Score: " + score;
                if (score >= GameInfo.instance.maxScore)
                    GameInfo.instance.maxScore = score;
                _rankingSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Max Score: " + GameInfo.instance.maxScore;
                flagLevelEnd = true;
            }
        }
		else
		{
            this.GetComponent<TextMeshProUGUI>().text = "TIME: 0.00";

        }
        
    }

}
