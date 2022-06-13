using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINameBar : MonoBehaviour 
{

    // open components
    public Text NameNameBar;
    public Character character;
    public UIBuffIcons buffIcons;


    // Use this for initialization
    void Start () {
		if(this.character!=null)
        {
            buffIcons.SetOwner(this.character);
        }
	}
	
	// Update is called once per frame
	void Update () {

        // update ui name bar per frame
        this.UpadteUINameBar();
	}

    // update UINambeBar method
    void UpadteUINameBar()
    {
        // character object is available
        if (this.character != null)
        {
            // get character Name + Level   
            string name = this.character.Name + " Lv." + this.character.Info.Level;

            // load character name to avatar name
            if(name != this.NameNameBar.text)
            {
                this.NameNameBar.text = name;
            }
        }
    }
}
