using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Models;

namespace GameServer.Managers
{
    class MapManager : Singleton<MapManager>
    {
        // Maps Dictionary : Store the map information
        Dictionary<int, Map> Maps = new Dictionary<int, Map>();

        // initialization
        public void Init()
        {
            // travel all map data in db
            foreach (var mapdefine in DataManager.Instance.Maps.Values)
            {
                Map map = new Map(mapdefine);  // construct the Map Object
                Log.InfoFormat("MapManager.Init > Map:{0}:{1}", map.Define.ID, map.Define.Name);
                this.Maps[mapdefine.ID] = map; // add Map Object created before in to Maps Dictionary
            }
        }



        public Map this[int key]
        {
            // overload operation
            get
            {
                return this.Maps[key];
            }
        }


        // self update service : except operations asked by others,
        // slef should do operations
        public void Update()
        {
            // update map logic, such as update Boss in period
            foreach(var map in this.Maps.Values)
            {
                map.Update();
            }
        }
    }
}
