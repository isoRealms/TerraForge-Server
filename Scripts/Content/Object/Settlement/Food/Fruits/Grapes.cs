﻿namespace Server.Items
{
	public class Grapes : Food
	{
		[Constructable]
		public Grapes() : this(1)
		{
		}

		[Constructable]
		public Grapes(int amount) : base(amount, 0x9D1)
		{
			Weight = 1.0;
			FillFactor = 1;
		}

		public Grapes(Serial serial) : base(serial)
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