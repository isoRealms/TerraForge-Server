﻿namespace Server.Items
{
	public abstract class BaseIngot : Item, ICommodity
	{
		private CraftResource m_Resource;

		[CommandProperty(AccessLevel.GameMaster)]
		public CraftResource Resource
		{
			get => m_Resource;
			set { m_Resource = value; InvalidateProperties(); }
		}

		public override double DefaultWeight => 0.1;

		int ICommodity.DescriptionNumber => LabelNumber;
		bool ICommodity.IsDeedable => true;

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			writer.Write((int)m_Resource);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();

			switch (version)
			{
				case 1:
					{
						m_Resource = (CraftResource)reader.ReadInt();
						break;
					}
				case 0:
					{
						OreInfo info;

						switch (reader.ReadInt())
						{
							case 0: info = OreInfo.Iron; break;
							case 1: info = OreInfo.DullCopper; break;
							case 2: info = OreInfo.ShadowIron; break;
							case 3: info = OreInfo.Copper; break;
							case 4: info = OreInfo.Bronze; break;
							case 5: info = OreInfo.Gold; break;
							case 6: info = OreInfo.Agapite; break;
							case 7: info = OreInfo.Verite; break;
							case 8: info = OreInfo.Valorite; break;
							default: info = null; break;
						}

						m_Resource = CraftResources.GetFromOreInfo(info);
						break;
					}
			}
		}

		public BaseIngot(CraftResource resource) : this(resource, 1)
		{
		}

		public BaseIngot(CraftResource resource, int amount) : base(0x1BF2)
		{
			Stackable = true;
			Amount = amount;
			Hue = CraftResources.GetHue(resource);

			m_Resource = resource;
		}

		public BaseIngot(Serial serial) : base(serial)
		{
		}

		public override void AddNameProperty(ObjectPropertyList list)
		{
			if (Amount > 1)
			{
				list.Add(1050039, "{0}\t#{1}", Amount, 1027154); // ~1_NUMBER~ ~2_ITEMNAME~
			}
			else
			{
				list.Add(1027154); // ingots
			}
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (!CraftResources.IsStandard(m_Resource))
			{
				var num = CraftResources.GetLocalizationNumber(m_Resource);

				if (num > 0)
				{
					list.Add(num);
				}
				else
				{
					list.Add(CraftResources.GetName(m_Resource));
				}
			}
		}

		public override int LabelNumber
		{
			get
			{
				if (m_Resource >= CraftResource.DullCopper && m_Resource <= CraftResource.Valorite)
				{
					return 1042684 + (m_Resource - CraftResource.DullCopper);
				}

				return 1042692;
			}
		}
	}
}