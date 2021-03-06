using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Elevens : MonoBehaviour {

	static public Elevens 	S;

	[Header("Set in Inspector")]
	public TextAsset			deckXML;
	public TextAsset            layoutXML;
	public float xOffset = 3;
	public float yOffset = -2.5f;
	public Vector3 layoutCenter;
	public Vector2 fsPosMid = new Vector2(0.5f, 0.90f);
	public Vector2 fsPosRun = new Vector2(0.5f, 0.75f);
	public Vector2 fsPosMid2 = new Vector2(0.4f, 1f);
	public Vector2 fsPosEnd = new Vector2(0.5f, 0.95f);
	public float reloadDelay = 2f;
	public Text gameOverText, roundResultText, highScoreText;


	[Header("Set Dynamically")]
	public Deck					deck;
	public Layout				layout;
	public List<CardProspector> drawPile;
	public List<CardProspector> stacker;
	public Transform layoutAnchor;
	public CardProspector target;
	public Vector3 table = new Vector3(0, 0, 0);
	public bool addUp = false;
	public List<Vector3> eleven;
	public List<CardProspector> tableau;
	public List<CardProspector> discardPile;
	public FloatingScore fsRun;
	public int goldCounter = 0;

	void Awake(){
		S = this;
		SetUpUITexts();
	}

	void SetUpUITexts()
    {
		GameObject go = GameObject.Find("HighScore");
		if(go != null)
        {
			highScoreText = go.GetComponent<Text>();
        }
		int highScore = ScoreManager.HIGH_SCORE;
		string hScore = "High Score: " + Utils.AddCommasToNumber(highScore);
		go.GetComponent<Text>().text = hScore;

		go = GameObject.Find("GameOver");
		if (go != null)
		{
			gameOverText = go.GetComponent<Text>();
		}

		go = GameObject.Find("RoundResult");
		if (go != null)
		{
			roundResultText = go.GetComponent<Text>();
		}

		ShowResultsUI(false);
	}

	void ShowResultsUI(bool show)
    {
		gameOverText.gameObject.SetActive(show);
		roundResultText.gameObject.SetActive(show);
    }

	void Start() {
		ScoreBoard.S.score = ScoreManager.SCORE;
		
		deck = GetComponent<Deck> ();
		deck.InitDeck (deckXML.text);
		Deck.Shuffle(ref deck.cards);

		layout = GetComponent<Layout>();
		layout.ReadLayout(layoutXML.text);
		drawPile = ConvertListCardsToListCardProspectors(deck.cards);
		layoutGame();
	}

	List<CardProspector> ConvertListCardsToListCardProspectors(List<Card> lCD)
    {
		List<CardProspector> lCP = new List<CardProspector>();
		CardProspector tCP;
		foreach(Card tCD in lCD)
        {
			tCP = tCD as CardProspector;
			lCP.Add(tCP);
        }
		return (lCP);
    }

	CardProspector Draw()
    {
		CardProspector cd = drawPile[0];
		drawPile.RemoveAt(0);
		return (cd);
    }

	void layoutGame()
    {
		if(layoutAnchor == null)
        {
			GameObject tGO = new GameObject("_LayoutAnchor");
			layoutAnchor = tGO.transform;
			layoutAnchor.transform.position = layoutCenter;
        }

		CardProspector cp;

		foreach(SlotDef tSD in layout.slotDefs)
        {
			cp = Draw();
			cp.faceUp = tSD.faceUp;
			cp.transform.parent = layoutAnchor;

			cp.transform.localPosition = new Vector3(layout.multiplier.x * tSD.x, layout.multiplier.y * tSD.y, -tSD.layerID);
			cp.layoutID = tSD.id;
			cp.slotDef = tSD;

			cp.state = eCardState.tableau;

			cp.SetSortingLayerName(tSD.layerName);

			tableau.Add(cp);
        }

		foreach(CardProspector tCP in tableau)
        {
			if (Random.value <= 0.1f)
			{
				tCP.back.GetComponent<SpriteRenderer>().sprite = deck.cardBackGold;
				tCP.GetComponent<SpriteRenderer>().sprite = deck.cardFrontGold;
				tCP.isGold = true;
			}

			foreach (int hid in tCP.slotDef.hiddenBy)
            {
				cp = FindCardByLayoutID(hid);
				tCP.hiddenBy.Add(cp);
            }
        }

		//MoveToTarget(Draw());

		UpdateDrawPile();
	}

	CardProspector FindCardByLayoutID(int layoutID)
    {
		foreach(CardProspector tCP in tableau)
        {
			if(tCP.layoutID == layoutID)
            {
				return (tCP);
            }
        }
		return (null);
    }

	void SetTableauFaces()
    {
		foreach (CardProspector cd in tableau)
        {
			bool faceUp = true;
			foreach(CardProspector cover in cd.hiddenBy)
            {
				if(cover.state == eCardState.tableau)
                {
					faceUp = false;
                }
            }
			cd.faceUp = faceUp;
        }

	}

	void MoveToDiscard(CardProspector cd)
    {
		cd.state = eCardState.discard;
		discardPile.Add(cd);
		cd.transform.parent = layoutAnchor;
		cd.transform.localPosition = new Vector3(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.layerID + 0.5f);
		cd.faceUp = true;

		cd.SetSortingLayerName(layout.discardPile.layerName);
		cd.SetSortOrder(-200 + (discardPile.Count*5));
    }

	void MoveToTarget(CardProspector cd)
	{
		if (target != null) MoveToDiscard(target);
		target = cd;
		
		cd.state = eCardState.discard;
		discardPile.Add(cd);
		cd.transform.parent = layoutAnchor;
		cd.transform.localPosition = new Vector3(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.layerID);
		cd.faceUp = true;

		cd.SetSortingLayerName(layout.discardPile.layerName);
		cd.SetSortOrder(0);
	}

	void MoveToTableau(CardProspector c0, Vector3 c1)
    {
		c0.transform.position = c1;
		c0.state = eCardState.tableau;
		tableau.Add(c0);
		UpdateDrawPile();
		ScoreManager.EVENT(eScoreEvent.draw, c0);
		FloatingScoreHandler(eScoreEvent.draw);

	}

	void UpdateDrawPile()
    {
		CardProspector cd;

		for(int i=0; i<drawPile.Count; i++)
        {
			cd = drawPile[i];
			cd.transform.parent = layoutAnchor;
			Vector2 dpStagger = layout.drawPile.stagger;

			cd.transform.localPosition = new Vector3(layout.multiplier.x * (layout.drawPile.x+i*dpStagger.x), layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y), -layout.drawPile.layerID+0.1f*i);

			cd.faceUp = false;
			cd.state = eCardState.drawpile;
			cd.SetSortingLayerName(layout.drawPile.layerName);
			cd.SetSortOrder(-10*i);
		}
    }

	public void CardClicked(CardProspector cd)
    {
		Vector3 newPos;
		GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");
		switch (cd.state)
        {
			case eCardState.target:
				break;
			//case eCardState.drawpile:
				//MoveToDiscard(target);
				//MoveToTarget(Draw());
				//UpdateDrawPile();
				//ScoreManager.EVENT(eScoreEvent.draw, cd);
				//FloatingScoreHandler(eScoreEvent.draw);
				//break;
			case eCardState.tableau:
				bool validMatch = true;
                if (!cd.faceUp)
                {
					validMatch = false;
                }
				if(addUp != true)
                {
					addUp = true;
					target = cd;
					newPos = cd.transform.position;
					//cd.Find("HaloAnchor").SetActive(true);
					cd.transform.position = newPos;
					if (cd.rank == 11)
                    {
						tableau.Remove(cd);
						MoveToTableau(Draw(), cd.transform.position);
						MoveToTarget(cd);
						SetTableauFaces();
						ScoreManager.EVENT(eScoreEvent.mine, cd);
						FloatingScoreHandler(eScoreEvent.mine);
						//cd.Find("HaloAnchor").SetActive(false);
						addUp = false;
						target = null;
						table = new Vector3(0, 0, 0);
						eleven.Clear();
						stacker.Clear();
						return;
                    }
					table = cd.transform.position;
					eleven.Add(table);
					stacker.Add(cd);
					return;
                }
                else if (!AddToEleven(cd,target))
                {
					validMatch = false;
					addUp = false;
                }
				if (!validMatch) return;

				if (cd.isGold == true)
                {
					goldCounter++;
                }

				table = cd.transform.position;
				eleven.Add(table);
				stacker.Add(cd);
				foreach (CardProspector card in stacker)
                {
					tableau.Remove(card);
					MoveToTarget(card);
                }
				foreach(Vector3 card in eleven)
                {
					MoveToTableau(Draw(), card);
                }
				//for (int i = 0; i < cards.Length; i++)
                //{
					//cards[i].parent.Find("HaloAnchor").SetActive(false);
				//}
				SetTableauFaces();
				ScoreManager.EVENT(eScoreEvent.mine, cd);
				FloatingScoreHandler(eScoreEvent.mine);
				target = null;
				table = new Vector3(0, 0, 0);
				eleven.Clear();
				stacker.Clear();
				addUp = false;
				break;
        }
		CheckForGameOver();
		

    }

	void CheckForGameOver()
    {
		if(tableau.Count == 0)
        {
			GameOver(true);
			return;
        }

        if (drawPile.Count > 0)
        {
			return;
        }

		GameOver(false);
    }

	void GameOver(bool won)
    {
		int score = ScoreManager.SCORE;
		if (fsRun != null) score += fsRun.score;
		if (won == true)
        {
			gameOverText.text = "Round Over";
			roundResultText.text = "You won this round!\nRoundScore: " + score;
			ShowResultsUI(true);

			//print("Game Over. You won");
			ScoreManager.EVENT(eScoreEvent.gameWin, null);
			FloatingScoreHandler(eScoreEvent.gameWin);
		}
        else
        {
			gameOverText.text = "Game Over";
			if(ScoreManager.HIGH_SCORE <= score)
            {
				string str = "You got the high score!\nHigh Score: " + score;
				roundResultText.text = str;
            }
            else
            {
				roundResultText.text = "Your final score was: " + score;
			}
			ShowResultsUI(true);
			//print("you lose");
			ScoreManager.EVENT(eScoreEvent.gameLoss, null);
			FloatingScoreHandler(eScoreEvent.gameLoss);
		}

		Invoke("ReloadLevel", reloadDelay);
    }

	void ReloadLevel()
    {
		SceneManager.LoadScene("__Prospector_Scene_0");
	}

	public bool AddToEleven(CardProspector c0, CardProspector c1)
    {
		if (!c0.faceUp || !c1.faceUp) return (false);

		if(Mathf.Abs(c0.rank + c1.rank) == 11)
        {
			return (true);
        }
		if (Mathf.Abs(c0.rank - c1.rank) == 11)
		{
			return (true);
		}
		if (Mathf.Abs(c1.rank - c0.rank) == 11)
		{
			return (true);
		}
		if (c0.rank == 11) return (true);

		return (false);
	}

	void FloatingScoreHandler(eScoreEvent evt)
    {
		List<Vector2> fsPts;
        switch (evt)
        {
			case eScoreEvent.draw:
			case eScoreEvent.gameWin:
			case eScoreEvent.gameLoss:
				if(fsRun != null)
                {
					fsPts = new List<Vector2>();
					fsPts.Add(fsPosRun);
					fsPts.Add(fsPosMid2);
					fsPts.Add(fsPosEnd);
					int modifiedTotal = fsRun.score * (int)Mathf.Pow(2, goldCounter);
					fsRun.score = modifiedTotal;
					fsRun.reportFinishTo = ScoreBoard.S.gameObject;
					fsRun.Init(fsPts, 0, 1);
					fsRun.fontSizes = new List<float>(new float[] { 28, 36, 4 });
					fsRun = null;
					goldCounter = 0;
				}
				break;
			case eScoreEvent.mine:
				FloatingScore fs;
				Vector2 p0 = Input.mousePosition;
				p0.x /= Screen.width;
				p0.y /= Screen.height;
				fsPts = new List<Vector2>();
				fsPts.Add(p0);
				fsPts.Add(fsPosMid);
				fsPts.Add(fsPosRun);
				fs = ScoreBoard.S.CreateFloatingScore(ScoreManager.CHAIN, fsPts);
				fs.fontSizes = new List<float>(new float[] { 4, 50, 28 });
				if(fsRun == null)
                {
					fsRun = fs;
					fsRun.reportFinishTo = null;
                }
                else
                {
					fs.reportFinishTo = fsRun.gameObject;
                }
				break;
		}
    }
}
