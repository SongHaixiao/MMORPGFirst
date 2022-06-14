using Entities;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElement : MonoBehaviour 
{
    // open compoent and attribution for unity
    public Transform owner;
    public UINameBar uiNameBar;

    public float height = 1.8f; // set height attribution

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        // character owner object is available
        if (owner != null)
        {
            this.height = uiNameBar.Height;

            // update its height
            this.transform.position = owner.position + Vector3.up * this.height;
        }

        // main camera is avaiable
        if(Camera.main != null)
        {
            // make UINameBar's toward direction is Main Camera in order to
            // let player see the UINameBar is frontal
            this.transform.forward = Camera.main.transform.forward;
        }
    }
}
