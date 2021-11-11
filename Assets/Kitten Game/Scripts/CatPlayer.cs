using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PlayerTypeCat
{
    human,
    ai
}

[System.Serializable]
public class CatPlayer
{
    public PlayerTypeCat type = PlayerTypeCat.ai;
    public int playerNum;
    public SlotDefCat handSlotDef;
    public List<CardCat> hand;

    public CardCat AddCard(CardCat eCC)
    {
        if (hand == null) hand = new List<CardCat>();

        hand.Add(eCC);

        if (type == PlayerTypeCat.human)
        {
            CardCat[] cards = hand.ToArray();

            cards = cards.OrderBy(cd => cd.rank).ToArray();

            hand = new List<CardCat>(cards);
        }

        eCC.SetSortingLayerName("10");
        eCC.eventualSortLayer = handSlotDef.layerName;

        FanHand();

        return (eCC);
    }

    public CardCat RemoveCard(CardCat cc)
    {
        if (hand == null || !hand.Contains(cc)) return null;

        hand.Remove(cc);

        FanHand();

        return (cc);
    }

    public void FanHand()
    {
        float startRot = 0;
        startRot = handSlotDef.rot;
        if (hand.Count > 1)
        {
            startRot += RatCat.S.handFanDegrees * (hand.Count - 1) / 2;
        }

        Vector3 pos;
        float rot;
        Quaternion rotQ;
        for (int i = 0; i < hand.Count; i++)
        {
            rot = startRot - RatCat.S.handFanDegrees * i;
            rotQ = Quaternion.Euler(0, 0, rot);

            pos = Vector3.up * CardCat.CARD_HEIGHT / 2f;

            pos = rotQ * pos;

            pos += handSlotDef.pos;
            pos.z = -0.5f * i;

            if (RatCat.S.phase != TurnPhaseCat.idle)
            {
                hand[i].timeStart = 0;
            }

            hand[i].MoveTo(pos, rotQ);
            hand[i].state = CCState.toHand;

            /*
            hand[i].transform.localPosition = pos;
            hand[i].transform.rotation = rotQ;
            hand[i].state = CBState.hand;
            */

            hand[i].faceUp = (type == PlayerTypeCat.human);

            hand[i].eventualSortOrder = i * 4;

        }
    }

    public void TakeTurn()
    {
        Utils.tr("Player.TakeTurn");

        if (type == PlayerTypeCat.human) return;

        RatCat.S.phase = TurnPhaseCat.waiting;

        CardCat cc;

        List<CardCat> validCards = new List<CardCat>();
        foreach (CardCat tCC in hand)
        {
            if (RatCat.S.ValidPlay(tCC))
            {
                validCards.Add(tCC);
            }
        }

        if (validCards.Count == 0)
        {
            cc = AddCard(RatCat.S.Draw());
            cc.callbackPlayer = this;
            return;
        }

        cc = validCards[Random.Range(0, validCards.Count)];
        RemoveCard(cc);
        RatCat.S.MoveToTarget(cc);
        cc.callbackPlayer = this;
    }

    public void CCCallback(CardCat tCC)
    {
        Utils.tr("Player.CBCallback()", tCC.name, "Player " + playerNum);
        RatCat.S.PassTurn();
    }
}
