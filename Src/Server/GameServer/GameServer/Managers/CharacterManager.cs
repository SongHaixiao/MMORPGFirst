using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
using GameServer.Entities;

namespace GameServer.Managers
{
    class CharacterManager : Singleton<CharacterManager>
    {
        // Dictionary Data Structure : find operation is more effective vid key
        
        // Character Dictionary to store Charcter object created 
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        public CharacterManager()
        {
        }

        public void Dispose()
        {
        }

        public void Init()
        {

        }

        // Character Manager Common Method : Clear()、Add()、Remove（）

        // crear all character objects in Character
        public void Clear()
        {
            this.Characters.Clear();
        }

        // change character data table to Character Object
        // and add Character Object into Characters Dictionary
        public Character AddCharacter(TCharacter cha)
        {
            Character character = new Character(CharacterType.Player, cha);
            EntityManager.Instance.AddEntity(cha.MapID, character);
            this.Characters[character.Id] = character;
            return character;
        }


        // delete Character Object from Characters Dictionary
        public void RemoveCharacter(int characterId)
        {
            var cha = this.Characters[characterId];
            EntityManager.Instance.RemoveEntity(cha.Data.MapID, cha);
            this.Characters.Remove(characterId);
        }
    }
}
