﻿using System.Collections.Generic;

#region Developer Notations

/// In Select Shops There Should ALWAYS Be One Merchant That Sells Every Resources For Their Trade
/// In Select Shops There Should ALWAYS Be One Merchant That Sells Every TradeTools For Their Trade
/// In Select Shops There Should ALWAYS Be One Merchant That Sells Products Created From Their Trade

#endregion

namespace Server.Mobiles
{
	public class Ranger : BaseVendor
	{
		private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos => m_SBInfos;

		[Constructable]
		public Ranger() : base("the ranger")
		{
			SetSkill(SkillName.Camping, 55.0, 78.0);
			SetSkill(SkillName.DetectHidden, 65.0, 88.0);
			SetSkill(SkillName.Hiding, 45.0, 68.0);
			SetSkill(SkillName.Archery, 65.0, 88.0);
			SetSkill(SkillName.Tracking, 65.0, 88.0);
			SetSkill(SkillName.Veterinary, 60.0, 83.0);
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add(new SBRanger());
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem(new Server.Items.Shirt(Utility.RandomNeutralHue()));
			AddItem(new Server.Items.LongPants(Utility.RandomNeutralHue()));
			AddItem(new Server.Items.Bow());
			AddItem(new Server.Items.ThighBoots(Utility.RandomNeutralHue()));
		}

		public Ranger(Serial serial) : base(serial)
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