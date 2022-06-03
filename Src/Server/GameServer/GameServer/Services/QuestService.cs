using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class QuestService : Singleton<QuestService>
    {

        /* Function : 
            1. Listen and  accept to process the network request message for accepting and submitting quest from Client.
            1. Return Server network response message for accepting and submitting quest to client. */
            

        public QuestService()
        {
            // add listener 
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestAcceptRequest>(this.OnQuestAccept);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestSubmitRequest>(this.OnQuestSubmit);
        }

        public void Init()
        {

        }

        // process quest accept request network message from client
        void OnQuestAccept(NetConnection<NetSession> sender, QuestAcceptRequest request)
        {
            // get character info in session from client
            Character character = sender.Session.Character;
            Log.InfoFormat("QuestAcceptRequest : character : [{0}], QuestId : [{1}]", character.Id, request.QuestId);

            // create a quest accept response network message
            sender.Session.Response.questAccept = new QuestAcceptResponse();

            // fill info to quest accept response network message
            Result result = character.QuestManager.AcceptQuest(sender, request.QuestId);
            sender.Session.Response.questAccept.Result = result;

            // return quest accept response message to client
            sender.SendResponse();
        }

        // process quest submit request network message from client
        void OnQuestSubmit(NetConnection<NetSession> sender, QuestSubmitRequest request)
        {
           // get character info in session from client
            Character character = sender.Session.Character;
            Log.InfoFormat("QuestAcceptRequest : character : [{0}], QuestId : [{1}]", character.Id, request.QuestId);

            // create a quest submit response network message
            sender.Session.Response.questSubmit = new QuestSubmitResponse();

            // fill info to quest submit response network message
            Result result = character.QuestManager.SubmitQuest(sender, request.QuestId);
            sender.Session.Response.questSubmit.Result = result;

            // return quest submit response message to client
            sender.SendResponse();
        }

    }
}