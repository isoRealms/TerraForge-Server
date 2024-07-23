﻿namespace Server.Mobiles
{
	public class SummonedSheep : BaseTalismanSummon
	{
		[Constructable]
		public SummonedSheep() : base()
		{
			Name = "a sheep";
			Body = 0xCF;
			BaseSoundID = 0xD6;
		}

		public SummonedSheep(Serial serial) : base(serial)
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