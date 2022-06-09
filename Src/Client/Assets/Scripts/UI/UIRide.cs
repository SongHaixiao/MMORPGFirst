using Entities;
using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRide : UIWindow
{
    // public components
    public Text description;
    public GameObject itemPrefab;
    public ListView listMain;
    public UIRideItem selectedItem;

    // Start is called before the first frame update
    void Start()
    {
        RefreshUI();
        this.listMain.onItemSelected += this.OnItemSelected;
    }

    private void OnDestroy()
    {
        this.listMain.onItemSelected -= this.OnItemSelected;
    }

    public void OnItemSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIRideItem;
        this.description.text = this.selectedItem.item.Define.Description;
    }

    void RefreshUI()
    {
        ClearItems();
        InitItems();
    }

    private void InitItems()
    {
        foreach (var kv in ItemManger.Instance.Items)
        {
            if (kv.Value.Define.Type == ItemType.Ride &&
                (kv.Value.Define.LimitClass == CharacterClass.None || kv.Value.Define.LimitClass == User.Instance.CurrentCharacterInfo.Class))
            {
                if (EquipManager.Instance.Contains(kv.Key))
                    continue;

                GameObject go = Instantiate(itemPrefab, this.listMain.transform);
                UIRideItem ui = go.GetComponent<UIRideItem>();
                ui.SetEquipItem(kv.Value, this, false);
                this.listMain.AddItem(ui);
            }
        }
    }

    public void ClearItems()
    {
        this.listMain.RemoveAll();
    }

    public void DoRide()
    {
        if(this.selectedItem == null)
        {
            MessageBox.Show("Selected rider to ride", "Tips");
            return;
        }

        User.Instance.Ride(this.selectedItem.item.Id);
    }
}
