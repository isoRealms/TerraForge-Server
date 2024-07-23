﻿namespace Server.Items
{
	public class FruitPie : Food
	{
		public override int LabelNumber => 1041346;  // baked fruit pie

		[Constructable]
		public FruitPie() : base(0x1041)
		{
			Stackable = false;
			Weight = 1.0;
			FillFactor = 5;
		}

		public FruitPie(Serial serial) : base(serial)
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