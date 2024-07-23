﻿namespace Server.Items
{
	[FlipableAttribute(0xC66, 0xC67)]
	public class GreenGourd : Food
	{
		[Constructable]
		public GreenGourd() : this(1)
		{
		}

		[Constructable]
		public GreenGourd(int amount) : base(amount, 0xC66)
		{
			Weight = 1.0;
			FillFactor = 1;
		}

		public GreenGourd(Serial serial) : base(serial)
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