using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;
using UnityEngine.Events;

using Entities;
using SkillBridge.Message;

namespace Managers
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        // unity event fro character enter game
        public UnityAction<Character> OnCharacterEnter;
        public UnityAction<Character> OnCharacterLeave;

        public CharacterManager()
        {

        }

        public void Dispose()
        {
        }

        public void Init()
        {

        }

        public void Clear()
        {
            this.Characters.Clear();
        }

        // add character object into game
        public void AddCharacter(SkillBridge.Message.NCharacterInfo cha)
        {
            Debug.LogFormat("AddCharacter:{0}:{1} Map:{2} Entity:{3}", cha.Id, cha.Name, cha.mapId, cha.Entity.String());
            Character character = new Character(cha);
            this.Characters[cha.Id] = character;
            EnityManager.Instance.AddEntity(character);
            if (this.OnCharacterEnter!=null)
            {
                this.OnCharacterEnter(character);
            }
        }


        // delete character object from game
        public void RemoveCharacter(int characterId)
        {
            Debug.LogFormat("RemoveCharacter:{0}", characterId);
            if(this.Characters.ContainsKey(characterId))
            {
                EnityManager.Instance.RemoveEntity(this.Characters[characterId].Info.Entity);
                if (this.OnCharacterLeave != null)
                {
                    this.OnCharacterLeave(this.Characters[characterId]);
                }
                this.Characters.Remove(characterId);
            }

        }
    }
}
