using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElement : MonoBehaviour 
{
    // open compoent and attribution for unity
    public Transform owner;


    public float height = 1.5f; // set height attribution

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        // character owner object is available
        if (owner != null)
        {
            // update its height
            this.transform.position = owner.position + Vector3.up * height;
        }
    }
}
