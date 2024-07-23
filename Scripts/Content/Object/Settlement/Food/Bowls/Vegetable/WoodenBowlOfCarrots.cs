﻿namespace Server.Items
{
	public class WoodenBowlOfCarrots : Food
	{
		[Constructable]
		public WoodenBowlOfCarrots() : base(0x15F9)
		{
			Stackable = false;
			Weight = 1.0;
			FillFactor = 2;
		}

		public override bool Eat(Mobile from)
		{
			if (!base.Eat(from))
			{
				return false;
			}

			from.AddToBackpack(new EmptyWoodenBowl());
			return true;
		}

		public WoodenBowlOfCarrots(Serial serial) : base(serial)
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