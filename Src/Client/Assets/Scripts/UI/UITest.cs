using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : UIWindow
{
    /* Function : UI Test Class succeed from UI Window Class.*/

    public Text title;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // update ui window's title
    public void SetTitle(string title)
    {
        this.title.text = title;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
