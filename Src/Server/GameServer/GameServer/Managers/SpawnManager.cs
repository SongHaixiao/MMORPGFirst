using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Models;

namespace GameServer.Managers
{
    class SpawnManager
    {

        // list to store Spawner as spawn rule
        private List<Spawner> Rulse = new List<Spawner>();

        private Map Map;

        // spawn rules init
        // 1. load current map's all spawn rules from db
        // 2. create every spawn rule as a Spawner
        public void Init(Map map)
        {
            this.Map = map;

            if(DataManager.Instance.SpawnRules.ContainsKey(Map.Define.ID))
            {
                foreach(var define in DataManager.Instance.SpawnRules[Map.Define.ID].Values)
                {
                    this.Rulse.Add(new Spawner(define, this.Map));
                }
            }
        }

         // update spawn rules
        public void Update()
        {
            // no spawn rule, return
            if(Rulse.Count == 0)
                return;

            // exist spawn rules, update every spawn rule
            for(int i = 0; i < this.Rulse.Count;i++)
            {
                this.Rulse[i].Update();
            }
        }
    }
}