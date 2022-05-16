using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;

using SkillBridge.Message;
using Models;

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
		public UnityEngine.Events.UnityAction<Result, string> OnLogin; // login event
        public UnityEngine.Events.UnityAction<Result, string> OnRegister; // register event
        public UnityEngine.Events.UnityAction<Result, string> OnCharacterCreate; // character create event

        NetMessage pendingMessage = null;
        bool connected = false;

        public UserService()
        {
            /* events sended from server are accepted in there */

            // add listener for events in follow form server
            NetClient.Instance.OnConnect += OnGameServerConnect;
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;
			
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Subscribe<UserGameEnterResponse>(this.OnGameEnter);
            MessageDistributer.Instance.Subscribe<UserGameLeaveResponse>(this.OnGameLeave);
            //MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnCharacterEnter);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Unsubscribe<UserGameEnterResponse>(this.OnGameEnter);
            MessageDistributer.Instance.Unsubscribe<UserGameLeaveResponse>(this.OnGameLeave);
            //MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnCharacterEnter);

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
                if (this.pendingMessage.Request.userLogin!=null)
                {
                    if (this.OnLogin != null)
                    {
                        this.OnLogin(Result.Failed, string.Format("Server is Disconnected！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                else if(this.pendingMessage.Request.userRegister!=null)
                {
                    if (this.OnRegister != null)
                    {
                        this.OnRegister(Result.Failed, string.Format("Server is Disconnected！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                else
                {
                    if (this.OnCharacterCreate != null)
                    {
                        this.OnCharacterCreate(Result.Failed, string.Format("Server is Disconnected！\n RESULT:{0} ERROR:{1}", result, reason));
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

        // client send character creation request information to server
        public void SendCharacterCreate(string charName, CharacterClass cls)
        {
            Debug.LogFormat("SendCharacterCreate : charName : {0} class : {1}", charName, cls);

            // create character create net message
            // and fill its information
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.createChar = new UserCreateCharacterRequest();
            message.Request.createChar.Name = charName;
            message.Request.createChar.Class = cls;

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

        // client accept the character create response information from server
        void OnUserCreateCharacter(object sender, UserCreateCharacterResponse response)
        {
            Debug.LogFormat("OnUserCreateCharacter:{0} [{1}]", response.Result, response.Errormsg);

            // the response result is success
            if(response.Result == Result.Success)
            {
                // firsly clear player's characters existed,then add character from server
                Models.User.Instance.Info.Player.Characters.Clear();
                Models.User.Instance.Info.Player.Characters.AddRange(response.Characters);
            }

            // character create event is existed
            if (this.OnCharacterCreate != null) 
            {
                //  informate ui layer （UICharacterSelect.cs) to do character create operation
                this.OnCharacterCreate(response.Result, response.Errormsg);
            }
        }

        // client send enter game request information to server
        public void SendGameEnter(int characterIdx)
        {
            Debug.LogFormat("UserGameEnterRequest::characterId :{0}", characterIdx);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameEnter = new UserGameEnterRequest();
            message.Request.gameEnter.characterIdx = characterIdx;
            NetClient.Instance.SendMessage(message);
        }

        // client accept the character enter game response information from server
        void OnGameEnter(object sender, UserGameEnterResponse response)
        {
            Debug.LogFormat("OnGameEnter : {0} [{1}]", response.Result, response.Errormsg);
            
            if(response.Result == Result.Success)
            {

            }
        }

        // client accept the character enter map response information from server
        //private void OnCharacterEnter(object sender, MapCharacterEnterResponse message)
        //{
        //    Debug.LogFormat("OnMapCharacterEnter : {0} ", message.mapId);

        //    // get character entering map's data from serevr
        //    NCharacterInfo info = message.Characters[0];

        //    // store info to CurrentCharacter in User
        //    User.Instance.CurrentCharacter = info;

        //    // loading map where character will enter to
        //    SceneManager.Instance.LoadScene(DataManager.Instance.Maps[message.mapId].Resource);
        //}

        // client send character game leave request information to server
        public void SendGameLeave()
        {
            Debug.Log("UserGameLeaveRequest");

            // create character game leave request net message
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameLeave = new UserGameLeaveRequest();

            // send to server
            NetClient.Instance.SendMessage(message);
        }

        void OnGameLeave(object sender, UserGameLeaveResponse response)
        {
            Debug.LogFormat("OnGameLeave:{0} [{1}]", response.Result, response.Errormsg);
        }
    }
}
