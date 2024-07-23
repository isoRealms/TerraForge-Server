﻿namespace Server.Items
{
	public class CocoonArtifact : BaseDecorationArtifact
	{
		public override int ArtifactRarity => 7;

		[Constructable]
		public CocoonArtifact() : base(0x10DA)
		{
		}

		public CocoonArtifact(Serial serial) : base(serial)
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