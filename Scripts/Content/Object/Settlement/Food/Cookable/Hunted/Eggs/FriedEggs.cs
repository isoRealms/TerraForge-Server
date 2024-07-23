﻿namespace Server.Items
{
	public class FriedEggs : Food
	{
		[Constructable]
		public FriedEggs() : this(1)
		{
		}

		[Constructable]
		public FriedEggs(int amount) : base(amount, 0x9B6)
		{
			Weight = 1.0;
			FillFactor = 4;
		}

		public FriedEggs(Serial serial) : base(serial)
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