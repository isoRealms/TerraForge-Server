﻿using Server.Engines.Plants;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

using System;
using System.Collections;

namespace Server.Items
{
	public interface IHasQuantity
	{
		int Quantity { get; set; }
	}

	public interface IWaterSource : IHasQuantity
	{
	}

	public abstract class BaseBeverage : Item, IHasQuantity
	{
		private BeverageType m_Content;
		private int m_Quantity;
		private Mobile m_Poisoner;
		private Poison m_Poison;

		public override int LabelNumber
		{
			get
			{
				var num = BaseLabelNumber;

				if (IsEmpty || num == 0)
				{
					return EmptyLabelNumber;
				}

				return BaseLabelNumber + (int)m_Content;
			}
		}

		public virtual bool ShowQuantity => (MaxQuantity > 1);
		public virtual bool Fillable => true;
		public virtual bool Pourable => true;

		public virtual int EmptyLabelNumber => base.LabelNumber;
		public virtual int BaseLabelNumber => 0;

		public abstract int MaxQuantity { get; }

		public abstract int ComputeItemID();

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsEmpty => (m_Quantity <= 0);

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ContainsAlchohol => (!IsEmpty && m_Content != BeverageType.Milk && m_Content != BeverageType.Water);

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsFull => (m_Quantity >= MaxQuantity);

