using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> 
{
    // open components
    public GameObject UINameBar;
    //public GameObject UIMiniMap;

    // dicionary to manager world element ui
    private Dictionary<Transform, GameObject> ElementsNameBar = new Dictionary<Transform, GameObject>();

    // add UINameBar for character object when it is created
    public void AddCharacterNameBar(Transform owner, Character character) 
    {
        GameObject goNameBar = Instantiate(UINameBar, this.transform); // instance UINameBar to this.transform positon with goNameBar
        goNameBar.name = "NameBar" + character.entityId;               // gieve goNameBar name's value
        goNameBar.GetComponent<UIWorldElement>().owner = owner;        // set goNameBar's UIWorldElement component  owner
        goNameBar.GetComponent<UINameBar>().character = character;     // set goNameBar's UINameBar component character object
        goNameBar.SetActive(true);                                     // active goNameBar
        this.ElementsNameBar[owner] = goNameBar;                       // add instanced goNameBar into elements

        //this.UIMiniMap.GetComponent<UIMinimap>().PlayerTransform = owner; // add UINameBar for character object when it is created
    }

    // remove UINameBar from character object when it is dead
    public void RemoveCharacterNameBar(Transform owner)
    {

        // if UINameBar of owner exieted in elemnts
        if (this.ElementsNameBar.ContainsKey(owner))
        {
            Destroy(this.ElementsNameBar[owner]);  // destroy UINameBar from owner
            this.ElementsNameBar.Remove(owner);    // remove UINameBar from owner
        }
    }
}
