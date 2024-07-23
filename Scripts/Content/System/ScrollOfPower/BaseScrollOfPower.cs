﻿using Server.Gumps;
using Server.Network;

using System;

namespace Server.Items
{
	public abstract class ScrollOfPower : Item
	{
		private SkillName m_Skill;
		private double m_Value;

		#region Old Item Serialization Vars
		/* DO NOT USE! Only used in serialization of special scrolls that originally derived from Item */
		private bool m_InheritsItem;

		protected bool InheritsItem => m_InheritsItem;
		#endregion

		public abstract int Message { get; }
		public virtual int Title => 0;
		public abstract string DefaultTitle { get; }

		public ScrollOfPower(SkillName skill, double value) : base(0x14F0)
		{
			LootType = LootType.Cursed;
			Weight = 1.0;

			m_Skill = skill;
			m_Value = value;
		}

		public ScrollOfPower(Serial serial) : base(serial)
		{
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public SkillName Skill
		{
			get => m_Skill;
			set => m_Skill = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public double Value
		{
			get => m_Value;
			set => m_Value = value;
		}

		public virtual string GetNameLocalized()
		{
			return String.Concat("#", AosSkillBonuses.GetLabel(m_Skill).ToString());
		}

		public virtual string GetName()
		{
			var index = (int)m_Skill;
			var table = SkillInfo.Table;

			if (index >= 0 && index < table.Length)
			{
				return table[index].Name.ToLower();
			}
			else
			{
				return "???";
			}
		}

		public virtual bool CanUse(Mobile from)
		{
			if (Deleted)
			{
				return false;
			}

			if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
				return false;
			}

			return true;
		}

		public virtual void Use(Mobile from)
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!CanUse(from))
			{
				return;
			}

			from.CloseGump(typeof(ScrollOfPower.InternalGump));
			from.SendGump(new InternalGump(from, this));
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			writer.Write((int)m_Skill);
			writer.Write(m_Value);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();

			switch (version)
			{
				case 1:
					{
						m_Skill = (SkillName)reader.ReadInt();
						m_Value = reader.ReadDouble();
						break;
					}
				case 0:
					{
						m_InheritsItem = true;

						if (!(this is StatCapScroll))
						{
							m_Skill = (SkillName)reader.ReadInt();
						}
						else
						{
							m_Skill = SkillName.Alchemy;
						}

						if (this is ScrollofAlacrity)
						{
							m_Value = 0.0;
						}
						else if (this is StatCapScroll)
						{
							m_Value = reader.ReadInt();
						}
						else
						{
							m_Value = reader.ReadDouble();
						}

						break;
					}
			}
		}

		public class InternalGump : Gump
		{
			private readonly Mobile m_Mobile;
			private readonly ScrollOfPower m_Scroll;

			public InternalGump(Mobile mobile, ScrollOfPower scroll) : base(25, 50)
			{
				m_Mobile = mobile;
				m_Scroll = scroll;

				AddPage(0);

				AddBackground(25, 10, 420, 200, 5054);

				AddImageTiled(33, 20, 401, 181, 2624);
				AddAlphaRegion(33, 20, 401, 181);

				AddHtmlLocalized(40, 48, 387, 100, m_Scroll.Message, true, true);

				AddHtmlLocalized(125, 148, 200, 20, 1049478, 0x7FFF, false, false); // Do you wish to use this scroll?

				AddButton(100, 172, 4005, 4007, 1, GumpButtonType.Reply, 0);
				AddHtmlLocalized(135, 172, 120, 20, 1046362, 0x7FFF, false, false); // Yes

				AddButton(275, 172, 4005, 4007, 0, GumpButtonType.Reply, 0);
				AddHtmlLocalized(310, 172, 120, 20, 1046363, 0x7FFF, false, false); // No

				if (m_Scroll.Title != 0)
				{
					AddHtmlLocalized(40, 20, 260, 20, m_Scroll.Title, 0x7FFF, false, false);
				}
				else
				{
					AddHtml(40, 20, 260, 20, m_Scroll.DefaultTitle, false, false);
				}

				if (m_Scroll is StatCapScroll)
				{
					AddHtmlLocalized(310, 20, 120, 20, 1038019, 0x7FFF, false, false); // Power
				}
				else
				{
					AddHtmlLocalized(310, 20, 120, 20, AosSkillBonuses.GetLabel(m_Scroll.Skill), 0x7FFF, false, false);
				}
			}

			public override void OnResponse(NetState state, RelayInfo info)
			{
				if (info.ButtonID == 1)
				{
					m_Scroll.Use(m_Mobile);
				}
			}
		}
	}
}