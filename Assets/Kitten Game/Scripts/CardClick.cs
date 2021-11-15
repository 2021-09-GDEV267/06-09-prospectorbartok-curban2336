using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardClick : MonoBehaviour
{
    public string designator;
    
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
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
