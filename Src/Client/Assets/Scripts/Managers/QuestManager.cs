using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using SkillBridge.Message;
using System;
using System.Linq;
using Services;
using UnityEngine.Events;

namespace Managers
{
    public enum NpcQuestStatus
    {
        None = 0,   // don't have quest
        Completed,   // have completed and can submit quest
        Available,  // have acceptable quest
        Uncompleted, // have uncompleted quest 
    }
    public class QuestManager : Singleton<QuestManager>
    {
        /* Function ： manger quest object */

        // define attributes
        public List<NQuestInfo> questInfos;
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();
        public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();
        public UnityAction<Quest> OnQuestStatusChanged;
        //public UnityAction OnQuestChanged;


        // init QuestManger when start
        public void Init(List<NQuestInfo> quests)
        {
            this.questInfos = quests;   // copy quest lists from server
            allQuests.Clear();          // clear all quests manger in local
            this.npcQuests.Clear();     // clear npcQuest manger  in local
            InitQuests();               // init quests
        }

        // init  haved quests
        private void InitQuests()
        {
            // init already have accepted quests
            foreach (var info in this.questInfos)
            {
                // create quest model
                Quest quest = new Quest(info);

                // copy quest model to all quests store manger
                this.allQuests[quest.Info.QuestId] = quest;
            }


            // init can accept quests
            this.CheckAvailableQuests();

            foreach (var kv in this.allQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
        }

        // init can accept quests
        void CheckAvailableQuests()
        {
            // get available quest from local db
            foreach (var kv in DataManager.Instance.Quests)
            {
                // not match class
                if (kv.Value.LimitClass != CharacterClass.None && kv.Value.LimitClass != User.Instance.CurrentCharacterInfo.Class)
                    continue;

                // not match level
                if (kv.Value.LimitLevel > User.Instance.CurrentCharacterInfo.Level)
                    continue;

                // quest existed
                if (this.allQuests.ContainsKey(kv.Key))
                    continue;

                // need to do previous quest
                if (kv.Value.PreQuest > 0)
                {
                    // get previous quest
                    Quest preQuest;
                    if (this.allQuests.TryGetValue(kv.Value.PreQuest, out preQuest))
                    {
                        // previous quest is not accepted
                        if (preQuest.Info == null)
                            continue;

                        // previous quest not completed
                        if (preQuest.Info.Status != QuestStatus.Finished)
                            continue;
                    }

                    // previous quest is not accepted
                    else
                        continue;
                }

                // create quest model
                Quest quest = new Quest(kv.Value);

                // add it into quest manager
                this.allQuests[quest.Define.ID] = quest;
            }
        }

        // Core Method :
        void AddNpcQuest(int npcId, Quest quest)
        {
            if (!this.npcQuests.ContainsKey(npcId))
                this.npcQuests[npcId] = new Dictionary<NpcQuestStatus, List<Quest>>();

            // create 3 kinds of quest lists
            List<Quest> availables;
            List<Quest> completeds;
            List<Quest> uncompleteds;

            // init 3 kinds of quest lists
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Available, out availables))
            {
                availables = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Available] = availables;
            }

            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Completed, out completeds))
            {
                completeds = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Completed] = completeds;
            }

            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Uncompleted, out uncompleteds))
            {
                uncompleteds = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Uncompleted] = uncompleteds;
            }

            // add quest to 3 kinds of quests lists
            if (quest.Info == null)
            {
                // add quest to available quest list
                if (npcId == quest.Define.AcceptNPC && !this.npcQuests[npcId][NpcQuestStatus.Available].Contains(quest))
                {
                    this.npcQuests[npcId][NpcQuestStatus.Available].Add(quest);
                }
            }

            else
            {
                // add quest to completed quest list
                if (quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.Completed)
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Completed].Contains(quest))
                    {
                        this.npcQuests[npcId][NpcQuestStatus.Completed].Add(quest);
                    }
                }

                // add quest to uncompleted quest list
                if (quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.InProgress)
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Uncompleted].Contains(quest))
                    {
                        this.npcQuests[npcId][NpcQuestStatus.Uncompleted].Add(quest);
                    }
                }
            }
        }

        // get NPC quest status
        public NpcQuestStatus GetQuestStatusByNpc(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();

            // get quest npc
            if (this.npcQuests.TryGetValue(npcId, out status))
            {
                if (status[NpcQuestStatus.Completed].Count > 0)
                    return NpcQuestStatus.Completed;

                if (status[NpcQuestStatus.Available].Count > 0)
                    return NpcQuestStatus.Available;

                if (status[NpcQuestStatus.Uncompleted].Count > 0)
                    return NpcQuestStatus.Uncompleted;
            }

            return NpcQuestStatus.None;
        }

        public bool OpenNpcQuest(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuests.TryGetValue(npcId, out status))
            {
                if (status[NpcQuestStatus.Completed].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Completed].First());

                if (status[NpcQuestStatus.Available].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Available].First());

                if (status[NpcQuestStatus.Uncompleted].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Uncompleted].First());
            }

            return false;
        }

        // show quest dialog ui
        private bool ShowQuestDialog(Quest quest)
        {
            // quest dialog ui will be shoed just when accept and submit quest

            // quest is new or finished, create quest dialog ui
            if (quest.Info == null || quest.Info.Status == QuestStatus.Completed)
            {
                UIQuestDialog dlg = UIManager.Instance.Show<UIQuestDialog>();
                dlg.SetQuest(quest);
                dlg.OnClose += OnQuestDialogClose;
                return true;
            }

            // quest is not new or unfinished, don't create quest dialog ui
            if (quest.Info != null || quest.Info.Status == QuestStatus.Completed)
            {
                if (!string.IsNullOrEmpty(quest.Define.DialogIncomplete))
                    MessageBox.Show(quest.Define.DialogIncomplete);
            }
            return true;
        }

        // method to process quest dialog close
        void OnQuestDialogClose(UIWindow sender, UIWindow.WindowResult result)
        {
            UIQuestDialog dlg = (UIQuestDialog)sender;

            // quest dialog ui , click Yes
            if (result == UIWindow.WindowResult.Yes)
            {

                MessageBox.Show(dlg.quest.Define.DialogAccept);

                // quest accept dialog ui
                if (dlg.quest.Info == null)
                    QuestService.Instance.SendQuestAccept(dlg.quest);

                // quest submit dialog ui
                else if (dlg.quest.Info.Status == QuestStatus.Completed)
                    QuestService.Instance.SendQuestSubmit(dlg.quest);
            }

            // quest dialog ui , click No
            else if (result == UIWindow.WindowResult.No)
            {
                // sho quest deny dialog ui 
                MessageBox.Show(dlg.quest.Define.DialogDeny);
            }
        }

        // refresh quest status
        Quest RefreshQuestStatus(NQuestInfo quest)
        {
            // clear quest npc
            this.npcQuests.Clear();

            // sync client and server quest info are both same

            // create a quest model
            Quest result;

            // have accepted quests,
            // quest from server existed in all quests store manager
            if (this.allQuests.ContainsKey(quest.QuestId))
            {
                // update this quest status in local all quests store manager
                this.allQuests[quest.QuestId].Info = quest;

                // give it to quest model created
                result = this.allQuests[quest.QuestId];
            }

            // new quest,
            // not existed in all quests store manger
            else
            {
                // use quest from server to instance quest model created
                result = new Quest(quest);

                // add it to all quests store manger
                this.allQuests[quest.QuestId] = result;
            }

            // init can accept quests        
            CheckAvailableQuests();

            // add available quests to its npc
            foreach (var kv in this.allQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }

            // quest status inform
            if (OnQuestStatusChanged != null)
                OnQuestStatusChanged(result);

            return result;
        }
        public void OnQuestAccepted(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogAccept);
        }

        public void OnQuestSubmitted(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogFinish);
        }
    }
}
