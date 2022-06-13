using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public interface IEntityController
    {
        void PlayAnim(string name);
        void SetStandby(bool standby);
        void PlayEffect(EffectType type, string name, Entity target, float duration);
        void PlayEffect(EffectType type, string name, NVector3 position, float duration);
        Transform GetTransform();
        void UpdateDirection();
        
    }
}