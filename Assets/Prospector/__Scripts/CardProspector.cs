using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCardState
{
    drawpile,
    tableau,
    target,
    discard
}

public class CardProspector : Card
{
    public eCardState state = eCardState.drawpile;
    public List<CardProspector> hiddenBy = new List<CardProspector>();

    public int layoutID;
    public SlotDef slotDef;
    public bool isGold = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    override public void OnMouseUpAsButton()
    {
        if(Prospector.S != null)
        {
            Prospector.S.CardClicked(this);
        }
        if(Elevens.S != null)
        {
            Elevens.S.CardClicked(this);
        }
        base.OnMouseUpAsButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
