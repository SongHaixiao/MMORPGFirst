using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Data;
using GameServer.Models;

namespace GameServer.Managers
{
    class Spawner
    {
        // spawn rule
        public SpawnRuleDefine Define { get; set; }

        // map 
        private Map map;

        // spawn time
        private float spawnTime = 0;

        // delete time
        private float unspawnTime = 0;

        // whether spawned
        private bool spawned = false;

        // spawn point
        private SpawnPointDefine spawnPoint = null;

        // constructor
        public Spawner(SpawnRuleDefine define, Map map)
        {
            // store current spawn rule and map
            this.Define = define;
            this.map = map;

            // current map exists in db, load spawn point
            if(DataManager.Instance.SpawnPoints.ContainsKey(this.map.ID))
            {
                // spawn point exists in current map, get it
                if(DataManager.Instance.SpawnPoints[this.map.ID].ContainsKey(this.Define.SpawnPoint))
                {
                    spawnPoint = DataManager.Instance.SpawnPoints[this.map.ID][this.Define.SpawnPoint];
                }

                // spawn point not exists in current map, show error
                else
                {
                    Log.ErrorFormat("SpawnRule : [{0}], SpawnPoint : [{1}] is not existed !", this.Define.ID, this.Define.SpawnPoint);
                }
            }
        }

        // update spawn
        public void Update()
        {
            if(this.CanSpawn())
            {
                this.Spawn();
            }
        }

        // decide whether can spawn
        bool CanSpawn()
        {
            // already spawned, failed
            if(this.spawned)
                return false;

            // check spawn time,  over set time , failed
            if(this.unspawnTime + this.Define.SpawnPeriod > Time.time)
                return false;

            // match request, spawn
            return true;
        }

        // spawn
        public void Spawn()
        {
            this.spawned = true;
            Log.InfoFormat("Map : [{0}], Spawn : [ ID : {1}, Monster : [{2}], Level : [{3}], Point : [{4}]", this.Define.MapID, this.Define.ID, this.Define.SpawnMonID, this.Define.SpawnLevel, this.Define.SpawnPoint);
            this.map.MonsterManager.Create(this.Define.SpawnMonID, this.Define.SpawnLevel, this.spawnPoint.Position, this.spawnPoint.Direction);
        }
    }
}