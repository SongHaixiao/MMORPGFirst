using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class QuestManager
    {
        Character Owner;

        public QuestManager(Character owner)
        {
            this.Owner = owner;
        }

        // get quest info in db, and chang it to network data
        public void GetQuestInfos(List<NQuestInfo> list)
        {
            foreach(var quest in this.Owner.Data.Quests)
            {
                list.Add(GetQuestInfo(quest));
            }
        }

        // change ques data in db into network data
        public NQuestInfo GetQuestInfo(TCharacterQuest quest)
        {
            return new NQuestInfo()
            {
                QuestId = quest.QuestID,
                QuestGuid = quest.Id,
                Status =  (QuestStatus)quest.Status,
                Targets = new int[3]
                {  
                    quest.Target1,
                    quest.Target2,
                    quest.Target3,
                }
            };
        }

        // accept quest
        public Result AcceptQuest(NetConnection<NetSession> sender, int questId)
        {
            // get character who is accepting quest
            Character character = sender.Session.Character;

            // check quest is in db in order to avoid cheating from client

            QuestDefine quest;

            // quest is in db, get it and add it to character
            if(DataManager.Instance.Quests.TryGetValue(questId, out quest))
            {
                // get quest from db
                var dbquest = DBService.Instance.Entities.CharacterQuests.Create();
                dbquest.QuestID = quest.ID;
                if(quest.Target1 == QuestTarget.None)
                {
                    // no quest target, directly set the quest status finished
                    dbquest.Status = (int)QuestStatus.Completed;
                }
                else
                {
                    // have quest target, set the quest status in progress
                    dbquest.Status = (int)QuestStatus.InProgress;
                }

                // add quest created to session and character
                // and save db
                sender.Session.Response.questAccept.Quest = this.GetQuestInfo(dbquest);
                character.Data.Quests.Add(dbquest);
                DBService.Instance.Save();
            
                return Result.Success;
            }

            // quest is not in db, return failed
            else
            {
                sender.Session.Response.questAccept.Errormsg = "Quest is not exited!";
                return Result.Failed;
            }
        }

          // submit quest
        public Result SubmitQuest(NetConnection<NetSession> sender, int questId)
        {
            // get character who is submitting quest
            Character character = sender.Session.Character;

            // check quest is in db in order to avoid cheating from client

            QuestDefine quest;

            // quest is in db, get it and add it to character
            if(DataManager.Instance.Quests.TryGetValue(questId, out quest))
            {
                // get quest from character
                var dbquest = character.Data.Quests.Where(q => q.QuestID == questId).FirstOrDefault();

                if(dbquest != null)
                {
                    // quest is not completed
                    if(dbquest.Status != (int)QuestStatus.Completed)
                    {
                        // quest in not uncompleted
                        sender.Session.Response.questSubmit.Errormsg = "Quest in uncompleted !";
                        return Result.Failed;
                    }
 
                    // quest is completed, set its status as finished
                    dbquest.Status = (int)QuestStatus.Finished;

                    // save quest status in session and db
                    sender.Session.Response.questSubmit.Quest = this.GetQuestInfo(dbquest);
                    DBService.Instance.Save();

                    // process quest reward

                    // money
                    if(quest.RewardGold > 0)
                    {
                        character.Gold += quest.RewardGold;
                    }

                    // experience
                    if(quest.RewardExp > 0)
                    {
                        // character.Exp += quest RewardExp
                    }

                    // tool item 1
                    if(quest.RewardItem1 > 0)
                    {
                        character.ItemManger.AddItem(quest.RewardItem1, quest.RewardItem1Count);
                    }

                    // tool item 2
                    if(quest.RewardItem2 > 0)
                    {
                        character.ItemManger.AddItem(quest.RewardItem2, quest.RewardItem2Count);
                    }

                    // tool item3
                    if(quest.RewardItem3 > 0)
                    {
                        character.ItemManger.AddItem(quest.RewardItem3, quest.RewardItem3Count);
                    }

                    // save db and return success result
                    DBService.Instance.Save();        
                    return Result.Success;
                }
            
                sender.Session.Response.questAccept.Errormsg = "Quest is not exited![2]";
                return Result.Failed;
            }
            // quest is not in db, return failed
            else
            {
                sender.Session.Response.questAccept.Errormsg = "Quest is not exited![1]";
                return Result.Failed;
            }
        }
    }
}