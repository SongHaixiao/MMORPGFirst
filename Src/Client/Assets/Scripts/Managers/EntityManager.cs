using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    interface IEntityNotify
    {
        void OnEntityChanged(Entity entity);
        void OnEntityEvent(EntityEvent eventEvent, int param);
        void onEntityRemoved();
    }

    class EntityManager : Singleton<EntityManager>
    {
        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        Dictionary<int, IEntityNotify> notifiers = new Dictionary<int, IEntityNotify>();

        public void RegiserEntityChangeNotity(int entityId, IEntityNotify notify)
        {
            this.notifiers[entityId] = notify;
        }

        public void AddEntity(Entity entity)
        {
            entities[entity.entityId] = entity;
        }

        internal void OnEntitySync(NEntitySync data)
        {
            Entity entity = null;
            entities.TryGetValue(data.Id, out entity);
            if (entity != null)
            {
                if (data.Entity != null)
                    entity.EntityData = data.Entity;
                if (notifiers.ContainsKey(data.Id))
                {
                    notifiers[entity.entityId].OnEntityChanged(entity);
                    notifiers[entity.entityId].OnEntityEvent(data.Event, data.Param);
                }
            }
        }

        public void RemoveEntity(NEntity entity)
        {
            this.entities.Remove(entity.Id);
            if (notifiers.ContainsKey(entity.Id))
            {
                notifiers[entity.Id].onEntityRemoved();
                notifiers.Remove(entity.Id);
            }
        }

        public Entity GetEntity(int entityId)
        {
            Entity entity = null;
            entities.TryGetValue(entityId, out entity);
            return entity;
        }
    }
}
