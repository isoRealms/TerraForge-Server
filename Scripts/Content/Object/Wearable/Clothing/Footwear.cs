﻿
using System;

namespace Server.Items
{
	/// Shoes
	[FlipableAttribute(0x170f, 0x1710)]
	public class Shoes : BaseShoes
	{
		public override CraftResource DefaultResource => CraftResource.RegularLeather;

		[Constructable]
		public Shoes() : this(0)
		{
		}

		[Constructable]
		public Shoes(int hue) : base(0x170F, hue)
		{
			Weight = 2.0;
		}

		public Shoes(Serial serial) : base(serial)
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

	/// Sandals
	[FlipableAttribute(0x170d, 0x170e)]
	public class Sandals : BaseShoes
	{
		public override CraftResource DefaultResource => CraftResource.RegularLeather;

		[Constructable]
		public Sandals() : this(0)
		{
		}

		[Constructable]
		public Sandals(int hue) : base(0x170D, hue)
		{
			Weight = 1.0;
		}

		public Sandals(Serial serial) : base(serial)
		{
		}

		public override bool Dye(Mobile from, DyeTub sender)
		{
			return false;
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

	/// Boots
	[FlipableAttribute(0x170b, 0x170c)]
	public class Boots : BaseShoes
	{
		public override CraftResource DefaultResource => CraftResource.RegularLeather;

		[Constructable]
		public Boots() : this(0)
		{
		}

		[Constructable]
		public Boots(int hue) : base(0x170B, hue)
		{
			Weight = 3.0;
		}

		public Boots(Serial serial) : base(serial)
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

	/// FurBoots
	[Flipable(0x2307, 0x2308)]
	public class FurBoots : BaseShoes
	{
		[Constructable]
		public FurBoots() : this(0)
		{
		}

		[Constructable]
		public FurBoots(int hue) : base(0x2307, hue)
		{
			Weight = 3.0;
		}

		public FurBoots(Serial serial) : base(serial)
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

	/// ThighBoots
	[Flipable]
	public class ThighBoots : BaseShoes, IArcaneEquip
	{
		#region Arcane Impl
		private int m_MaxArcaneCharges, m_CurArcaneCharges;

		[CommandProperty(AccessLevel.GameMaster)]
		public int MaxArcaneCharges
		{
			get => m_MaxArcaneCharges;
			set { m_MaxArcaneCharges = value; InvalidateProperties(); Update(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int CurArcaneCharges
		{
			get => m_CurArcaneCharges;
			set { m_CurArcaneCharges = value; InvalidateProperties(); Update(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsArcane => (m_MaxArcaneCharges > 0 && m_CurArcaneCharges >= 0);

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			if (IsArcane)
			{
				LabelTo(from, 1061837, String.Format("{0}\t{1}", m_CurArcaneCharges, m_MaxArcaneCharges));
			}
		}

		public void Update()
		{
			if (IsArcane)
			{
				ItemID = 0x26AF;
			}
			else if (ItemID == 0x26AF)
			{
				ItemID = 0x1711;
			}

			if (IsArcane && CurArcaneCharges == 0)
			{
				Hue = 0;
			}
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (IsArcane)
			{
				list.Add(1061837, "{0}\t{1}", m_CurArcaneCharges, m_MaxArcaneCharges); // arcane charges: ~1_val~ / ~2_val~
			}
		}

		public void Flip()
		{
			if (ItemID == 0x1711)
			{
				ItemID = 0x1712;
			}
			else if (ItemID == 0x1712)
			{
				ItemID = 0x1711;
			}
		}
		#endregion

		public override CraftResource DefaultResource => CraftResource.RegularLeather;

		[Constructable]
		public ThighBoots() : this(0)
		{
		}

		[Constructable]
		public ThighBoots(int hue) : base(0x1711, hue)
		{
			Weight = 4.0;
		}

		public ThighBoots(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			if (IsArcane)
			{
				writer.Write(true);
				writer.Write(m_CurArcaneCharges);
				writer.Write(m_MaxArcaneCharges);
			}
			else
			{
				writer.Write(false);
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();

			switch (version)
			{
				case 1:
					{
						if (reader.ReadBool())
						{
							m_CurArcaneCharges = reader.ReadInt();
							m_MaxArcaneCharges = reader.ReadInt();

							if (Hue == 2118)
							{
								Hue = ArcaneGem.DefaultArcaneHue;
							}
						}

						break;
					}
			}
		}
	}

	/// ElvenBoots
	[FlipableAttribute(0x2FC4, 0x317A)]
	public class ElvenBoots : BaseShoes
	{
		public override CraftResource DefaultResource => CraftResource.RegularLeather;

		public override Race RequiredRace => Race.Elf;

		[Constructable]
		public ElvenBoots() : this(0)
		{
		}

		[Constructable]
		public ElvenBoots(int hue) : base(0x2FC4, hue)
		{
			Weight = 2.0;
		}

		public ElvenBoots(Serial serial) : base(serial)
		{
		}

		public override bool Dye(Mobile from, DyeTub sender)
		{
			return false;
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

	/// NinjaTabi
	[Flipable(0x2797, 0x27E2)]
	public class NinjaTabi : BaseShoes
	{
		[Constructable]
		public NinjaTabi() : this(0)
		{
		}

		[Constructable]
		public NinjaTabi(int hue) : base(0x2797, hue)
		{
			Weight = 2.0;
		}

		public NinjaTabi(Serial serial) : base(serial)
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

	/// SamuraiTabi
	[Flipable(0x2796, 0x27E1)]
	public class SamuraiTabi : BaseShoes
	{
		[Constructable]
		public SamuraiTabi() : this(0)
		{
		}

		[Constructable]
		public SamuraiTabi(int hue) : base(0x2796, hue)
		{
			Weight = 2.0;
		}

		public SamuraiTabi(Serial serial) : base(serial)
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

	/// Waraji
	[Flipable(0x2796, 0x27E1)]
	public class Waraji : BaseShoes
	{
		[Constructable]
		public Waraji() : this(0)
		{
		}

		[Constructable]
		public Waraji(int hue) : base(0x2796, hue)
		{
			Weight = 2.0;
		}

		public Waraji(Serial serial) : base(serial)
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