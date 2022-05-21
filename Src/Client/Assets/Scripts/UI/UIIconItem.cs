using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIIconItem : MonoBehaviour
{
    /* Function : UI of tool item icon */

    // open components
    public Image mainIamge;
    public Image secondIamge;
    public Text mainText;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // set main image icon for itme
    public void SetMainIcon(string iconName, string text)
    {
        // loading icon mian iamge and override it
        this.mainIamge.overrideSprite = Resloader.Load<Sprite>(iconName);

        // set the text
        this.mainText.text = text;
    }

}
