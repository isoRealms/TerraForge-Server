﻿namespace Server.Items
{
	/// Pouch
	public class Pouch : TrapableContainer
	{
		[Constructable]
		public Pouch() : base(0xE79)
		{
			Weight = 1.0;
		}

		public Pouch(Serial serial) : base(serial)
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