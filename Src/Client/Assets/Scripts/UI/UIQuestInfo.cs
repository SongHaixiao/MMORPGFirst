using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestInfo : MonoBehaviour
{
    /* Function : Set quest info.*/

    // public components
    public Text title;
    public Text[] targets;
    public Text description;
    public Text overview;

    public UIIconItem rewardItems;

    public Text rewardMoney;
    public Text rewardExp;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // set quest information
    public void SetQuestInfo(Quest quest)
    {
        // set quest title
        this.title.text = string.Format("[{0}]   {1}", quest.Define.Type, quest.Define.Name);

        if (this.overview != null) this.overview.text = quest.Define.Overview;

        if(this.description != null)
        {
            // quest info is null , set quest description
            if (quest.Info == null)
            {
                this.description.text = quest.Define.Dialog;
            }

            // quest info is not null , and quest is not completed, set quest finished description
            else
            {
                if (quest.Info.Status == SkillBridge.Message.QuestStatus.Finished)
                {
                    this.description.text = quest.Define.DialogFinish;
                }
            }
        }

        
        // TODO : add quest targets

        // TODO : add quest reward items

        // set reward money
        this.rewardMoney.text = quest.Define.RewardGold.ToString();

        // set reward text
        this.rewardExp.text = quest.Define.RewardExp.ToString();

        // auto layout to avoid not refreshing when content changeds
        foreach(var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    // accept button
    public void OnClickAccpet()
    {

    }

    // abandon button
    public void OnClickAbandon()
    {

    }
}
