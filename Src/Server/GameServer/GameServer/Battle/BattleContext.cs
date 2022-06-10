using GameServer.Core;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
	public class BattleContext
	{
		public Battle Battle;
		public Creature Caster;
		public Creature Target;
		public Vector3Int Position;

		public NSkillCastInfo CastSkill;
		public NDamageInfo Damage;

		public SkillResult Result;

		public BattleContex(Battle battle)
        {
			this.Battle = battle;
        }
	}
}

