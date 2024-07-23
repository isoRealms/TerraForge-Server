﻿using Server.Items;

namespace Server.Mobiles
{
	[CorpseName("a skeletal corpse")]
	public class BoneKnight : BaseCreature
	{
		[Constructable]
		public BoneKnight() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = "a bone knight";
			Body = 57;
			BaseSoundID = 451;

			SetStr(196, 250);
			SetDex(76, 95);
			SetInt(36, 60);

			SetHits(118, 150);

			SetDamage(8, 18);

			SetDamageType(ResistanceType.Physical, 40);
			SetDamageType(ResistanceType.Cold, 60);

			SetResistance(ResistanceType.Physical, 35, 45);
			SetResistance(ResistanceType.Fire, 20, 30);
			SetResistance(ResistanceType.Cold, 50, 60);
			SetResistance(ResistanceType.Poison, 20, 30);
			SetResistance(ResistanceType.Energy, 30, 40);

			SetSkill(SkillName.MagicResist, 65.1, 80.0);
			SetSkill(SkillName.Tactics, 85.1, 100.0);
			SetSkill(SkillName.Wrestling, 85.1, 95.0);

			Fame = 3000;
			Karma = -3000;

			VirtualArmor = 40;

			switch (Utility.Random(6))
			{
				case 0: PackItem(new PlateArms()); break;
				case 1: PackItem(new PlateChest()); break;
				case 2: PackItem(new PlateGloves()); break;
				case 3: PackItem(new PlateGorget()); break;
				case 4: PackItem(new PlateLegs()); break;
				case 5: PackItem(new PlateHelm()); break;
			}

			PackSlayer();
			PackItem(new Scimitar());
			PackItem(new WoodenShield());
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.Average);
			AddLoot(LootPack.Meager);
		}

		public override bool BleedImmune => true;

		public BoneKnight(Serial serial) : base(serial)
		{
		}

		public override OppositionGroup OppositionGroup => OppositionGroup.FeyAndUndead;

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			var version = reader.ReadInt();
		}
	}
}