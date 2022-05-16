using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> 
{
    // open component
    public GameObject UINameBar;

    private Dictionary<Transform, GameObject> elements = new Dictionary<Transform, GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // add UINameBar for character object when it is created
    public void AddCharacterNameBar(Transform owner, Character character) 
    {
        GameObject goNameBar = Instantiate(UINameBar, this.transform); // instance UINameBar to this.transform positon with goNameBar
        goNameBar.name = "NameBar" + character.entityId;               // gieve goNameBar name's value
        goNameBar.GetComponent<UIWorldElement>().owner = owner;        // set goNameBar's UIWorldElement component  owner
        goNameBar.GetComponent<UINameBar>().character = character;     // set goNameBar's UINameBar component character object
        goNameBar.SetActive(true);                                     // active goNameBar
        this.elements[owner] = goNameBar;                              // add instanced goNameBar into elements
    }

    // remove UINameBar from character object when it is dead
    public void RemoveCharacterNameBar(Transform owner)
    {

        // if UINameBar of owner exieted in elemnts
        if (this.elements.ContainsKey(owner))
        {
            Destroy(this.elements[owner]);  // destroy UINameBar from owner
            this.elements.Remove(owner);    // remove UINameBar from owner
        }
    }
}
