﻿namespace Server.Items
{
	public class Peach : Food
	{
		[Constructable]
		public Peach() : this(1)
		{
		}

		[Constructable]
		public Peach(int amount) : base(amount, 0x9D2)
		{
			Weight = 1.0;
			FillFactor = 1;
		}

		public Peach(Serial serial) : base(serial)
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