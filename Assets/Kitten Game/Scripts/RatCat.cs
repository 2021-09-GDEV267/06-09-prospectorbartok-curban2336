using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TurnPhaseCat
{
    idle,
    pre,
    waiting,
    post,
    gameOver
}

public class RatCat : MonoBehaviour
{
    static public RatCat S;
    static public CatPlayer CURRENT_PLAYER;

    [Header("Set in Inspector")]
    public TextAsset deckXML;
    public TextAsset layoutXML;
    public Vector3 layoutCenter = Vector3.zero;
    public float handFanDegrees = 10f;
    public int numStartingCards = 7;
    public float drawTimeStagger = 0.1f;

    [Header("Set Dynamically")]
    public Deck deck;
    public List<CardCat> drawpile;
    public List<CardCat> discardpile;
    public List<CatPlayer> players;
    public CardCat targetCard;
    public TurnPhaseCat phase = TurnPhaseCat.idle;

    private CatLayout layout;
    private Transform layoutAnchor;

    void Awake()
    {
        S = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        deck = GetComponent<Deck>();
        deck.InitDeck(deckXML.text);
        Deck.Shuffle(ref deck.cards);
        deck.cards = deck.cards as List<Card>;
        Debug.Log(deck.cards[0]);

        layout = GetComponent<CatLayout>();
        layout.ReadLayout(layoutXML.text);

        drawpile = UpgradeCardsList(deck.cards);
        LayoutGame();
    }

    public void ArrangeDrawPile()
    {
        CardCat tCC;

        for (int i = 0; i < drawpile.Count; i++)
        {
            tCC = drawpile[i];
            tCC.transform.SetParent(layoutAnchor);
            tCC.transform.localPosition = layout.drawPile.pos;
            tCC.faceUp = false;
            tCC.SetSortingLayerName(layout.drawPile.layerName);
            tCC.SetSortOrder(-i * 4);
            tCC.state = CCState.drawpile;
        }
    }

    void LayoutGame()
    {
        if (layoutAnchor == null)
        {
            GameObject tGO = new GameObject("LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;
        }

        ArrangeDrawPile();

        CatPlayer pl;
        players = new List<CatPlayer>();
        foreach (SlotDefCat tSD in layout.slotDefs)
        {
            pl = new CatPlayer();
            pl.handSlotDef = tSD;
            players.Add(pl);
            pl.playerNum = tSD.player;
        }
        players[0].type = PlayerTypeCat.human;

        CardCat tCC;

        for (int i = 0; i < numStartingCards; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                tCC = Draw();

                tCC.timeStart = Time.time + drawTimeStagger * (i * 4 + j);

                players[(j + 1) % 4].AddCard(tCC);
            }

            Invoke("DrawFirstTarget", drawTimeStagger * (numStartingCards * 4 + 4));
        }
    }

    public void DrawFirstTarget()
    {
        CardCat tCC = MoveToTarget(Draw());
        tCC.reportFinishTo = this.gameObject;
    }

    public void CCCallback(CardCat cc)
    {
        Utils.tr("Rat-a-tat-cat:CCCallback", cc.name);
        StartGame();
    }

    public void StartGame()
    {
        PassTurn(1);
    }

    public void PassTurn(int num = 1)
    {
        if (num == 1)
        {
            int ndx = players.IndexOf(CURRENT_PLAYER);
            num = (ndx + 1) % 4;
        }
        int lastPlayerNum = -1;
        if (CURRENT_PLAYER != null)
        {
            lastPlayerNum = CURRENT_PLAYER.playerNum;
            if (CheckGameOver())
            {
                return;
            }
        }
        CURRENT_PLAYER = players[num];
        phase = TurnPhaseCat.pre;
        CURRENT_PLAYER.TakeTurn();
        Utils.tr("Rat-a-tat-cat:PassTurn()", "Old: " + lastPlayerNum, "New: " + CURRENT_PLAYER.playerNum);
    }

