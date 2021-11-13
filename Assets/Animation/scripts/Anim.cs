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
    public List<CardBartok> clones;
    public List<int> counters;
    public int redCounter = 0;
    public int greenCounter = 0;
    public int blueCounter = 0;
    public int yellowCounter = 0;
    private int colorchange1 = 0;
    private int colorchange2 = 0;
    private int colorchange3 = 0;
    private int colorchange4 = 0;
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

        counters.Add(colorchange1);
        counters.Add(colorchange2);
        counters.Add(colorchange3);
        counters.Add(colorchange4);

        LayoutGame();
    }

    public void ArrangeTargets()
    {
        CardBartok tCB;

        for (int i = 0; i < colorSquares.Count; i++)
        {
            tCB = colorSquares[i].GetComponent<CardBartok>();
            tCB.transform.SetParent(layoutAnchor);
            tCB.transform.localPosition = layout.slots[i].pos;
            tCB.SetSortingLayerName(layout.slots[i].layerName);
            tCB.SetSortOrder(-i * 4);
            tCB.state = CBState.target;
        }
    }

    void LayoutGame()
    {
        if (layoutAnchor == null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;
        }

        ArrangeTargets();

        Invoke("MoveFirstTarget", 2);
    }

    public void MoveFirstTarget()
    {
        int i = Random.Range(0, 4);
        int r = Random.Range(0, 4);

        GameObject tGO = GameObject.Instantiate(colorSquares[i]);
        tGO.GetComponent<SpriteRenderer>().color = layout.SetColor(tGO, layout.colorRange[r]);
        counters[i]++;
        CardBartok tCB = MoveToTarget(tGO.GetComponent<CardBartok>(), i);
        clones.Add(tCB);
        tCB.reportFinishTo = this.gameObject;
    }

    public void CBCallback(CardBartok cb)
    {
        StartAnim();
    }

    public void StartAnim()
    {
        PassTarget();
    }

    public void PassTarget()
    {
        int i = Random.Range(0, 4);
        int r = Random.Range(0, 4);

        if (CheckGameOver())
        {
            return;
        }
        else
        {
            GameObject tGO = GameObject.Instantiate(colorSquares[i]);
            tGO.GetComponent<SpriteRenderer>().color = layout.SetColor(tGO, layout.colorRange[r]);
            counters[i]++;
            CardBartok tCB = MoveToTarget(tGO.GetComponent<CardBartok>(), i);
            colorSquares[i].GetComponent<SpriteRenderer>().color = tGO.GetComponent<SpriteRenderer>().color;
            clones.Add(tCB);
            tCB.reportFinishTo = this.gameObject;
        }
    }

    void Update()
    {
        CheckGameOver();
        
        for (int i = 0; i < clones.Count; i++)
        {
            if (clones.Count == 0)
            {
                return;
            }
            else if (clones[i].state == CBState.target || clones[i].state == CBState.idle)
            {
                Destroy(clones[i].gameObject, 0.6f);
                clones.RemoveAt(i);
            }
        }
    }

    public bool CheckGameOver()
    {
        redCounter = 0;
        greenCounter = 0;
        blueCounter = 0;
        yellowCounter = 0;
        foreach (GameObject obj in colorSquares)
        {
            if(obj.GetComponent<SpriteRenderer>().color == new Color(255, 0, 0,255))
            {
                redCounter += 1;
            }
            if (obj.GetComponent<SpriteRenderer>().color == new Color(0, 255, 0,255))
            {
                greenCounter += 1;
            }
            if (obj.GetComponent<SpriteRenderer>().color == new Color(0, 0, 255,255))
            {
                blueCounter += 1;
            }
            if (obj.GetComponent<SpriteRenderer>().color == new Color(255, 255, 0,255))
            {
                yellowCounter += 1;
            }
        }
        if (redCounter == 4 || greenCounter == 4 || blueCounter == 4 || yellowCounter == 4)
        {
            phase = TurnPhase.gameOver;
            return (true);
        }
        return (false);
    }

    public CardBartok MoveToTarget(CardBartok tCB, int exception)
    {
        int i = Random.Range(0, 3);
        while(i == exception)
        {
            i = Random.Range(0, 3);
        }

        tCB.timeStart = 0;
        tCB.MoveTo(layout.slots[i].pos + Vector3.back);
        tCB.state = CBState.toTarget;

        tCB.SetSortingLayerName("10");
        tCB.eventualSortLayer = "11";

        targetCard = tCB;

        return (tCB);
    }
}
