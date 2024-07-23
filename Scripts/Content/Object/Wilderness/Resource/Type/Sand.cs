﻿namespace Server.Items
{
	[FlipableAttribute(0x11EA, 0x11EB)]
	public class Sand : Item, ICommodity
	{
		int ICommodity.DescriptionNumber => LabelNumber;
		bool ICommodity.IsDeedable => true;

		public override int LabelNumber => 1044626;  // sand

		[Constructable]
		public Sand() : this(1)
		{
		}

		[Constructable]
		public Sand(int amount) : base(0x11EA)
		{
			Stackable = Core.ML;
			Weight = 1.0;
		}

		public Sand(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();

			if (version == 0 && Name == "sand")
			{
				Name = null;
			}
		}
	}
}