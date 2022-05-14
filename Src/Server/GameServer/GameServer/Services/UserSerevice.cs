﻿using System;
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
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCreateCharacter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
        }

        public void Init()
        {

        }

        // processs register request from client,
        // then send response to client
        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userRegister = new UserRegisterResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                message.Response.userRegister.Result = Result.Failed;
                message.Response.userRegister.Errormsg = "User is existed！";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                message.Response.userRegister.Result = Result.Success;
                message.Response.userRegister.Errormsg = "None";
            }

            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        // processs login request from client,
        // then send response to client 
        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            // create login response net message data structure,
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userLogin = new UserLoginResponse();

            // get user info from db
            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();

            // give the login response net message information
            if (user == null)
            {
                // user is unavailable
                // give the failed information to net messae
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "User is not exited!";
            }
            else if (user.Password != request.Passward)
            {
                // password is not correct
                // give the failed information to net messae
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "Password is not correct!";
            }
            else
            {
                // user is available

                // store user infor from db to session
                sender.Session.User = user;

                // give the successed information to net messae
                message.Response.userLogin.Result = Result.Success;
                message.Response.userLogin.Errormsg = "None";
                message.Response.userLogin.Userinfo = new NUserInfo();
                message.Response.userLogin.Userinfo.Id = 1;
                message.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                message.Response.userLogin.Userinfo.Player.Id = user.Player.ID;

                // initialization for characters
                foreach (var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;
                    info.Name = c.Name;
                    info.Class = (CharacterClass)c.Class;
                    message.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }
            }

            // send data to client
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
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
                MapPosX = 5000,
                MapPosY = 4000,
                MapPosZ = 820,
            };

            // 2. add character data table into DB
            DBService.Instance.Entities.Characters.Add(character);
            // 3. update session
            sender.Session.User.Player.Characters.Add(character);
            // 4. save DB
            DBService.Instance.Entities.SaveChanges();

            // create character create response net message
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.createChar = new UserCreateCharacterResponse();
            message.Response.createChar.Result = Result.Success;
            message.Response.createChar.Errormsg = "None";

            // send character create response to clinet
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        // processs enter game request from client,
        // then send response to client 
        private void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            // when login successed, Session.User is gived value,
            // we can get character data from session
            TCharacter dbchar = sender.Session.User.Player.Characters.ElementAt(request.characterIdx);

            Log.InfoFormat("UserGameEnterRequest : characterID : {0} : {1} Map : {2}", dbchar.ID, dbchar.Name, dbchar.MapID);

            // Character Manager make character data into Character object
            Character character = CharacterManager.Instance.AddCharacter(dbchar);

            // create enter game response net message
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.gameEnter = new UserGameEnterResponse();
            message.Response.createChar.Result = Result.Success;
            message.Response.createChar.Errormsg = "None";

            // send enter game response to clinet
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
            sender.Session.Character = character;   // after enter game, give value to Character in Session
                                                    // then, we can know we are operating which character via Session.Character

            // add character into map
            MapManager.Instance[dbchar.MapID].CharacterEnter(sender, character);
        }

    }
}
