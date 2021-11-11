using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI1 : MonoBehaviour
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
        if(RatCat.S.phase != TurnPhaseCat.gameOver)
        {
            txt.text = "";
            return;
        }
        if (RatCat.CURRENT_PLAYER == null) return;
        if(RatCat.CURRENT_PLAYER.type == PlayerTypeCat.human)
        {
            txt.text = "You won!";
        }
        else
        {
            txt.text = "Game Over!";
        }
    }
}
