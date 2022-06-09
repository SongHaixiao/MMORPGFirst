using Common.Data;
using Managers;
using Models;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkill : UIWindow
{
    public Text descript;
    public GameObject itemPrefab;
    public ListView listMain;
    public UISkillItem selectedItem;

    // Use this for initialization
    void Start()
    {
        RefreshUI();
        this.listMain.onItemSelected += this.OnItemSelected;
    }

    private void OnDestroy()
    {

    }

    public void OnItemSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UISkillItem;
        this.descript.text = this.selectedItem.item.Define.Description;
    }

    void RefreshUI()
    {
        ClearItems();
        InitItems();
    }

    // init all skill list
    void InitItems()
    {
        var Skills = User.Instance.CurrentCharacter.SkillMgr.Skills;
        foreach (var skill in Skills)
        {
            if (skill.Define.Type == Common.Battle.SkillType.Skill)
            {
                GameObject go = Instantiate(itemPrefab, this.listMain.transform);
                UISkillItem ui = go.GetComponent<UISkillItem>();
                ui.SetItem(skill, this, false);
                this.listMain.AddItem(ui);
            }
        }
    }

    void ClearItems()
    {
        this.listMain.RemoveAll();
    }
}