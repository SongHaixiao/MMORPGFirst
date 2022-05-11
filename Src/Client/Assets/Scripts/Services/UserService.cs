using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;

using SkillBridge.Message;

namespace Services
{
    class UserService : Singleton<UserService>, IDisposable
    {
        /* Function : 
         *  Accept the net message sended from server */

        /* Unity Event : 
         * 
         *  - it can be call in other class, and then do operation
         *  
         *  - Logic Layer via Unity Event tell UI Layer to do operation
         * 
         */
        public UnityEngine.Events.UnityAction<Result, string> OnRegister; // register event
        public UnityEngine.Events.UnityAction<Result, string> OnLogin; // login event

        NetMessage pendingMessage = null;
        bool connected = false;

        public UserService()
        {
            /* events sended from server are accepted in there */

            // add listener for events in follow form server
            NetClient.Instance.OnConnect += OnGameServerConnect;
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogin);

        }

        public void Dispose()
        {
            // remove listener for added in UserService
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogin);
            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
        }

        public void Init()
        {

        }

        // client connect to server
        public void ConnectToServer()
        {
            Debug.Log("ConnectToServer() Start ");
            //NetClient.Instance.CryptKey = this.SessionId;
            NetClient.Instance.Init("127.0.0.1", 8000);
            NetClient.Instance.Connect();
        }


        // network connection event
        void OnGameServerConnect(int result, string reason)
        {
            Log.InfoFormat("LoadingMesager::OnGameServerConnect :{0} reason:{1}", result, reason);

            // connected
            if (NetClient.Instance.Connected)
            {
                this.connected = true;
                if(this.pendingMessage!=null)
                {
                    NetClient.Instance.SendMessage(this.pendingMessage);
                    this.pendingMessage = null;
                }
            }

            // disconnected
            else
            {
                if (!this.DisconnectNotify(result, reason))
                {
                    MessageBox.Show(string.Format("Network Error! Can't connect to server ! \n RESULT:{0} ERROR:{1}", result, reason), "Error!!!", MessageBoxType.Error);
                }
            }
        }

        // network disconnection event
        public void OnGameServerDisconnect(int result, string reason)
        {
            this.DisconnectNotify(result, reason);
            return;
        }

        // network is disconnected
        bool DisconnectNotify(int result,string reason)
        {
            if (this.pendingMessage != null)
            {
                if (this.pendingMessage.Request.userRegister!=null)
                {
                    if (this.OnRegister != null)
                    {
                        this.OnRegister(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                return true;
            }
            return false;
        }

        // client send request information to server
        public void SendRegister(string user, string psw)
        {
            Debug.LogFormat("UserRegisterRequest::user :{0} psw:{1}", user, psw);

            // create register net message
            // and fill its information
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userRegister = new UserRegisterRequest();
            message.Request.userRegister.User = user;
            message.Request.userRegister.Passward = psw;

            // network is connected, send register net messagee
            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }

            // network is disconnected, try to connect to server
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        // client accept the register response information from server
        void OnUserRegister(object sender, UserRegisterResponse response)
        {
            Debug.LogFormat("OnUserRegister:{0} [{1}]", response.Result, response.Errormsg);

            // register unity event is not null
            if (this.OnRegister != null)
            {
                // call OnRegister() method to do register operation
                this.OnRegister(response.Result, response.Errormsg);
            }
        }

        // client send login request information to server
        public void SendLogin(string user, string psw)
        {
            Debug.LogFormat("UserLoginRequest : user : {0} psw : {1}", user, psw);

            // create register net message
            // and fill its information
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userLogin = new UserLoginRequest();
            message.Request.userLogin.User = user;
            message.Request.userLogin.Passward = psw;

            // network is connected, send register net messagee
            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }

            // network is disconnected, try to connect to server
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        // client accept the login response information from server
        void OnUserLogin(object sender, UserLoginResponse response)
        {
            Debug.LogFormat("OnLogin:{0} [{1}]", response.Result, response.Errormsg);

            // login successed
            if (response.Result == Result.Success)
            {
                // loading charater models
                Models.User.Instance.SetupUserInfo(response.Userinfo);
            }

            // loing event is available,
            // call OnLogin() method in UI layer to 
            // do login operation
            if(this.OnLogin != null)
            {
                this.OnLogin(response.Result, response.Errormsg);
            }
        }
    }
}
