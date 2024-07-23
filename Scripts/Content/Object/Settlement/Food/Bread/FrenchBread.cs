﻿namespace Server.Items
{
	public class FrenchBread : Food
	{
		[Constructable]
		public FrenchBread() : this(1)
		{
		}

		[Constructable]
		public FrenchBread(int amount) : base(amount, 0x98C)
		{
			Weight = 2.0;
			FillFactor = 3;
		}

		public FrenchBread(Serial serial) : base(serial)
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