		[CommandProperty(AccessLevel.GameMaster)]
		public Poison Poison
		{
			get => m_Poison;
			set => m_Poison = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Poisoner
		{
			get => m_Poisoner;
			set => m_Poisoner = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public BeverageType Content
		{
			get => m_Content;
			set
			{
				m_Content = value;

				InvalidateProperties();

				var itemID = ComputeItemID();

				if (itemID > 0)
				{
					ItemID = itemID;
				}
				else
				{
					Delete();
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Quantity
		{
			get => m_Quantity;
			set
			{
				if (value < 0)
				{
					value = 0;
				}
				else if (value > MaxQuantity)
				{
					value = MaxQuantity;
				}

				m_Quantity = value;

				InvalidateProperties();

				var itemID = ComputeItemID();

				if (itemID > 0)
				{
					ItemID = itemID;
				}
				else
				{
					Delete();
				}
			}
		}

		public virtual int GetQuantityDescription()
		{
			var perc = (m_Quantity * 100) / MaxQuantity;

			if (perc <= 0)
			{
				return 1042975; // It's empty.
			}
			else if (perc <= 33)
			{
				return 1042974; // It's nearly empty.
			}
			else if (perc <= 66)
			{
				return 1042973; // It's half full.
			}
			else
			{
				return 1042972; // It's full.
			}
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (ShowQuantity)
			{
				list.Add(GetQuantityDescription());
			}
		}

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			if (ShowQuantity)
			{
				LabelTo(from, GetQuantityDescription());
			}
		}

		public virtual bool ValidateUse(Mobile from, bool message)
		{
			if (Deleted)
			{
				return false;
			}

			if (!Movable && !Fillable)
			{
				var house = Multis.BaseHouse.FindHouseAt(this);

				if (house == null || !house.IsLockedDown(this))
				{
					if (message)
					{
						from.SendLocalizedMessage(502946, "", 0x59); // That belongs to someone else.
					}

					return false;
				}
			}

			if (from.Map != Map || !from.InRange(GetWorldLocation(), 2) || !from.InLOS(this))
			{
				if (message)
				{
					from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
				}

				return false;
			}

			return true;
		}

		public virtual void Fill_OnTarget(Mobile from, object targ)
		{
			if (!IsEmpty || !Fillable || !ValidateUse(from, false))
			{
				return;
			}

			if (targ is BaseBeverage)
			{
				var bev = (BaseBeverage)targ;

				if (bev.IsEmpty || !bev.ValidateUse(from, true))
				{
					return;
				}

				Content = bev.Content;
				Poison = bev.Poison;
				Poisoner = bev.Poisoner;

				if (bev.Quantity > MaxQuantity)
				{
					Quantity = MaxQuantity;
					bev.Quantity -= MaxQuantity;
				}
				else
				{
					Quantity += bev.Quantity;
					bev.Quantity = 0;
				}
			}
			else if (targ is BaseWaterContainer)
			{
				var bwc = targ as BaseWaterContainer;

				if (Quantity == 0 || (Content == BeverageType.Water && !IsFull))
				{
					var iNeed = Math.Min((MaxQuantity - Quantity), bwc.Quantity);

					if (iNeed > 0 && !bwc.IsEmpty && !IsFull)
					{
						bwc.Quantity -= iNeed;
						Quantity += iNeed;
						Content = BeverageType.Water;

						from.PlaySound(0x4E);
					}
				}
			}
			else if (targ is Item)
			{
				var item = (Item)targ;
				IWaterSource src;

				src = (item as IWaterSource);

				if (src == null && item is AddonComponent)
				{
					src = (((AddonComponent)item).Addon as IWaterSource);
				}

				if (src == null || src.Quantity <= 0)
				{
					return;
				}

				if (from.Map != item.Map || !from.InRange(item.GetWorldLocation(), 2) || !from.InLOS(item))
				{
					from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
					return;
				}

				Content = BeverageType.Water;
				Poison = null;
				Poisoner = null;

				if (src.Quantity > MaxQuantity)
				{
					Quantity = MaxQuantity;
					src.Quantity -= MaxQuantity;
				}
				else
				{
					Quantity += src.Quantity;
					src.Quantity = 0;
				}

				from.SendLocalizedMessage(1010089); // You fill the container with water.
			}
			else if (targ is Cow)
			{
				var cow = (Cow)targ;

				if (cow.TryMilk(from))
				{
					Content = BeverageType.Milk;
					Quantity = MaxQuantity;
					from.SendLocalizedMessage(1080197); // You fill the container with milk.
				}
			}
		}

		private static readonly int[] m_SwampTiles = new int[]
			{
				0x9C4, 0x9EB,
				0x3D65, 0x3D65,
				0x3DC0, 0x3DD9,
				0x3DDB, 0x3DDC,
				0x3DDE, 0x3EF0,
				0x3FF6, 0x3FF6,
				0x3FFC, 0x3FFE,
			};

		#region Effects of achohol
		private static readonly Hashtable m_Table = new Hashtable();

		public static void Initialize()
		{
			EventSink.Login += new LoginEventHandler(EventSink_Login);
		}

		private static void EventSink_Login(LoginEventArgs e)
		{
			CheckHeaveTimer(e.Mobile);
		}

		public static void CheckHeaveTimer(Mobile from)
		{
			if (from.BAC > 0 && from.Map != Map.Internal && !from.Deleted)
			{
				var t = (Timer)m_Table[from];

				if (t == null)
				{
					if (from.BAC > 60)
					{
						from.BAC = 60;
					}

					t = new HeaveTimer(from);
					t.Start();

					m_Table[from] = t;
				}
			}
			else
			{
				var t = (Timer)m_Table[from];

				if (t != null)
				{
					t.Stop();
					m_Table.Remove(from);

					from.SendLocalizedMessage(500850); // You feel sober.
				}
			}
		}

		private class HeaveTimer : Timer
		{
			private readonly Mobile m_Drunk;

			public HeaveTimer(Mobile drunk)
				: base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
			{
				m_Drunk = drunk;

				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				if (m_Drunk.Deleted || m_Drunk.Map == Map.Internal)
				{
					Stop();
					m_Table.Remove(m_Drunk);
				}
				else if (m_Drunk.Alive)
				{
					if (m_Drunk.BAC > 60)
					{
						m_Drunk.BAC = 60;
					}

					// chance to get sober
					if (10 > Utility.Random(100))
					{
						--m_Drunk.BAC;
					}

					// lose some stats
					m_Drunk.Stam -= 1;
					m_Drunk.Mana -= 1;

					if (Utility.Random(1, 4) == 1)
					{
						if (!m_Drunk.Mounted)
						{
							// turn in a random direction
							m_Drunk.Direction = (Direction)Utility.Random(8);

							// heave
							m_Drunk.Animate(32, 5, 1, true, false, 0);
						}

						// *hic*
						m_Drunk.PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, 500849);
					}

					if (m_Drunk.BAC <= 0)
					{
						Stop();
						m_Table.Remove(m_Drunk);

						m_Drunk.SendLocalizedMessage(500850); // You feel sober.
					}
				}
			}
		}

		#endregion

		public virtual void Pour_OnTarget(Mobile from, object targ)
		{
			if (IsEmpty || !Pourable || !ValidateUse(from, false))
			{
				return;
			}

			if (targ is BaseBeverage)
			{
				var bev = (BaseBeverage)targ;

				if (!bev.ValidateUse(from, true))
				{
					return;
				}

				if (bev.IsFull && bev.Content == Content)
				{
					from.SendLocalizedMessage(500848); // Couldn't pour it there.  It was already full.
				}
				else if (!bev.IsEmpty)
				{
					from.SendLocalizedMessage(500846); // Can't pour it there.
				}
				else
				{
					bev.Content = Content;
					bev.Poison = Poison;
					bev.Poisoner = Poisoner;

					if (Quantity > bev.MaxQuantity)
					{
						bev.Quantity = bev.MaxQuantity;
						Quantity -= bev.MaxQuantity;
					}
					else
					{
						bev.Quantity += Quantity;
						Quantity = 0;
					}

					from.PlaySound(0x4E);
				}
			}
			else if (from == targ)
			{
				if (from.Thirst < 20)
				{
					from.Thirst += 1;
				}

				if (ContainsAlchohol)
				{
					var bac = 0;

					switch (Content)
					{
						case BeverageType.Ale: bac = 1; break;
						case BeverageType.Wine: bac = 2; break;
						case BeverageType.Cider: bac = 3; break;
						case BeverageType.Liquor: bac = 4; break;
					}

					from.BAC += bac;

					if (from.BAC > 60)
					{
						from.BAC = 60;
					}

					CheckHeaveTimer(from);
				}

				from.PlaySound(Utility.RandomList(0x30, 0x2D6));

				if (m_Poison != null)
				{
					from.ApplyPoison(m_Poisoner, m_Poison);
				}

				--Quantity;
			}
			else if (targ is BaseWaterContainer)
			{
				var bwc = targ as BaseWaterContainer;

				if (Content != BeverageType.Water)
				{
					from.SendLocalizedMessage(500842); // Can't pour that in there.
				}
				else if (bwc.Items.Count != 0)
				{
					from.SendLocalizedMessage(500841); // That has something in it.
				}
				else
				{
					var itNeeds = Math.Min((bwc.MaxQuantity - bwc.Quantity), Quantity);

					if (itNeeds > 0)
					{
						bwc.Quantity += itNeeds;
						Quantity -= itNeeds;

						from.PlaySound(0x4E);
					}
				}
			}
			else if (targ is PlantItem)
			{
				((PlantItem)targ).Pour(from, this);
			}
			else
			{
				from.SendLocalizedMessage(500846); // Can't pour it there.
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (IsEmpty)
			{
				if (!Fillable || !ValidateUse(from, true))
				{
					return;
				}

				from.BeginTarget(-1, true, TargetFlags.None, new TargetCallback(Fill_OnTarget));
				SendLocalizedMessageTo(from, 500837); // Fill from what?
			}
			else if (Pourable && ValidateUse(from, true))
			{
				from.BeginTarget(-1, true, TargetFlags.None, new TargetCallback(Pour_OnTarget));
				from.SendLocalizedMessage(1010086); // What do you want to use this on?
			}
		}

		public static bool ConsumeTotal(Container pack, BeverageType content, int quantity)
		{
			return ConsumeTotal(pack, typeof(BaseBeverage), content, quantity);
		}

		public static bool ConsumeTotal(Container pack, Type itemType, BeverageType content, int quantity)
		{
			var items = pack.FindItemsByType(itemType);

			// First pass, compute total
			var total = 0;

			for (var i = 0; i < items.Length; ++i)
			{
				var bev = items[i] as BaseBeverage;

				if (bev != null && bev.Content == content && !bev.IsEmpty)
				{
					total += bev.Quantity;
				}
			}

			if (total >= quantity)
			{
				// We've enough, so consume it

				var need = quantity;

				for (var i = 0; i < items.Length; ++i)
				{
					var bev = items[i] as BaseBeverage;

					if (bev == null || bev.Content != content || bev.IsEmpty)
					{
						continue;
					}

					var theirQuantity = bev.Quantity;

					if (theirQuantity < need)
					{
						bev.Quantity = 0;
						need -= theirQuantity;
					}
					else
					{
						bev.Quantity -= need;
						return true;
					}
				}
			}

			return false;
		}

		public BaseBeverage()
		{
			ItemID = ComputeItemID();
		}

		public BaseBeverage(BeverageType type)
		{
			m_Content = type;
			m_Quantity = MaxQuantity;
			ItemID = ComputeItemID();
		}

		public BaseBeverage(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			writer.Write(m_Poisoner);

			Poison.Serialize(m_Poison, writer);
			writer.Write((int)m_Content);
			writer.Write(m_Quantity);
		}

		protected bool CheckType(string name)
		{
			return (World.LoadingType == String.Format("Server.Items.{0}", name));
		}

		public override void Deserialize(GenericReader reader)
		{
			InternalDeserialize(reader, true);
		}

		protected void InternalDeserialize(GenericReader reader, bool read)
		{
			base.Deserialize(reader);

			if (!read)
			{
				return;
			}

			var version = reader.ReadInt();

			switch (version)
			{
				case 1:
					{
						m_Poisoner = reader.ReadMobile();
						goto case 0;
					}
				case 0:
					{
						m_Poison = Poison.Deserialize(reader);
						m_Content = (BeverageType)reader.ReadInt();
						m_Quantity = reader.ReadInt();
						break;
					}
			}
		}
	}
}