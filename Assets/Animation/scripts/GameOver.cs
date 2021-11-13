using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Text txt1;
    public Text txt2;
    public Text txt3;
    public Text txt4;
    private List<Text> texts;

    void Awake()
    {
        texts = new List<Text>();
        
        texts.Add(txt1);
        texts.Add(txt2);
        texts.Add(txt3);
        texts.Add(txt4);

        foreach (Text txt in texts)
        {
            txt.text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Anim.S.phase == TurnPhase.gameOver)
        {
            int i = 0;
            
            foreach(Text txt in texts)
            {
                if (i <= Anim.S.counters.Count)
                {
                    txt.text = "Color Changed Amount: " + Anim.S.counters[i];
                    i++;
                }
            }
            return;
        }
    }
}
