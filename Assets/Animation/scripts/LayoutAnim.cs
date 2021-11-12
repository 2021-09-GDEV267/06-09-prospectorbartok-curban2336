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

    // Update is called once per frame
    void Update()
    {
        
    }
}
