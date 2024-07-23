﻿namespace Server.Items
{
	public class SausagePizza : Food
	{
		public override int LabelNumber => 1044517;  // sausage pizza

		[Constructable]
		public SausagePizza() : base(0x1040)
		{
			Stackable = false;
			Weight = 1.0;
			FillFactor = 6;
		}

		public SausagePizza(Serial serial) : base(serial)
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