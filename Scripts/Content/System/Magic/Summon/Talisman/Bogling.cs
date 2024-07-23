﻿namespace Server.Mobiles
{
	public class SummonedBogling : BaseTalismanSummon
	{
		[Constructable]
		public SummonedBogling() : base()
		{
			Name = "a bogling";
			Body = 779;
			BaseSoundID = 422;
		}

		public SummonedBogling(Serial serial) : base(serial)
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