using Common.Data;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : UIWindow
{
    // open components
    public Text title;
    public Text money;
    public GameObject shopItem;
    public Transform[] itemRoot;

    ShopDefine shop;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitItems());
    }

    IEnumerator InitItems()
    {
        foreach(var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if(kv.Value.Status > 0)
            {
                GameObject go = Instantiate(shopItem, itemRoot[0]);
                UIShopItem ui = go.GetComponent<UIShopItem>();
                ui.SetShopItem(kv.Key, kv.Value, this);
            }
        }

        yield return null;
    }

    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        this.title.text = shop.Name;
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    private UIShopItem selectedItem;
    public void SelectShopItem(UIShopItem item)
    {
        if (selectedItem != null)
            selectedItem.Selected = false;
        selectedItem = item;
    }

    public void OnClickBuy()
    {
        if(this.selectedItem == null)
        {
            MessageBox.Show("Please tool item to buy !", "Purchase Tip");
            return;
        }

        if (!ShopManager.Instance.BuyItem(this.shop.ID, this.selectedItem.ShopItemID))
        {
            Debug.LogFormat("But Item Error !");
        }
    }
}
