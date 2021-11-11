using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundResultUI1 : MonoBehaviour
{
    private Text txt;

    void Awake()
    {
        txt = GetComponent<Text>();
        txt.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (RatCat.S.phase != TurnPhaseCat.gameOver)
        {
            txt.text = "";
            return;
        }
        CatPlayer cP = RatCat.CURRENT_PLAYER;
        if (cP == null || cP.type == PlayerTypeCat.human)
        {
            txt.text = "";
        }
        else
        {
            txt.text = "Player " + (cP.playerNum) + " won!";
        }
    }
}
