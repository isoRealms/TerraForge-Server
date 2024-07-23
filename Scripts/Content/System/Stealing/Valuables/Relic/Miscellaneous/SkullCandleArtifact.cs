﻿namespace Server.Items
{
	public class SkullCandleArtifact : BaseDecorationArtifact
	{
		public override int ArtifactRarity => 1;

		[Constructable]
		public SkullCandleArtifact() : base(0x1858)
		{
			Light = LightType.Circle150;
		}

		public SkullCandleArtifact(Serial serial) : base(serial)
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