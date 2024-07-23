﻿namespace Server.Items
{
	[FlipableAttribute(0xc7b, 0xc7c)]
	public class Cabbage : Food
	{
		[Constructable]
		public Cabbage() : this(1)
		{
		}

		[Constructable]
		public Cabbage(int amount) : base(amount, 0xc7b)
		{
			Weight = 1.0;
			FillFactor = 1;
		}

		public Cabbage(Serial serial) : base(serial)
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