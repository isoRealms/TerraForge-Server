﻿
using System;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName("a blade spirit corpse")]
	public class BladeSpirits : BaseCreature
	{
		public override bool DeleteCorpseOnDeath => Core.AOS;
		public override bool IsHouseSummonable => true;

		public override double DispelDifficulty => 0.0;
		public override double DispelFocus => 20.0;

		public override double GetFightModeRanking(Mobile m, FightMode acqType, bool bPlayerOnly)
		{
			return (m.Str + m.Skills[SkillName.Tactics].Value) / Math.Max(GetDistanceToSqrt(m), 1.0);
		}

		[Constructable]
		public BladeSpirits()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.3, 0.6)
		{
			Name = "a blade spirit";
			Body = 574;

			SetStr(150);
			SetDex(150);
			SetInt(100);

			SetHits((Core.SE) ? 160 : 80);
			SetStam(250);
			SetMana(0);

			SetDamage(10, 14);

			SetDamageType(ResistanceType.Physical, 60);
			SetDamageType(ResistanceType.Poison, 20);
			SetDamageType(ResistanceType.Energy, 20);

			SetResistance(ResistanceType.Physical, 30, 40);
			SetResistance(ResistanceType.Fire, 40, 50);
			SetResistance(ResistanceType.Cold, 30, 40);
			SetResistance(ResistanceType.Poison, 100);
			SetResistance(ResistanceType.Energy, 20, 30);

			SetSkill(SkillName.MagicResist, 70.0);
			SetSkill(SkillName.Tactics, 90.0);
			SetSkill(SkillName.Wrestling, 90.0);

			Fame = 0;
			Karma = 0;

			VirtualArmor = 40;
			ControlSlots = (Core.SE) ? 2 : 1;
		}

		public override bool BleedImmune => true;
		public override Poison PoisonImmune => Poison.Lethal;

		public override int GetAngerSound()
		{
			return 0x23A;
		}

		public override int GetAttackSound()
		{
			return 0x3B8;
		}

		public override int GetHurtSound()
		{
			return 0x23A;
		}

		public override void OnThink()
		{
			if (Core.SE && Summoned)
			{
				var spirtsOrVortexes = new ArrayList();

				foreach (var m in GetMobilesInRange(5))
				{
					if (m is EnergyVortex || m is BladeSpirits)
					{
						if (((BaseCreature)m).Summoned)
						{
							spirtsOrVortexes.Add(m);
						}
					}
				}

				while (spirtsOrVortexes.Count > 6)
				{
					var index = Utility.Random(spirtsOrVortexes.Count);
					Dispel(((Mobile)spirtsOrVortexes[index]));
					spirtsOrVortexes.RemoveAt(index);
				}
			}

			base.OnThink();
		}

		public BladeSpirits(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();
		}
	}
}