using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestItem : ListView.ListViewItem
{
    /* Functions : control quest item object in ui quest. */
    
    // public components
    public Text title;
    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;
    public Quest quest;

    // when quest item in ui quest is selected,
    // change its background iamge
    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //  set quest information
    public void SetQuestInfo(Quest item)
    {
        this.quest = item;
        if (this.title != null) this.title.text = this.quest.Define.Name;
    }
}
