using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour 
{
    /* Function : control the arrange of the item button in bage UI.*/

    // attributions

    public Sprite activeImage;  // active state image
    private Sprite normalImage; // normal state image

    public TabView tabView;     // TabView component

    public int tabIndex = 0;    // tab page number
    public bool selected = false;   // flag to recommend wheter is selected

    private Image tabImage;         // button imaeg

	// Use this for initialization
	void Start () {
        // get image in current button 
        tabImage = this.GetComponent<Image>();

        // store normal state image
        normalImage = tabImage.sprite;

        // add click linstenre to this button in bag
        this.GetComponent<Button>().onClick.AddListener(OnClick);
	}

    // active item image when item is selected
    public void Select(bool select)
    {
        tabImage.overrideSprite = select ? activeImage : normalImage;
    }

    // change the bag page when click item button in bag
    void OnClick()
    {
        this.tabView.SelectTab(this.tabIndex);
    }

}
