﻿namespace Server.Items
{
	[FlipableAttribute(0xc77, 0xc78)]
	public class Carrot : Food
	{
		[Constructable]
		public Carrot() : this(1)
		{
		}

		[Constructable]
		public Carrot(int amount) : base(amount, 0xc78)
		{
			Weight = 1.0;
			FillFactor = 1;
		}

		public Carrot(Serial serial) : base(serial)
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