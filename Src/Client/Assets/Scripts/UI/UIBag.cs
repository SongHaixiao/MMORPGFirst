
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIBag : UIWindow
{
    // summary 1:30
    public Text moeny;
    public Transform[] pages;
    public GameObject bagItem;

    List<Image> slots;

    // User this for initialization
    void Start()
    {
        if(slots==null)
        {
            slots = new List<Image>();
            for(int page = 0; page < pages.Length; page++)
            {
               List<Image> imgs = new List<Image>();
               imgs.AddRange(this.pages[page].GetComponentsInChildren<Image>(true));

               foreach(var img in imgs)
                {
                    if(img.CompareTag("BagItem"))
                    {
                        slots.Add(img);
                    }
                }
            }
        }

        StartCoroutine(InitBags());
    }

    IEnumerator InitBags()
    {
        for(int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            if (item.ItemId > 0) 
            {
                GameObject go = Instantiate(bagItem, slots[i].transform);
                var ui = go.GetComponent<UIBagItem>();
                var def = ItemManger.Instance.Items[item.ItemId].Define;
                ui.SetMainIcon(def.Icon, item.Count.ToString());
                ui.gameObject.SetActive(true);
                
            }
        }

        for(int i = BagManager.Instance.Items.Length; i < slots.Count;i++)
        {
            slots[i].color = Color.gray;
        }

        this.moeny.text = User.Instance.CurrentCharacterInfo.Gold.ToString();

        yield return null;
    }

    // bag resetting
    public void OnReset()
    {
        BagManager.Instance.Reset();
        this.Clear();
        StartCoroutine(InitBags());
    }

    // destroy bag item 
    private void Clear()
    {
        for (int i = 0; i < slots.Count; i++) 
        {
            if (slots[i].transform.childCount > 0)
            {
                Destroy(slots[i].transform.GetChild(0).gameObject);
            }
        }
    }
}