using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

using System.Threading;

using Network;
using GameServer.Services;
using GameServer.Managers;

namespace GameServer
{
    class GameServer
    {

        NetService network; // net service
        Thread thread;  
        bool running = false;

        public bool Init()
        {
            // network initialization
            int Port = Properties.Settings.Default.ServerPort; // get network port
            network = new NetService(); // create net service
            network.Init(Port);         // initialization network port

            // start services
            DBService.Instance.Init();
            DataManager.Instance.Load();
            MapService.Instance.Init();
            UserService.Instance.Init();
            ItemService.Instance.Init();
            QuestService.Instance.Init();
            BattleService.Instance.Init();

            thread = new Thread(new ThreadStart(this.Update));
            return true;
        }

        public void Start()
        {
            network.Start();
            running = true;
            thread.Start();
        }


        public void Stop()
        {
            running = false;
            thread.Join();
            network.Stop();
        }

        public void Update()
        {
            var mapManager = MapManager.Instance;
            while (running)
            {
                Time.Tick();
                Thread.Sleep(100);
                //Console.WriteLine("{0} {1} {2} {3} {4}", Time.deltaTime, Time.frameCount, Time.ticks, Time.time, Time.realtimeSinceStartup);
                mapManager.Update();
            }
        }
    }
}
