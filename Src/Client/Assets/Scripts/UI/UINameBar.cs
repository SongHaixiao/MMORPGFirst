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


    // Use this for initialization
    void Start () {
		if(this.character!=null)
        {
            
        }
	}
	
	// Update is called once per frame
	void Update () {

        // update ui name bar per frame
        this.UpadteUINameBar();

        // make UINameBar's toward direction is Main Camera in order to
        // let player see the UINameBar is frontal
        this.transform.forward = Camera.main.transform.forward;
	}

    // update UINambeBar method
    void UpadteUINameBar()
    {
        // character object is available
        if (this.character != null)
        {
            // get character Name + Level   
            string name = this.character.Name + " Level." + this.character.Info.Level;

            // load character name to avatar name
            if(name != this.NameNameBar.text)
            {
                this.NameNameBar.text = name;
            }
        }
    }
}
