using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardClick : MonoBehaviour
{
    public string designator;
    public GameObject otherButton;
    public GameObject confirm;
    
    // Start is called before the first frame update
    void Start()
    {
        if (designator == "show")
        {
            this.gameObject.SetActive(true);
        }
        else if(designator == "back")
        {
            this.gameObject.SetActive(false);
        }
        else if(designator == "confirm")
        {
            this.gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowCards()
    {
        RatCat.CURRENT_PLAYER.hand[0].faceUp = true;
        RatCat.CURRENT_PLAYER.hand[3].faceUp = true;
        if(otherButton.GetComponent<CardClick>().designator == "back")
        {
            otherButton.SetActive(true);
        }
    }

    public void ShowConfirm()
    {
        confirm.SetActive(true);
    }

    public void HideCards()
    {
        RatCat.CURRENT_PLAYER.hand[0].faceUp = false;
        RatCat.CURRENT_PLAYER.hand[3].faceUp = false;
        if (otherButton.GetComponent<CardClick>().designator == "show")
        {
            otherButton.SetActive(true);
        }
    }

    public void HideAll()
    {
        GameObject obj = otherButton.GetComponent<CardClick>().otherButton;
        otherButton.SetActive(false);
        obj.SetActive(false);
        RatCat.CURRENT_PLAYER.hand[0].faceUp = false;
        RatCat.CURRENT_PLAYER.hand[3].faceUp = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
