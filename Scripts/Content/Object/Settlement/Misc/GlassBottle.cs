﻿namespace Server.Items
{
	public class GlassBottle : Item
	{
		[Constructable]
		public GlassBottle() : base(0xe2b)
		{
			Weight = 0.3;
		}

		public GlassBottle(Serial serial) : base(serial)
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