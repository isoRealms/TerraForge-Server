﻿namespace Server.Items
{
	public class ShadowIronOre : BaseOre
	{
		[Constructable]
		public ShadowIronOre() : this(1)
		{
		}

		[Constructable]
		public ShadowIronOre(int amount) : base(CraftResource.ShadowIron, amount)
		{
		}

		public ShadowIronOre(Serial serial) : base(serial)
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

		public override BaseIngot GetIngot()
		{
			return new ShadowIronIngot();
		}
	}
}