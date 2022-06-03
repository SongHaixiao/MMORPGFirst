using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services
{
    class QuestService : Singleton<QuestService>, IDisposable
    {

        /* Function : 
            1. Send Client network request for accepting and submitting quest to  server.
            2. Listen and  accept to process the network response for accepting and submitting quest from server. */

        public QuestService()
        {
            // add listener 
            MessageDistributer.Instance.Subscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Subscribe<QuestSubmitResponse>(this.OnQuestSubmit);
        }

        public void Dispose()
        {
            // delete listener
            MessageDistributer.Instance.Unsubscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Unsubscribe<QuestSubmitResponse>(this.OnQuestSubmit);
        }

        // client send quest accept network request message to server
        public bool SendQuestAccept(Quest quest)
        {
            Debug.Log("SendQuestAccept!");

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questAccept = new QuestAcceptRequest();
            message.Request.questAccept.QuestId = quest.Define.ID;

            NetClient.Instance.SendMessage(message);
            return true;
        }

        // process quest accept response network message from server 
        private void OnQuestAccept(object sender, QuestAcceptResponse message)
        {
            Debug.LogFormat("OnQuestAccept : {0}, ERR : {1}", message.Result, message.Errormsg);

            if(message.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestAccepted(message.Quest);
            }
            else
            {
                MessageBox.Show("Quest Accept Failed", "Error !!!", MessageBoxType.Error);
            }
        }

        // client send quest submit network request message to server
        public bool SendQuestSubmit(Quest quest)
        {
            Debug.Log("SendQuestAccept!");

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questSubmit = new QuestSubmitRequest();
            message.Request.questSubmit.QuestId = quest.Define.ID;

            NetClient.Instance.SendMessage(message);
            return true;
        }

        // process quest submit response network message from server
        private void OnQuestSubmit(object sender, QuestSubmitResponse message)
        {
            Debug.LogFormat("OnQuestSubmit : {0}, ERR : {1}", message.Result, message.Errormsg);

            if(message.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestSubmitted(message.Quest);
            }
            else
            {
                MessageBox.Show("Quest Submit Failed!!!", "Error!!!", MessageBoxType.Error);
            }
        }

    }
}