﻿namespace Server.Items
{
	public class AcidProofRobe : Robe
	{
		public override int LabelNumber => 1095236;  // Acid-Proof Robe [Replica]

		public override int BaseFireResistance => 4;

		public override int InitMinHits => 150;
		public override int InitMaxHits => 150;

		public override bool CanFortify => false;

		[Constructable]
		public AcidProofRobe()
		{
			Hue = 0x455;
			LootType = LootType.Blessed;
		}

		public AcidProofRobe(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();

			if (version < 1 && Hue == 1)
			{
				Hue = 0x455;
			}
		}
	}
}