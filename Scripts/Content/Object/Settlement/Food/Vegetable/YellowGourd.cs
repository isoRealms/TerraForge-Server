﻿namespace Server.Items
{
	[FlipableAttribute(0xC64, 0xC65)]
	public class YellowGourd : Food
	{
		[Constructable]
		public YellowGourd() : this(1)
		{
		}

		[Constructable]
		public YellowGourd(int amount) : base(amount, 0xC64)
		{
			Weight = 1.0;
			FillFactor = 1;
		}

		public YellowGourd(Serial serial) : base(serial)
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