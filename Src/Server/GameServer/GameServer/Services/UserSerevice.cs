using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class UserService : Singleton<UserService>
    {

        public UserService()
        {
            // linsted the events form cleint
		    MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCreateCharacter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(this.OnGameLeave);
        }

        public void Init()
        {

        }
		
		// processs login request from client,
        // then send response to client 
        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            // create login response net message data structure,
            sender.Session.Response.userLogin = new UserLoginResponse();

            // get user info from db
            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();

            // give the login response net message information
            if (user == null)
            {
                // user is unavailable
                // give the failed information to net messae
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "User is not exited!";
            }
            else if (user.Password != request.Passward)
            {
                // password is not correct
                // give the failed information to net messae
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "Password is not correct!";
            }
            else
            {
                // user is available

                // store user infor from db to session
                sender.Session.User = user;

                // give the successed information to net messae
                sender.Session.Response.userLogin.Result = Result.Success;
                sender.Session.Response.userLogin.Errormsg = "None";
                sender.Session.Response.userLogin.Userinfo = new NUserInfo();
                sender.Session.Response.userLogin.Userinfo.Id = (int)user.ID;
                sender.Session.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                sender.Session.Response.userLogin.Userinfo.Player.Id = user.Player.ID;

                // initialization for characters
                foreach (var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;
                    info.Name = c.Name;
                    info.Type = CharacterType.Player;
                    info.Class = (CharacterClass)c.Class;
                    info.Tid = c.ID;
                    sender.Session.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }
            }

            // send data to client
            sender.SendResponse();
        }

        // processs register request from client,
        // then send response to client
        void OnRegister(NetConnection<NetSession> conn, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            conn.Session.Response.userRegister = new UserRegisterResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                conn.Session.Response.userRegister.Result = Result.Failed;
                conn.Session.Response.userRegister.Errormsg = "User is existed！";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                conn.Session.Response.userRegister.Result = Result.Success;
                conn.Session.Response.userRegister.Errormsg = "None";
            }

           conn.SendResponse();
        }


        // processs character create request from client,
        // then send response to client 
        private void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("UserCreateCharacterRequest: Name:{0}  Class:{1}", request.Name, request.Class);

            // data operation

            // 1. create a character data table
            TCharacter character = new TCharacter()
            {
                Name = request.Name,
                Class = (int)request.Class,
                TID = (int)request.Class,
                MapID = 1,
                MapPosX = 5000, // start position x
                MapPosY = 4000, // start position y
                MapPosZ = 820,  // start position z
                Gold = 3000, //  start money
                Equips = new byte[20]
            };

            // add bag data table to character
            var bag = new TCharacterBag();
            bag.Owner = character;
            bag.Items = new byte[0];
            bag.Unlocked = 20;
            character.Bag = DBService.Instance.Entities.CharacterBag.Add(bag);

            // afeter character is creted, get character data from db
            character = DBService.Instance.Entities.Characters.Add(character);

            // add begining items after character is created
            character.Items.Add(new TCharacterItem()
            {
                Owner = character,
                ItemID = 1,
                ItemCount = 20,
            });

            character.Items.Add(new TCharacterItem()
            {
                Owner = character,
                ItemID = 2,
                ItemCount = 20,

            });

            character.Items.Add(new TCharacterItem()
            {
                Owner = character,
                ItemID = 1001,
                ItemCount = 1,
            });

            // save creaed character data into session and DB
            sender.Session.User.Player.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();

            // create character create response net message
            sender.Session.Response.createChar = new UserCreateCharacterResponse();
            sender.Session.Response.createChar.Result = Result.Success;
            sender.Session.Response.createChar.Errormsg = "None";

            // add every character in session to new message
            foreach(var c in sender.Session.User.Player.Characters)
            {
                NCharacterInfo info = new NCharacterInfo();
                info.Id = 0;
                info.Name = c.Name;
                info.Type = CharacterType.Player;
                info.Class = (CharacterClass)c.Class;
                info.Tid = c.ID;
                sender.Session.Response.createChar.Characters.Add(info);
            }

            // send character create response to clinet
            sender.SendResponse();
        }

        // processs enter game request from client,
        // then send response to client 
        void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            // when login successed, Session.User is gived value,
            // we can get character data from session
            TCharacter dbchar = sender.Session.User.Player.Characters.ElementAt(request.characterIdx);
            Log.InfoFormat("UserGameEnterRequest: characterID:{0}:{1} Map:{2}", dbchar.ID, dbchar.Name, dbchar.MapID);
            Character character = CharacterManager.Instance.AddCharacter(dbchar);

            // create enter game response net message 
            sender.Session.Response.gameEnter = new UserGameEnterResponse();
            sender.Session.Response.gameEnter.Result = Result.Success;
            sender.Session.Response.gameEnter.Errormsg = "None";

            // enter game successed, send character information
            // add charter infomation ( including Tool info)
            // to game enter response message
            sender.Session.Response.gameEnter.Character = character.Info;

            // send enter game response to clinet
            sender.SendResponse();

            sender.Session.Character = character;   // after enter game, give value to Character in Session
                                                    // then, we can know we are operating which character via Session.Character
            // add character into map
            MapManager.Instance[dbchar.MapID].CharacterEnter(sender, character);
        }

        // processs character leave game request from client,
        // then send response to client 
        void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            // get the character from session from client
            Character character = sender.Session.Character;

            Log.InfoFormat("UserGameLeaveRequest : characterID : {0} : {1} Map : {2}", character.Id, character.Info.Name, character.Info.mapId);

            // remove character
            CharacterLeave(character);

            // create character leave game response message to client
            sender.Session.Response.gameLeave = new UserGameLeaveResponse();
            sender.Session.Response.gameLeave.Result = Result.Success;
            sender.Session.Response.gameLeave.Errormsg = "None";

            // send character leave game response message to client
            sender.SendResponse();

        }

        // remove  Chracter 
        public void CharacterLeave(Character character)
        {
            // remove Character from CharacterManager
            CharacterManager.Instance.RemoveCharacter(character.Id);

            // remove Character from MapManager
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
        }
    }
}
