using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TabView : MonoBehaviour 
{
    /*Function : manage the arrange of the bag UI */

    // open components
    public TabButton[] tabButtons;  // item button in bag
    public GameObject[] tabPages;   // bag page
                                    // note : the number of tabButtons
                                    // and tabPages is usually same

    public UnityAction<int> OnTabSelect;

    //public UnityAction<int> OnTabSelect;

    public int index = -1;  // current bag page  start from 0


    // Use this for initialization
    IEnumerator Start () {
        
        // travels every item buttons, and set its info
        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].tabView = this;   // set tab button's owner
            tabButtons[i].tabIndex = i;     // add index to set its page number
        }
        // wait 1 frame
        yield return new WaitForEndOfFrame();

        // add it into first page
        SelectTab(0);
    }


    // select bag page tab
    public void SelectTab(int index)
    {
        // current bag page is not the selecting page,
        // change to selecting page
        if (this.index != index)
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].Select(i == index);
                if (i < tabPages.Length - 1)
                    tabPages[i].SetActive(i == index);
            }
            if (OnTabSelect != null)
                OnTabSelect(index);
        }
    }

    void Update()
    {
        
    }
}
