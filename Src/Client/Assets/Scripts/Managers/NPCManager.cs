using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    class NPCManager : Singleton<NPCManager>
    {

        /* Function : 
         *  - Control the NPC event.
         *  - Event is the only interface between NPC Sytem and others.*/

        public delegate bool NpcActionHandler(NpcDefine npc);

        Dictionary<NpcFunction, NpcActionHandler> eventMap = new Dictionary<NpcFunction, NpcActionHandler>();

        // register npc event
        // other system tell npc system to call which event
        public void RegisterNpcEvent(NpcFunction function, NpcActionHandler action)
        {
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;
            }
            else
            {
                eventMap[function] += action;
            }
        }

        // get npc data
        public NpcDefine GetNpcDefine(int npcID)
        {
            NpcDefine npc = null;
            DataManager.Instance.NPCs.TryGetValue(npcID, out npc);
            return npc;
        }

        // interactive checking
        public bool Interactive(int npcId)
        {
            // npcId is existed
            if(DataManager.Instance.NPCs.ContainsKey(npcId))
            {
                // get this npc, and call Interactive(NPCDefine)
                // to make player interact with npc
                var npc = DataManager.Instance.NPCs[npcId];
                return Interactive(npc);
            }
            return false;
        }

        // interactive real opeartion
        public bool Interactive(NpcDefine npc)
        {
            // npc type is task, do task ineractive
            if (DoTaskInteractive(npc))
            {
                return true;
            }

            // npc type is functional
            else if(npc.Type == NpcType.Functional)
            {
                // do functional interact
                return DoFunctionInteractive(npc);
            }
            return false;
        }

        // task interactive operation
        private bool DoTaskInteractive(NpcDefine npc)
        {
            var status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
            if (status == NpcQuestStatus.None)
                return false;

            return QuestManager.Instance.OpenNpcQuest(npc.ID);
        }

        // function interactive opeartion
        private bool DoFunctionInteractive(NpcDefine npc)
        {
            // check npc type
            if (npc.Type != NpcType.Functional)
                return false;

            // check npc function is existed in npc event manager
            if (!eventMap.ContainsKey(npc.Function))
                return false;

            // all is ok, return the npc 
            return eventMap[npc.Function](npc);
        }
    }
}
