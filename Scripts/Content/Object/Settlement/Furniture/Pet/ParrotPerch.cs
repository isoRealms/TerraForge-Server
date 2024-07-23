﻿namespace Server.Items
{
	public class ParrotPerchAddon : BaseAddon
	{
		public override BaseAddonDeed Deed => new ParrotPerchDeed();

		[Constructable]
		public ParrotPerchAddon()
		{
			AddComponent(new AddonComponent(0x2FF4), 0, 0, 0);
		}

		public ParrotPerchAddon(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadEncodedInt();
		}
	}

	public class ParrotPerchDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new ParrotPerchAddon();
		public override int LabelNumber => 1072617;  // parrot perch

		[Constructable]
		public ParrotPerchDeed()
		{
		}

		public ParrotPerchDeed(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadEncodedInt();
		}
	}
}