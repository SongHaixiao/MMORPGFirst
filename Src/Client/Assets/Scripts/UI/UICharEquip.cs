using Entities;
using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharEquip : UIWindow
{
    // public components

    // ui char euqip
    public Text title;
    public Text money;

    // equip panel
    public GameObject itemPrefab;

    // view panel
    public GameObject itemEquipedPrefab;
    public Transform itemListRoot;
    public List<Transform> slots;

    // attribute panel
    public Text chaTitel;
    //public Text hpValue;
    //public Text mpValue;
    //public Text 


    // Start is called before the first frame update
    void Start()
    {
        RefreshUI();
        EquipManager.Instance.OnEquipChanged += RefreshUI;
    }

    private void OnDestroy()
    {
        EquipManager.Instance.OnEquipChanged -= RefreshUI;
    }

    void RefreshUI()
    {
        ClearAllEquipList();
        InitAllEquipItems();
        ClearEquipedList();
        InitEquipeItems();
        InitEquipedItems();

        InitAttributePanel();
        InitBottomPanel();
    }


    /// <summary>
    /// Init all equips list
    /// </summary>
    private void InitEquipeItems()
    {
       foreach(var kv in ItemManger.Instance.Items)
        {
            if(kv.Value.Define.Type == ItemType.Equip && kv.Value.Define.LimitClass == User.Instance.CurrentCharacter.Class)
            {
                // equipment which is equiped will not show
                if (EquipManager.Instance.Contains(kv.Key))
                    continue;

                GameObject go = Instantiate(itemPrefab, itemListRoot);
                UIEquipItem ui = go.GetComponent<UIEquipItem>();
                ui.SetEquipItem(kv.Key, kv.Value, this, false);
            }
        }
    }

    private void ClearEquipedList()
    {
        foreach(var item in itemListRoot.GetComponentsInChildren<UIEquipItem>())
        {

            Destroy(item.gameObject);
        }
    }

    private void InitAllEquipItems()
    {
        foreach(var item in slots)
        {
            if (item.childCount > 0)
            {
                Destroy(item.GetChild(0).gameObject);
            }
        }
    }

    private void ClearAllEquipList()
    {
        foreach(var item in slots)
        {
            if (item.childCount > 0)
                Destroy(item.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// init equipments which is equiped
    /// </summary>
    void InitEquipedItems()
    {
        for(int i = 0 ; i < (int)EquipSlot.SlotMax; i++)
        {
            var item = EquipManager.Instance.Equips[i];
            {
                if (item != null) 
                {
                    GameObject go = Instantiate(itemEquipedPrefab, slots[i]);
                    UIEquipItem ui = go.GetComponent<UIEquipItem>();
                    ui.SetEquipItem(i, item, this, true);
                }
            }
        }
    }

    public void DoEquip(Item item)
    {
        EquipManager.Instance.EquipItem(item);
    }

    public void UnEquip(Item item)
    {
        EquipManager.Instance.UnEquipItem(item);
    }

    // init character attributes
    void InitAttributePanel()
    {
        var character = User.Instance.CurrentCharacter;
        if(character == null)
        {
            Debug.LogError("Loading character attributes error ! Character is not existed ! ");
            return;
        }

        string name = character.Name.ToString();
        string level = "Lv. " + character.Level.ToString();
        this.chaTitel.text = name + "   "  +  level;
    }

    // init money dat in bottom panel
    void InitBottomPanel()
    {
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }
}
