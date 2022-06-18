using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestDialog : UIWindow
{
    /*Function : Control quest dialog ui to load data.*/

    // public components
    public UIQuestInfo questInfo;
    public Quest quest;
    public GameObject openButtons;
    public GameObject submitButtons;

    // Start is called before the first frame upda  te
    void Start()
    {
        
    }


    // set quest to update quest information
    // and switch accept and submit button based on quest status
    public void SetQuest(Quest quest)
    {
        // copy quest
        this.quest = quest;

        // update quest info
        this.UpdateQuest();

        // quest is acceptable
        // qust.Info : quest information from server
        // quest.Info is null : new quest
        // quest.Info.Status is Completed : quest is finished, can submit
        // else quest error
        if (this.quest.Info == null)
        {
            // active accept button
            openButtons.SetActive(true);

            // close submit button
            submitButtons.SetActive(false);
        }

        // quest is not acceptable
        else
        {
            // quest is completed
            if(this.quest.Info.Status == SkillBridge.Message.QuestStatus.Completed)
            {
                // close accept button
                openButtons.SetActive(false);

                // active submit button
                submitButtons.SetActive(true);
            }

            // quest occur problem
            else
            {
                // MessageBox.Show("Quest is not correct ！")；
                
                // close accept button
                openButtons.SetActive(false);

                // close submit button
                submitButtons.SetActive(false);
            }
        }

    }

    // update quest information
    private void UpdateQuest()
    {
        // quest and quest info are available,
        // set quest information
        if(this.quest != null)
        {
            if(this.questInfo != null)
            {
                this.questInfo.SetQuestInfo(this.quest);
            }
        }
    }

    public void AcceptReward()
    {
        string rewardMoney = questInfo.rewardMoney.ToString();
        int money = Convert.ToInt32(rewardMoney);
        User.Instance.AddGold(money);
    }
}

