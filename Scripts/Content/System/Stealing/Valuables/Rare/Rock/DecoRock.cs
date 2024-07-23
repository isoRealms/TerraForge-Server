﻿namespace Server.Engines.Stealables
{
	public class DecoRock : Item
	{

		[Constructable]
		public DecoRock() : base(0x1778)
		{
			Movable = true;
			Stackable = false;
		}

		public DecoRock(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();
		}
	}
}