using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SkillBridge.Message;

namespace Entities
{
    public class Entity
    {
        public int entityId;


        public Vector3Int Position;
        public Vector3Int Direction;
        public int Speed;

        public IEntityController Controller;

        private NEntity entityData;
        public NEntity EntityData
        {
            get {
                UpdateEntityData();
                return entityData;
            }
            set {
                entityData = value;
                this.SetEntityData(value);
            }
        }

        public Entity(NEntity entity)
        {
            this.entityId = entity.Id;
            this.entityData = entity;
            this.SetEntityData(entity);
        }

        public virtual void OnUpdate(float delta)
        {
            if (this.Speed != 0)
            {
                Vector3 dir = this.Direction;
                this.Position += Vector3Int.RoundToInt(dir * Speed * delta / 100f);
            }
            entityData.Position.FromVector3Int(this.Position);
            entityData.Direction.FromVector3Int(this.Direction);
            entityData.Speed = this.Speed;
        }

        public void SetEntityData(NEntity entity)
        {
            this.Position = this.Position.FromNVector3(entity.Position);
            this.Direction = this.Direction.FromNVector3(entity.Direction);
            this.Speed = entity.Speed;
        }

        public void UpdateEntityData()
        {
            entityData.Position.FromVector3Int(this.Position);
            entityData.Direction.FromVector3Int(this.Direction);
            entityData.Speed = this.Speed;
        }
    }
}
