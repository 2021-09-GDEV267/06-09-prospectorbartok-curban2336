using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slot
{
    public float x;
    public float y;
    public string layerName = "Default";
    public string startingColor;
    public int layerID = 0;
    public int id;
    public List<int> hiddenBy = new List<int>();
    public float rot;
    public string type = "slot";
    public Vector2 stagger;
    public int player;
    public Vector3 pos;
}

public class LayoutAnim : MonoBehaviour
{
    public List<string> colorRange;
    public List<Slot> slots;
    public GameObject target1;
    public GameObject target2;
    public GameObject target3;
    public GameObject target4;
    SpriteRenderer sprite;

    [Header("Set Dynamically")]
    public PT_XMLReader xmlr;
    public PT_XMLHashtable xml;
    public Vector2 multiplier;

    void Awake()
    {
        colorRange = new List<string>();
        colorRange.Add("red");
        colorRange.Add("green");
        colorRange.Add("blue");
        colorRange.Add("yellow");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        SetColor();
    }

    public void SetColor()
    {
        sprite = target1.GetComponent<SpriteRenderer>();

        sprite.color = new Color(255, 0, 0, 255);

        sprite = target2.GetComponent<SpriteRenderer>();

        sprite.color = new Color(0, 255, 0, 255);

        sprite = target3.GetComponent<SpriteRenderer>();

        sprite.color = new Color(0, 0, 255, 255);

        sprite = target4.GetComponent<SpriteRenderer>();

        sprite.color = new Color(255, 255, 0, 255);
    }

    public void SetColor(GameObject obj, string str)
    {
        if (str == "red")
        {
            sprite = obj.GetComponent<SpriteRenderer>();

            sprite.color = new Color(255, 0, 0, 255);
        }
        else if (str == "green")
        {
            sprite = obj.GetComponent<SpriteRenderer>();

            sprite.color = new Color(0, 255, 0, 255);
        }
        else if (str == "blue")
        {
            sprite = obj.GetComponent<SpriteRenderer>();

            sprite.color = new Color(0, 0, 255, 255);
        }
        else if (str == "yellow")
        {
            sprite = obj.GetComponent<SpriteRenderer>();

            sprite.color = new Color(255, 255, 0, 255);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReadLayout(string xmlText)
    {
        xmlr = new PT_XMLReader();
        xmlr.Parse(xmlText);
        xml = xmlr.xml["xml"][0];

        multiplier.x = float.Parse(xml["multiplier"][0].att("x"));
        multiplier.y = float.Parse(xml["multiplier"][0].att("y"));

        Slot tSD;

        PT_XMLHashList slotsX = xml["slot"];

        for (int i = 0; i < slotsX.Count; i++)
        {
            tSD = new Slot();
            if (slotsX[i].HasAtt("type"))
            {
                tSD.type = slotsX[i].att("type");
            }
            else
            {
                tSD.type = "slot";
            }

            tSD.x = float.Parse(slotsX[i].att("x"));
            tSD.y = float.Parse(slotsX[i].att("y"));
            tSD.pos = new Vector3(tSD.x * multiplier.x, tSD.y * multiplier.y, 0);
            tSD.layerID = int.Parse(slotsX[i].att("layer"));
            tSD.startingColor = colorRange[Random.Range(0, 3)];

            tSD.layerName = tSD.layerID.ToString();

            slots.Add(tSD);
        }
    }
}
