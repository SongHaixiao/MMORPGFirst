using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class MonsterManager
    {
        private Map Map; // monster belongs to map

        public Dictionary<int, Monster> Monsters = new Dictionary<int, Monster>(); // conrol monster object via dictionary data structure 

        public void Init(Map map)
        {
            this.Map = map;
        }

        // create monster
        internal Monster Create(int spawnMonID, int spawnLevel, NVector3 position, NVector3 direction)
        {
            // construct monster model
            Monster monster = new Monster(spawnMonID, spawnLevel, position, direction);
            EntityManager.Instance.AddEntity(this.Map.ID, monster);
            
            // add monster info to model
            monster.Info.Id = monster.entityId;
            monster.Info.mapId = this.Map.ID;

            // add monster model created to Monster store dictionary
            Monsters[monster.Id] = monster;


            // map make monster created enter map
            this.Map.MonsterEnter(monster);

            return monster;
        }
    }
}