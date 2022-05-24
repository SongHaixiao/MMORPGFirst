using Network;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Services
{
    class ItemService : Singleton<ItemService>,IDisposable
    {
        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);
        }

        public void SendBuyItem(int shopId, int shopItemId)
        {
            Debug.Log("SendBuyItem!");

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemBuy = new ItemBuyRequest();
            message.Request.itemBuy.shopId = shopId;
            message.Request.itemBuy.shopItemId = shopItemId;

            NetClient.Instance.SendMessage(message);
        }

        private void OnItemBuy(object sender, ItemBuyResponse message)
        {
            MessageBox.Show("Purchase Result : " + message.Result + "\n" + message.Errormsg, " Purchase Completed !");
        }
    }
}
