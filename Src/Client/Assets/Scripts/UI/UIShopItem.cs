using Common.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour, ISelectHandler
{
    // open components
    public Image icon;
    public Text title;
    public Text price;
    public Text limitClass;

    public Text count;
    public Image background;
    public Image shopItemBackground;
    public Sprite normalBg;
    public Sprite selectedBg;

    // define attributions
    private bool selected;
    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            this.background.overrideSprite = selected ? selectedBg : normalBg;
            this.shopItemBackground.overrideSprite = selected ? selectedBg : normalBg;
        }
    }

    public int ShopItemID { get; set; }
    private UIShop shop;
    private ItemDefine item;
    private ShopItemDefine ShopItem { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetShopItem(int id, ShopItemDefine shopItem, UIShop owner)
    {
        this.shop = owner;
        this.ShopItemID = id;
        this.ShopItem = shopItem;
        this.item = DataManager.Instance.Items[this.ShopItem.ItemID];

        this.title.text = this.item.Name;
        this.count.text = "x" + ShopItem.Count.ToString();
        this.price.text = ShopItem.Price.ToString();
        if (this.item.LimitClass.ToString() != "None") 
            this.limitClass.text = this.item.LimitClass.ToString();
        this.icon.overrideSprite = Resloader.Load<Sprite>(item.Icon);
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.LogFormat("Shop Item is Selected !");

        this.Selected = true;
        this.shop.SelectShopItem(this);
    }
}