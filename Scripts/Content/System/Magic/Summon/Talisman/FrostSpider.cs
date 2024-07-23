﻿namespace Server.Mobiles
{
	public class SummonedFrostSpider : BaseTalismanSummon
	{
		[Constructable]
		public SummonedFrostSpider() : base()
		{
			Name = "a frost spider";
			Body = 20;
			BaseSoundID = 0x388;
		}

		public SummonedFrostSpider(Serial serial) : base(serial)
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