    public bool CheckGameOver()
    {
        if (drawpile.Count == 0)
        {
            List<Card> cards = new List<Card>();
            foreach (CardCat cc in discardpile)
            {
                cards.Add(cc);
            }
            discardpile.Clear();
            Deck.Shuffle(ref cards);
            drawpile = UpgradeCardsList(cards);
            ArrangeDrawPile();
        }

        if (CURRENT_PLAYER.hand.Count == 0)
        {
            phase = TurnPhaseCat.gameOver;
            Invoke("RestartGame", 1);
            return (true);
        }
        return (false);
    }

    public void RestartGame()
    {
        CURRENT_PLAYER = null;
        SceneManager.LoadScene("Rat-a-tat-cat");
    }

    public bool ValidPlay(CardCat cc)
    {
        if (cc.rank == targetCard.rank) return (true);

        if (cc.suit == targetCard.suit)
        {
            return (true);
        }
        return (false);
    }

    public CardCat MoveToTarget(CardCat tCC)
    {
        tCC.timeStart = 0;
        tCC.MoveTo(layout.discardPile.pos + Vector3.back);
        tCC.state = CCState.toTarget;
        tCC.faceUp = true;

        tCC.SetSortingLayerName("10");
        tCC.eventualSortLayer = layout.target.layerName;
        if (targetCard != null)
        {
            MoveToDiscard(targetCard);
        }

        targetCard = tCC;

        return (tCC);
    }

    public CardCat MoveToDiscard(CardCat tCC)
    {
        tCC.state = CCState.discard;
        discardpile.Add(tCC);
        tCC.SetSortingLayerName(layout.discardPile.layerName);
        tCC.SetSortOrder(discardpile.Count * 4);
        tCC.transform.localPosition = layout.discardPile.pos + Vector3.back / 2;

        return (tCC);
    }

    public CardCat Draw()
    {
        CardCat cc = drawpile[0];
        if (drawpile.Count == 0)
        {
            int ndx;
            while (discardpile.Count > 0)
            {
                ndx = Random.Range(0, discardpile.Count);
                drawpile.Add(discardpile[ndx]);
                discardpile.RemoveAt(ndx);
            }
            ArrangeDrawPile();

            float t = Time.time;
            foreach (CardCat tCC in drawpile)
            {
                tCC.transform.localPosition = layout.discardPile.pos;
                tCC.callbackPlayer = null;
                tCC.MoveTo(layout.drawPile.pos);
                tCC.timeStart = t;
                t += 0.02f;
                tCC.state = CCState.toDrawpile;
                tCC.eventualSortLayer = "0";
            }
        }
        drawpile.RemoveAt(0);
        return (cc);
    }

    public void CardClicked(CardCat tCC)
    {
        if (CURRENT_PLAYER.type != PlayerTypeCat.human) return;
        if (phase == TurnPhaseCat.waiting) return;

        switch (tCC.state)
        {
            case CCState.drawpile:
                CardCat cc = CURRENT_PLAYER.AddCard(Draw());
                cc.callbackPlayer = CURRENT_PLAYER;
                Utils.tr("Rat-a-tat-cat:CardClicked()", "Draw", cc.name);
                phase = TurnPhaseCat.waiting;
                break;
            case CCState.hand:
                if (ValidPlay(tCC))
                {
                    CURRENT_PLAYER.RemoveCard(tCC);
                    MoveToTarget(tCC);
                    tCC.callbackPlayer = CURRENT_PLAYER;
                    Utils.tr("Rat-a-tat-cat:CardClicked()", "Play", tCC.name, targetCard.name + " is target");
                    phase = TurnPhaseCat.waiting;
                }
                else
                {
                    Utils.tr("Rat-a-tat-cat:CardClicked()", "Attempted to Play", tCC.name, targetCard.name + " is target");
                }
                break;
        }
    }

    List<CardCat> UpgradeCardsList(List<Card> lCD)
    {
        List<CardCat> lCC = new List<CardCat>();
        foreach (Card tCD in lCD)
        {
            lCC.Add(tCD as CardCat);
        }
        Debug.Log(lCC[0]);
        return (lCC);
    }
}
