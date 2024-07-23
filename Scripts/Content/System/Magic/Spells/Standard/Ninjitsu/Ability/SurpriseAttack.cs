﻿using Server.SkillHandlers;

using System;
using System.Collections;

namespace Server.Spells.Ninjitsu
{
	public class SurpriseAttackAbility : NinjitsuAbility
	{
		private static readonly Hashtable m_Table = new();

		public override SpellName ID => SpellName.SurpriseAttack;

		public override TextDefinition AbilityMessage => 1063128; // You prepare to surprise your prey.

		public override bool ValidatesDuringHit => false;

		public SurpriseAttackAbility()
		{
		}

		public override bool Validate(Mobile from)
		{
			if (!from.Hidden || from.AllowedStealthSteps <= 0)
			{
				from.SendLocalizedMessage(1063087); // You must be in stealth mode to use this ability.
				return false;
			}

			return base.Validate(from);
		}

		public override bool OnBeforeSwing(Mobile attacker, Mobile defender)
		{
			var valid = Validate(attacker) && CheckMana(attacker, true);

			if (valid)
			{
				attacker.BeginAction(typeof(Stealth));

				Timer.DelayCall(TimeSpan.FromSeconds(5.0), attacker.EndAction, typeof(Stealth));
			}

			return valid;
		}

		public override void OnHit(Mobile attacker, Mobile defender, int damage)
		{
			//Validates before swing

			ClearCurrentMove(attacker);

			attacker.SendLocalizedMessage(1063129); // You catch your opponent off guard with your Surprise Attack!
			defender.SendLocalizedMessage(1063130); // Your defenses are lowered as your opponent surprises you!

			defender.FixedParticles(0x37B9, 1, 5, 0x26DA, 0, 3, EffectLayer.Head);

			attacker.RevealingAction();

			SurpriseAttackInfo info;

			if (m_Table.Contains(defender))
			{
				info = (SurpriseAttackInfo)m_Table[defender];

				info.m_Timer?.Stop();

				m_Table.Remove(defender);
			}

			var ninjitsu = attacker.Skills[SkillName.Ninjitsu].Fixed;
			var malus = ninjitsu / 60 + (int)Tracking.GetStalkingBonus(attacker, defender);

			info = new SurpriseAttackInfo(defender, malus);
			info.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(8.0), EndSurprise, info);

			m_Table[defender] = info;

			CheckGain(attacker);
		}

		public override void OnMiss(Mobile attacker, Mobile defender)
		{
			ClearCurrentMove(attacker);

			attacker.SendLocalizedMessage(1063161); // You failed to properly use the element of surprise.

			attacker.RevealingAction();
		}

		public static bool GetMalus(Mobile target, ref int malus)
		{
			if (m_Table[target] is SurpriseAttackInfo info)
			{
				malus = info.m_Malus;
				return true;
			}

			return false;
		}

		private static void EndSurprise(SurpriseAttackInfo info)
		{
			m_Table.Remove(info.m_Target);

			info.m_Timer?.Stop();

			info.m_Target.SendLocalizedMessage(1063131); // Your defenses have returned to normal.
		}

		private class SurpriseAttackInfo
		{
			public Mobile m_Target;
			public int m_Malus;
			public Timer m_Timer;

			public SurpriseAttackInfo(Mobile target, int effect)
			{
				m_Target = target;
				m_Malus = effect;
			}
		}
	}
}