﻿namespace Server.Items
{
	public class RoastPig : Food
	{
		[Constructable]
		public RoastPig() : this(1)
		{
		}

		[Constructable]
		public RoastPig(int amount) : base(amount, 0x9BB)
		{
			Weight = 45.0;
			FillFactor = 20;
		}

		public RoastPig(Serial serial) : base(serial)
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