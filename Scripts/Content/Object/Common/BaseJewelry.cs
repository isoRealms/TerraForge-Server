﻿using Server.Engines.Craft;

using System;

namespace Server.Items
{
	public enum GemType
	{
		None,
		StarSapphire,
		Emerald,
		Sapphire,
		Ruby,
		Citrine,
		Amethyst,
		Tourmaline,
		Amber,
		Diamond
	}

	public abstract class BaseJewel : Item, ICraftable
	{
		private int m_MaxHitPoints;
		private int m_HitPoints;

		private AosAttributes m_AosAttributes;
		private AosElementAttributes m_AosResistances;
		private AosSkillBonuses m_AosSkillBonuses;
		private CraftResource m_Resource;
		private GemType m_GemType;

		[CommandProperty(AccessLevel.GameMaster)]
		public int MaxHitPoints
		{
			get => m_MaxHitPoints;
			set { m_MaxHitPoints = value; InvalidateProperties(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int HitPoints
		{
			get => m_HitPoints;
			set
			{
				if (value != m_HitPoints && MaxHitPoints > 0)
				{
					m_HitPoints = value;

					if (m_HitPoints < 0)
					{
						Delete();
					}
					else if (m_HitPoints > MaxHitPoints)
					{
						m_HitPoints = MaxHitPoints;
					}

					InvalidateProperties();
				}
			}
		}

		[CommandProperty(AccessLevel.Player)]
		public AosAttributes Attributes
		{
			get => m_AosAttributes;
			set { }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public AosElementAttributes Resistances
		{
			get => m_AosResistances;
			set { }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public AosSkillBonuses SkillBonuses
		{
			get => m_AosSkillBonuses;
			set { }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public CraftResource Resource
		{
			get => m_Resource;
			set { m_Resource = value; Hue = CraftResources.GetHue(m_Resource); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public GemType GemType
		{
			get => m_GemType;
			set { m_GemType = value; InvalidateProperties(); }
		}

		public override int PhysicalResistance => m_AosResistances.Physical;
		public override int FireResistance => m_AosResistances.Fire;
		public override int ColdResistance => m_AosResistances.Cold;
		public override int PoisonResistance => m_AosResistances.Poison;
		public override int EnergyResistance => m_AosResistances.Energy;
		public virtual int BaseGemTypeNumber => 0;

		public virtual int InitMinHits => 0;
		public virtual int InitMaxHits => 0;

		public override int LabelNumber
		{
			get
			{
				if (m_GemType == GemType.None)
				{
					return base.LabelNumber;
				}

				return BaseGemTypeNumber + (int)m_GemType - 1;
			}
		}

		public override void OnAfterDuped(Item newItem)
		{
			var jewel = newItem as BaseJewel;

			if (jewel == null)
			{
				return;
			}

			jewel.m_AosAttributes = new AosAttributes(newItem, m_AosAttributes);
			jewel.m_AosResistances = new AosElementAttributes(newItem, m_AosResistances);
			jewel.m_AosSkillBonuses = new AosSkillBonuses(newItem, m_AosSkillBonuses);
		}

		public virtual int ArtifactRarity => 0;

		public BaseJewel(int itemID, Layer layer) : base(itemID)
		{
			m_AosAttributes = new AosAttributes(this);
			m_AosResistances = new AosElementAttributes(this);
			m_AosSkillBonuses = new AosSkillBonuses(this);
			m_Resource = CraftResource.Iron;
			m_GemType = GemType.None;

			Layer = layer;

			m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax(InitMinHits, InitMaxHits);
		}

		public override void OnAdded(IEntity parent)
		{
			if (Core.AOS && parent is Mobile)
			{
				var from = (Mobile)parent;

				m_AosSkillBonuses.AddTo(from);

				var strBonus = m_AosAttributes.BonusStr;
				var dexBonus = m_AosAttributes.BonusDex;
				var intBonus = m_AosAttributes.BonusInt;

				if (strBonus != 0 || dexBonus != 0 || intBonus != 0)
				{
					var modName = Serial.ToString();

					if (strBonus != 0)
					{
						from.AddStatMod(new StatMod(StatType.Str, modName + "Str", strBonus, TimeSpan.Zero));
					}

					if (dexBonus != 0)
					{
						from.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero));
					}

					if (intBonus != 0)
					{
						from.AddStatMod(new StatMod(StatType.Int, modName + "Int", intBonus, TimeSpan.Zero));
					}
				}

				from.CheckStatTimers();
			}
		}

		public override void OnRemoved(IEntity parent)
		{
			if (Core.AOS && parent is Mobile)
			{
				var from = (Mobile)parent;

				m_AosSkillBonuses.Remove();

				var modName = Serial.ToString();

				from.RemoveStatMod(modName + "Str");
				from.RemoveStatMod(modName + "Dex");
				from.RemoveStatMod(modName + "Int");

				from.CheckStatTimers();
			}
		}

		public BaseJewel(Serial serial) : base(serial)
		{
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			m_AosSkillBonuses.GetProperties(list);

			int prop;

			if ((prop = ArtifactRarity) > 0)
			{
				list.Add(1061078, prop.ToString()); // artifact rarity ~1_val~
			}

			if ((prop = m_AosAttributes.WeaponDamage) != 0)
			{
				list.Add(1060401, prop.ToString()); // damage increase ~1_val~%
			}

			if ((prop = m_AosAttributes.DefendChance) != 0)
			{
				list.Add(1060408, prop.ToString()); // defense chance increase ~1_val~%
			}

			if ((prop = m_AosAttributes.BonusDex) != 0)
			{
				list.Add(1060409, prop.ToString()); // dexterity bonus ~1_val~
			}

			if ((prop = m_AosAttributes.EnhancePotions) != 0)
			{
				list.Add(1060411, prop.ToString()); // enhance potions ~1_val~%
			}

			if ((prop = m_AosAttributes.CastRecovery) != 0)
			{
				list.Add(1060412, prop.ToString()); // faster cast recovery ~1_val~
			}

			if ((prop = m_AosAttributes.CastSpeed) != 0)
			{
				list.Add(1060413, prop.ToString()); // faster casting ~1_val~
			}

			if ((prop = m_AosAttributes.AttackChance) != 0)
			{
				list.Add(1060415, prop.ToString()); // hit chance increase ~1_val~%
			}

			if ((prop = m_AosAttributes.BonusHits) != 0)
			{
				list.Add(1060431, prop.ToString()); // hit point increase ~1_val~
			}

			if ((prop = m_AosAttributes.BonusInt) != 0)
			{
				list.Add(1060432, prop.ToString()); // intelligence bonus ~1_val~
			}

			if ((prop = m_AosAttributes.LowerManaCost) != 0)
			{
				list.Add(1060433, prop.ToString()); // lower mana cost ~1_val~%
			}

			if ((prop = m_AosAttributes.LowerRegCost) != 0)
			{
				list.Add(1060434, prop.ToString()); // lower reagent cost ~1_val~%
			}

			if ((prop = m_AosAttributes.Luck) != 0)
			{
				list.Add(1060436, prop.ToString()); // luck ~1_val~
			}

			if ((prop = m_AosAttributes.BonusMana) != 0)
			{
				list.Add(1060439, prop.ToString()); // mana increase ~1_val~
			}

			if ((prop = m_AosAttributes.RegenMana) != 0)
			{
				list.Add(1060440, prop.ToString()); // mana regeneration ~1_val~
			}

			if ((prop = m_AosAttributes.NightSight) != 0)
			{
				list.Add(1060441); // night sight
			}

			if ((prop = m_AosAttributes.ReflectPhysical) != 0)
			{
				list.Add(1060442, prop.ToString()); // reflect physical damage ~1_val~%
			}

			if ((prop = m_AosAttributes.RegenStam) != 0)
			{
				list.Add(1060443, prop.ToString()); // stamina regeneration ~1_val~
			}

			if ((prop = m_AosAttributes.RegenHits) != 0)
			{
				list.Add(1060444, prop.ToString()); // hit point regeneration ~1_val~
			}

			if ((prop = m_AosAttributes.SpellChanneling) != 0)
			{
				list.Add(1060482); // spell channeling
			}

			if ((prop = m_AosAttributes.SpellDamage) != 0)
			{
				list.Add(1060483, prop.ToString()); // spell damage increase ~1_val~%
			}

			if ((prop = m_AosAttributes.BonusStam) != 0)
			{
				list.Add(1060484, prop.ToString()); // stamina increase ~1_val~
			}

			if ((prop = m_AosAttributes.BonusStr) != 0)
			{
				list.Add(1060485, prop.ToString()); // strength bonus ~1_val~
			}

			if ((prop = m_AosAttributes.WeaponSpeed) != 0)
			{
				list.Add(1060486, prop.ToString()); // swing speed increase ~1_val~%
			}

			if (Core.ML && (prop = m_AosAttributes.IncreasedKarmaLoss) != 0)
			{
				list.Add(1075210, prop.ToString()); // Increased Karma Loss ~1val~%
			}

			base.AddResistanceProperties(list);

			if (m_HitPoints >= 0 && m_MaxHitPoints > 0)
			{
				list.Add(1060639, "{0}\t{1}", m_HitPoints, m_MaxHitPoints); // durability ~1_val~ / ~2_val~
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(3); // version

			writer.WriteEncodedInt(m_MaxHitPoints);
			writer.WriteEncodedInt(m_HitPoints);

			writer.WriteEncodedInt((int)m_Resource);
			writer.WriteEncodedInt((int)m_GemType);

			m_AosAttributes.Serialize(writer);
			m_AosResistances.Serialize(writer);
			m_AosSkillBonuses.Serialize(writer);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();

			switch (version)
			{
				case 3:
					{
						m_MaxHitPoints = reader.ReadEncodedInt();
						m_HitPoints = reader.ReadEncodedInt();

						goto case 2;
					}
				case 2:
					{
						m_Resource = (CraftResource)reader.ReadEncodedInt();
						m_GemType = (GemType)reader.ReadEncodedInt();

						goto case 1;
					}
				case 1:
					{
						m_AosAttributes = new AosAttributes(this, reader);
						m_AosResistances = new AosElementAttributes(this, reader);
						m_AosSkillBonuses = new AosSkillBonuses(this, reader);

						if (Core.AOS && Parent is Mobile)
						{
							m_AosSkillBonuses.AddTo((Mobile)Parent);
						}

						var strBonus = m_AosAttributes.BonusStr;
						var dexBonus = m_AosAttributes.BonusDex;
						var intBonus = m_AosAttributes.BonusInt;

						if (Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0))
						{
							var m = (Mobile)Parent;

							var modName = Serial.ToString();

							if (strBonus != 0)
							{
								m.AddStatMod(new StatMod(StatType.Str, modName + "Str", strBonus, TimeSpan.Zero));
							}

							if (dexBonus != 0)
							{
								m.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero));
							}

							if (intBonus != 0)
							{
								m.AddStatMod(new StatMod(StatType.Int, modName + "Int", intBonus, TimeSpan.Zero));
							}
						}

						if (Parent is Mobile)
						{
							((Mobile)Parent).CheckStatTimers();
						}

						break;
					}
				case 0:
					{
						m_AosAttributes = new AosAttributes(this);
						m_AosResistances = new AosElementAttributes(this);
						m_AosSkillBonuses = new AosSkillBonuses(this);

						break;
					}
			}

			if (version < 2)
			{
				m_Resource = CraftResource.Iron;
				m_GemType = GemType.None;
			}
		}

		#region ICraftable

		public virtual int OnCraft(int quality, bool makersMark, Mobile from, ICraftSystem craftSystem, Type typeRes, ICraftTool tool, ICraftItem craftItem, int resHue)
		{
			var resourceType = typeRes;

			var ci = craftItem as CraftItem;

			if (resourceType == null && ci != null)
			{
				resourceType = ci.Resources.GetAt(0).ItemType;
			}

			Resource = CraftResources.GetFromType(resourceType);

			if (craftSystem is CraftSystem cs)
			{
				var context = cs.GetContext(from);

				if (context != null && context.DoNotColor)
				{
					Hue = 0;
				}
			}

			if (ci != null && ci.Resources.Count > 1)
			{
				resourceType = ci.Resources.GetAt(1).ItemType;

				if (resourceType == typeof(StarSapphire))
				{
					GemType = GemType.StarSapphire;
				}
				else if (resourceType == typeof(Emerald))
				{
					GemType = GemType.Emerald;
				}
				else if (resourceType == typeof(Sapphire))
				{
					GemType = GemType.Sapphire;
				}
				else if (resourceType == typeof(Ruby))
				{
					GemType = GemType.Ruby;
				}
				else if (resourceType == typeof(Citrine))
				{
					GemType = GemType.Citrine;
				}
				else if (resourceType == typeof(Amethyst))
				{
					GemType = GemType.Amethyst;
				}
				else if (resourceType == typeof(Tourmaline))
				{
					GemType = GemType.Tourmaline;
				}
				else if (resourceType == typeof(Amber))
				{
					GemType = GemType.Amber;
				}
				else if (resourceType == typeof(Diamond))
				{
					GemType = GemType.Diamond;
				}
			}

			return 1;
		}

		#endregion
	}
}