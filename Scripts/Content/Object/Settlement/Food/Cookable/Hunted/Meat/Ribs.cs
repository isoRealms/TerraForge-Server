﻿namespace Server.Items
{
	public class Ribs : Food
	{
		[Constructable]
		public Ribs() : this(1)
		{
		}

		[Constructable]
		public Ribs(int amount) : base(amount, 0x9F2)
		{
			Weight = 1.0;
			FillFactor = 5;
		}

		public Ribs(Serial serial) : base(serial)
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