using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim : MonoBehaviour
{
    static public Anim S;
    static public Player CURRENT_PLAYER;

    [Header("Set in Inspector")]
    public TextAsset deckXML;
    public TextAsset layoutXML;
    public Vector3 layoutCenter = Vector3.zero;

    [Header("Set Dynamically")]
    public CardBartok targetCard;
    public TurnPhase phase = TurnPhase.idle;
    public List<GameObject> colorSquares;
    private LayoutAnim layout;
    private Transform layoutAnchor;

    void Awake()
    {
        S = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        layout = GetComponent<LayoutAnim>();
        layout.ReadLayout(layoutXML.text);

        colorSquares.Add(layout.target1);
        colorSquares.Add(layout.target2);
        colorSquares.Add(layout.target3);
        colorSquares.Add(layout.target4);

        LayoutGame();
    }

    void LayoutGame()
    {
        if (layoutAnchor == null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;
        }
    }

    public void DrawFirstTarget()
    {
        //CardBartok tCB = MoveToTarget(Draw());
        //tCB.reportFinishTo = this.gameObject;
    }

    public void CBCallback(CardBartok cb)
    {
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
            //int ndx = players.IndexOf(CURRENT_PLAYER);
            //num = (ndx + 1) % 4;
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
        //CURRENT_PLAYER = players[num];
        phase = TurnPhase.pre;
        CURRENT_PLAYER.TakeTurn();
    }

    public bool CheckGameOver()
    {
        int redCounter = 0;
        int greenCounter = 0;
        int blueCounter = 0;
        int yellowCounter = 0;
        foreach (GameObject obj in colorSquares)
        {
            if(obj.GetComponent<SpriteRenderer>().color == new Color(255, 0, 0))
            {
                redCounter++;
            }
            if (obj.GetComponent<SpriteRenderer>().color == new Color(0, 255, 0))
            {
                greenCounter++;
            }
            if (obj.GetComponent<SpriteRenderer>().color == new Color(0, 0, 255))
            {
                blueCounter++;
            }
            if (obj.GetComponent<SpriteRenderer>().color == new Color(255, 255, 0))
            {
                yellowCounter++;
            }
        }
        if (redCounter == 4 || greenCounter == 4 || blueCounter == 4 || yellowCounter == 4)
        {
            phase = TurnPhase.gameOver;
            return (true);
        }
        return (false);
    }

    public CardBartok MoveToTarget(CardBartok tCB)
    {
        tCB.timeStart = 0;
        //tCB.MoveTo(layout.discardPile.pos + Vector3.back);
        tCB.state = CBState.toTarget;
        tCB.faceUp = true;

        tCB.SetSortingLayerName("10");
        //tCB.eventualSortLayer = layout.target.layerName;

        targetCard = tCB;

        return (tCB);
    }
}
