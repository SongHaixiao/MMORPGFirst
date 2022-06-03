using Common.Data;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestSystem : UIWindow
{
    /* Function : control ui quest's behaviour. */

    // public components
    
    public GameObject itemPrefab;

    public TabView Tabs;
    
    public ListView listMain;
    public ListView listBranch;
    
    public UIQuestInfo questInfo;

    // private attributes
    private bool showAvailableList = false;

    // Start is called before the first frame update
    void Start()
    {
        // when start, add event for these method
        this.listMain.onItemSelected += this.OnQuestSelected;   // when quest is seleceted
        this.listBranch.onItemSelected += this.OnQuestSelected; // when quest is selected
        this.Tabs.OnTabSelect += OnSelectTab;                   // when tab is selected
        RefreshUI();

        //QuestManager.Instance.OnQuestChanged += RefreshUI;
    }

    // when tab is selected,
    // tab change function
    void OnSelectTab(int idx)
    {
        // whether to show  available quests
        // idx == 1 -> true， means show available quest
        // else means not show avaiable quests
        showAvailableList = idx == 1;
        RefreshUI();
    }

    private void OnDestroy()
    {
        //QuestManager.Instance.OnQuestChanged -= RefreshUI;
    }

    // refresh ui
    private void RefreshUI()
    {
        ClearAllQuestList();
        InitAllQuestItems();
    }

    // init all quest items
    private void InitAllQuestItems()
    {
        // travel all quests in Quest Manager, add it to quest ui
        foreach (var kv in QuestManager.Instance.allQuests)
        {
            // available quest :  1. accepted -> skip
            //                    2. acceptabl, but no quest info -> skip 
            //                    3. acceptable, have quest info , but not accept -> add it to quest ui

            if (showAvailableList)
            {
                // quest accepted, skip
                if (kv.Value.Info != null)
                    continue;
            }
            else
            {
                // quest acceptable, but no quest info, skip
                if (kv.Value.Info == null)
                    continue;
            }

            // create a UIQuestItem prefab with its parent note ( listMain or listBranch )  
            GameObject go = Instantiate(itemPrefab, kv.Value.Define.Type == QuestType.Main ? this.listMain.transform : this.listBranch.transform);

            // get UIQuestItem prefab
            UIQuestItem ui = go.GetComponent<UIQuestItem>();

            // set quest info for geeted UIQuestItem,
            // it means to record quest
            ui.SetQuestInfo(kv.Value);

            // add it to Mian quest
            if (kv.Value.Define.Type == QuestType.Main)
                this.listMain.AddItem(ui);

            // add it to Branch quest
            else
                this.listBranch.AddItem(ui);
        }
    }

    // clear all quests in Mian and Branch List
    private void ClearAllQuestList()
    {
        this.listMain.RemoveAll();
        this.listBranch.RemoveAll();
    }

    // set quest information to quest ui panel when quest is selected
    public void OnQuestSelected(ListView.ListViewItem item)
    {
        UIQuestItem questItem = item as UIQuestItem;
        this.questInfo.SetQuestInfo(questItem.quest);
    }
}
