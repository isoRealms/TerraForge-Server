﻿namespace Server.Engines.Stealables
{
	public class DecoFullJars3 : Item
	{
		[Constructable]
		public DecoFullJars3()
			: base(0xE4a)
		{
			Movable = true;
			Stackable = false;
		}

		public DecoFullJars3(Serial serial)
			: base(serial)
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