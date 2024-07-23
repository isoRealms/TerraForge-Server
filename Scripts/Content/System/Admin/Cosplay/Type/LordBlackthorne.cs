﻿namespace Server.Items
{
	public class LordBlackthorneCostume : BaseAdminCosplay
	{
		[Constructable]
		public LordBlackthorneCostume() : base(AccessLevel.GameMaster, 0x0, 0x2043)
		{
		}

		public LordBlackthorneCostume(Serial serial) : base(serial)
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