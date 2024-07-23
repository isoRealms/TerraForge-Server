﻿namespace Server.Items
{
	public class ApplePie : Food
	{
		public override int LabelNumber => 1041343;  // baked apple pie

		[Constructable]
		public ApplePie() : base(0x1041)
		{
			Stackable = false;
			Weight = 1.0;
			FillFactor = 5;
		}

		public ApplePie(Serial serial) : base(serial)
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