﻿using Server.Accounting;
using Server.ContextMenus;
using Server.Engines.CannedEvil;
using Server.Engines.ConPVP;
using Server.Engines.Craft;
using Server.Engines.Help;
using Server.Engines.PartySystem;
using Server.Factions;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Multis;
using Server.Network;
using Server.Regions;
using Server.Spells;
using Server.Spells.Bushido;
using Server.Spells.Magery;
using Server.Spells.Mysticism;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.Racial;
using Server.Spells.Spellweaving;
using Server.Targeting;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
	#region Enums

	[Flags]
	public enum PlayerFlag // First 16 bits are reserved for default-distro use, start custom flags at 0x00010000
	{
		None = 0x00000000,
		Glassblowing = 0x00000001,
		Masonry = 0x00000002,
		SandMining = 0x00000004,
		StoneMining = 0x00000008,
		ToggleMiningStone = 0x00000010,
		KarmaLocked = 0x00000020,
		AutoRenewInsurance = 0x00000040,
		UseOwnFilter = 0x00000080,
		PublicMyRunUO = 0x00000100,
		PagingSquelched = 0x00000200,
		Young = 0x00000400,
		AcceptGuildInvites = 0x00000800,
		DisplayChampionTitle = 0x00001000,
		HasStatReward = 0x00002000,
		RefuseTrades = 0x00004000
	}

	public enum SolenFriendship
	{
		None,
		Red,
		Black
	}

	public enum BlockMountType
	{
		None = -1,
		Dazed = 1040024,
		BolaRecovery = 1062910,
		DismountRecovery = 1070859
	}

	#endregion

	public interface FacetEditingQuery
	{
		int QueryMobile(Mobile m, int previousBlock);
	}

	public partial class PlayerMobile : Mobile, IHonorTarget
	{
		public static FacetEditingQuery BlockQuery;
		private int m_PreviousMapBlock = -1;

		#region Stygian Abyss
		public override bool CanBeginFlight()
		{
			if (Frozen)
			{
				SendLocalizedMessage(1060170); // You cannot use this ability while frozen.
				return false;
			}

			return base.CanBeginFlight();
		}

		public override bool CanEndFlight()
		{
			if (!base.CanEndFlight())
			{
				LocalOverheadMessage(MessageType.Regular, 0x3B2, 1113081); // You may not land here.
				return false;
			}

			return true;
		}

		protected override void OnFlyingChange()
		{
			if (Spell?.IsCasting == true)
			{
				Spell.Interrupt(SpellInterrupt.Unspecified);
			}

			base.OnFlyingChange();

			if (Flying)
			{
				SendSpeedControl(SpeedControlType.MountSpeed);
				BuffInfo.AddBuff(this, new BuffInfo(BuffIcon.Fly, 1112193, 1112567)); // Flying & You are flying.
			}
			else
			{
				SendSpeedControl(SpeedControlType.Disable);
				BuffInfo.RemoveBuff(this, BuffIcon.Fly);
			}
		}
		#endregion

		private class CountAndTimeStamp
		{
			private int m_Count;
			private DateTime m_Stamp;

			public CountAndTimeStamp()
			{
			}

			public DateTime TimeStamp => m_Stamp;
			public int Count
			{
				get => m_Count;
				set { m_Count = value; m_Stamp = DateTime.UtcNow; }
			}
		}

		private DesignContext m_DesignContext;

		private NpcGuild m_NpcGuild;
		private DateTime m_NpcGuildJoinTime;
		private DateTime m_NextBODTurnInTime;
		private TimeSpan m_NpcGuildGameTime;
		private PlayerFlag m_Flags;
		private int m_StepsTaken;
		private int m_Profession;
		private bool m_IsStealthing; // IsStealthing should be moved to Server.Mobiles
		private bool m_IgnoreMobiles; // IgnoreMobiles should be moved to Server.Mobiles
		private int m_NonAutoreinsuredItems; // number of items that could not be automatically reinsured because gold in bank was not enough
		private bool m_NinjaWepCooldown;
		/*
		 * a value of zero means, that the mobile is not executing the spell. Otherwise,
		 * the value should match the BaseMana required
		*/
		private int m_ExecutesLightningStrike; // move to Server.Mobiles??

		private DateTime m_LastOnline;
		private Guilds.RankDefinition m_GuildRank;

		private int m_GuildMessageHue, m_AllianceMessageHue;

		private List<Mobile> m_AutoStabled;
		private List<Mobile> m_AllFollowers;
		private List<Mobile> m_RecentlyReported;

		private DateTime m_PromoGiftLast;
		private DateTime m_LastTimePaged;

		#region Currency
		[CommandProperty(AccessLevel.GameMaster)]
		public double AccountCurrency
		{
			get
			{
				IGoldAccount acct = Account;

				if (acct != null)
				{
					return acct.TotalCurrency;
				}

				return 0;
			}
			set
			{
				IGoldAccount acct = Account;

				if (acct != null)
				{
					acct.SetCurrency(value);
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int AccountGold
		{
			get
			{
				IGoldAccount acct = Account;

				if (acct != null)
				{
					return acct.TotalGold;
				}

				return 0;
			}
			set
			{
				IGoldAccount acct = Account;

				if (acct != null)
				{
					acct.SetGold(value);
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int AccountPlat
		{
			get
			{
				IGoldAccount acct = Account;

				if (acct != null)
				{
					return acct.TotalPlat;
				}

				return 0;
			}
			set
			{
				IGoldAccount acct = Account;

				if (acct != null)
				{
					acct.SetPlat(value);
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int AccountSovereigns
		{
			get
			{
				var acct = Account as Account;

				if (acct != null)
				{
					return acct.Sovereigns;
				}

				return 0;
			}
			set
			{
				var acct = Account as Account;

				if (acct != null)
				{
					acct.SetSovereigns(value);
				}
			}
		}

		public bool DepositCurrency(double amount)
		{
			IGoldAccount acct = Account;

			if (acct != null)
			{
				return acct.DepositCurrency(amount);
			}

			return false;
		}

		public bool WithdrawCurrency(double amount)
		{
			IGoldAccount acct = Account;

			if (acct != null)
			{
				return acct.WithdrawCurrency(amount);
			}

			return false;
		}

		public bool DepositGold(int amount)
		{
			IGoldAccount acct = Account;

			if (acct != null)
			{
				return acct.DepositGold(amount);
			}

			return false;
		}

		public bool WithdrawGold(int amount)
		{
			IGoldAccount acct = Account;

			if (acct != null)
			{
				return acct.WithdrawGold(amount);
			}

			return false;
		}

		public bool DepositPlat(int amount)
		{
			IGoldAccount acct = Account;

			if (acct != null)
			{
				return acct.DepositPlat(amount);
			}

			return false;
		}

		public bool WithdrawPlat(int amount)
		{
			IGoldAccount acct = Account;

			if (acct != null)
			{
				return acct.WithdrawPlat(amount);
			}

			return false;
		}

		public bool DepositSovereigns(int amount)
		{
			var acct = Account as Account;

			if (acct != null)
			{
				return acct.DepositSovereigns(amount);
			}

			return false;
		}

		public bool WithdrawSovereigns(int amount)
		{
			var acct = Account as Account;

			if (acct != null)
			{
				return acct.WithdrawSovereigns(amount);
			}

			return false;
		}
		#endregion

		#region Getters & Setters

		public List<Mobile> RecentlyReported
		{
			get => m_RecentlyReported;
			set => m_RecentlyReported = value;
		}

		public List<Mobile> AutoStabled => m_AutoStabled;

		public bool NinjaWepCooldown
		{
			get => m_NinjaWepCooldown;
			set => m_NinjaWepCooldown = value;
		}

		public List<Mobile> AllFollowers
		{
			get
			{
				if (m_AllFollowers == null)
				{
					m_AllFollowers = new List<Mobile>();
				}

				return m_AllFollowers;
			}
		}

		public Server.Guilds.RankDefinition GuildRank
		{
			get
			{
				if (AccessLevel >= AccessLevel.GameMaster)
				{
					return Server.Guilds.RankDefinition.Leader;
				}
				else
				{
					return m_GuildRank;
				}
			}
			set => m_GuildRank = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int GuildMessageHue
		{
			get => m_GuildMessageHue;
			set => m_GuildMessageHue = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int AllianceMessageHue
		{
			get => m_AllianceMessageHue;
			set => m_AllianceMessageHue = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Profession
		{
			get => m_Profession;
			set => m_Profession = value;
		}

		public int StepsTaken
		{
			get => m_StepsTaken;
			set => m_StepsTaken = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsStealthing // IsStealthing should be moved to Server.Mobiles
		{
			get => m_IsStealthing;
			set => m_IsStealthing = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IgnoreMobiles // IgnoreMobiles should be moved to Server.Mobiles
		{
			get => m_IgnoreMobiles;
			set
			{
				if (m_IgnoreMobiles != value)
				{
					m_IgnoreMobiles = value;
					Delta(MobileDelta.Flags);
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public NpcGuild NpcGuild
		{
			get => m_NpcGuild;
			set => m_NpcGuild = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime NpcGuildJoinTime
		{
			get => m_NpcGuildJoinTime;
			set => m_NpcGuildJoinTime = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan NpcGuildGameTime
		{
			get => m_NpcGuildGameTime;
			set => m_NpcGuildGameTime = value;
		}

		[CommandProperty(AccessLevel.GameMaster, true)]
		public NpcGuildInfo NpcGuildInfo => NpcGuilds.Find(this);

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime NextBODTurnInTime
		{
			get => m_NextBODTurnInTime;
			set => m_NextBODTurnInTime = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastOnline
		{
			get => m_LastOnline;
			set => m_LastOnline = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public long LastMoved => LastMoveTime;

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime PromoGiftLast
		{
			get => m_PromoGiftLast;
			set => m_PromoGiftLast = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastTimePaged
		{
			get => m_LastTimePaged;
			set => m_LastTimePaged = value;
		}

		private int m_ToTItemsTurnedIn;

		[CommandProperty(AccessLevel.GameMaster)]
		public int ToTItemsTurnedIn
		{
			get => m_ToTItemsTurnedIn;
			set => m_ToTItemsTurnedIn = value;
		}

		private int m_ToTTotalMonsterFame;

		[CommandProperty(AccessLevel.GameMaster)]
		public int ToTTotalMonsterFame
		{
			get => m_ToTTotalMonsterFame;
			set => m_ToTTotalMonsterFame = value;
		}

		public int ExecutesLightningStrike
		{
			get => m_ExecutesLightningStrike;
			set => m_ExecutesLightningStrike = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int ToothAche
		{
			get => CandyCane.GetToothAche(this);
			set => CandyCane.SetToothAche(this, value);
		}

		#endregion

		#region PlayerFlags
		public PlayerFlag Flags
		{
			get => m_Flags;
			set => m_Flags = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool PagingSquelched
		{
			get => GetFlag(PlayerFlag.PagingSquelched);
			set => SetFlag(PlayerFlag.PagingSquelched, value);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Glassblowing
		{
			get => GetFlag(PlayerFlag.Glassblowing);
			set => SetFlag(PlayerFlag.Glassblowing, value);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Masonry
		{
			get => GetFlag(PlayerFlag.Masonry);
			set => SetFlag(PlayerFlag.Masonry, value);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool SandMining
		{
			get => GetFlag(PlayerFlag.SandMining);
			set => SetFlag(PlayerFlag.SandMining, value);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool StoneMining
		{
			get => GetFlag(PlayerFlag.StoneMining);
			set => SetFlag(PlayerFlag.StoneMining, value);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ToggleMiningStone
		{
			get => GetFlag(PlayerFlag.ToggleMiningStone);
			set => SetFlag(PlayerFlag.ToggleMiningStone, value);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool KarmaLocked
		{
			get => GetFlag(PlayerFlag.KarmaLocked);
			set => SetFlag(PlayerFlag.KarmaLocked, value);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool AutoRenewInsurance
		{
			get => GetFlag(PlayerFlag.AutoRenewInsurance);
			set => SetFlag(PlayerFlag.AutoRenewInsurance, value);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool UseOwnFilter
		{
			get => GetFlag(PlayerFlag.UseOwnFilter);
			set => SetFlag(PlayerFlag.UseOwnFilter, value);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool PublicMyRunUO
		{
			get => GetFlag(PlayerFlag.PublicMyRunUO);
			set { SetFlag(PlayerFlag.PublicMyRunUO, value); InvalidateMyRunUO(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool AcceptGuildInvites
		{
			get => GetFlag(PlayerFlag.AcceptGuildInvites);
			set => SetFlag(PlayerFlag.AcceptGuildInvites, value);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool HasStatReward
		{
			get => GetFlag(PlayerFlag.HasStatReward);
			set => SetFlag(PlayerFlag.HasStatReward, value);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool RefuseTrades
		{
			get => GetFlag(PlayerFlag.RefuseTrades);
			set => SetFlag(PlayerFlag.RefuseTrades, value);
		}
		#endregion

		#region Auto Arrow Recovery
		private readonly Dictionary<Type, int> m_RecoverableAmmo = new Dictionary<Type, int>();

		public Dictionary<Type, int> RecoverableAmmo => m_RecoverableAmmo;

		public void RecoverAmmo()
		{
			if (Core.SE && Alive)
			{
				foreach (var kvp in m_RecoverableAmmo)
				{
					if (kvp.Value > 0)
					{
						Item ammo = null;

						try
						{
							ammo = Activator.CreateInstance(kvp.Key) as Item;
						}
						catch
						{
						}

						if (ammo != null)
						{
							var name = ammo.Name;
							ammo.Amount = kvp.Value;

							if (name == null)
							{
								if (ammo is Arrow)
								{
									name = "arrow";
								}
								else if (ammo is Bolt)
								{
									name = "bolt";
								}
							}

							if (name != null && ammo.Amount > 1)
							{
								name = String.Format("{0}s", name);
							}

							if (name == null)
							{
								name = String.Format("#{0}", ammo.LabelNumber);
							}

							PlaceInBackpack(ammo);
							SendLocalizedMessage(1073504, String.Format("{0}\t{1}", ammo.Amount, name)); // You recover ~1_NUM~ ~2_AMMO~.
						}
					}
				}

				m_RecoverableAmmo.Clear();
			}
		}

		#endregion

		private DateTime m_AnkhNextUse;

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime AnkhNextUse
		{
			get => m_AnkhNextUse;
			set => m_AnkhNextUse = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan DisguiseTimeLeft => DisguiseTimers.TimeRemaining(this);

		private DateTime m_PeacedUntil;

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime PeacedUntil
		{
			get => m_PeacedUntil;
			set => m_PeacedUntil = value;
		}

		#region Scroll of Alacrity
		private DateTime m_AcceleratedStart;

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime AcceleratedStart
		{
			get => m_AcceleratedStart;
			set => m_AcceleratedStart = value;
		}

		private SkillName m_AcceleratedSkill;

		[CommandProperty(AccessLevel.GameMaster)]
		public SkillName AcceleratedSkill
		{
			get => m_AcceleratedSkill;
			set => m_AcceleratedSkill = value;
		}
		#endregion

		#region Home Town

		public static bool HomeTownsEnabled { get; set; } = true;
		public static bool HomeTownCheckAtLogin { get; set; } = true;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public Town HomeTown { get; set; }

		public void CheckHomeTown(bool requestChange)
		{
			if (HomeTownsEnabled && NetState?.Running == true && (HomeTown == null || requestChange))
			{
				SendGump(new HomeTownGump(this, 0, Town.Towns));
			}
		}

		public string ApplyTownSuffix(string suffix)
		{
			if (HomeTownsEnabled && HomeTown != null)
			{
				if (suffix?.Length > 0)
				{
					suffix = $"{suffix} of {HomeTown.Definition.FriendlyName}";
				}
				else
				{
					suffix = $"of {HomeTown.Definition.FriendlyName}";
				}
			}

			return suffix;
		}

		public sealed class HomeTownGump : BaseGridGump
		{
			private const int EntriesPerPage = 10;

			private readonly PlayerMobile m_Player;

			private readonly int m_Page;

			private readonly List<Town> m_Towns;

			public override int BorderSize => 10;
			public override int OffsetSize => 1;

			public override int EntryHeight => 20;

			public override int OffsetGumpID => 0x2430;
			public override int HeaderGumpID => 0x243A;
			public override int EntryGumpID => 0x2458;
			public override int BackGumpID => 0x2486;

			public override int TextHue => 0;
			public override int TextOffsetX => 2;
			
			public const int PageLeftID1 = 0x25EA;
			public const int PageLeftID2 = 0x25EB;
			public const int PageLeftWidth = 16;
			public const int PageLeftHeight = 16;

			public const int PageRightID1 = 0x25E6;
			public const int PageRightID2 = 0x25E7;
			public const int PageRightWidth = 16;
			public const int PageRightHeight = 16;
			
			public HomeTownGump(PlayerMobile player, int page, List<Town> towns) 
				: base(100, 100)
			{
				m_Player = player;
				m_Page = page;
				m_Towns = towns;

				Closable = false;
				Disposable = false;
				Dragable = false;
				Resizable = false;

				AddNewPage();

				AddEntryHtml(20 + OffsetSize + 160 + 20, SetCenter("Home Town Declaration"));

				AddNewLine();

				if (m_Page > 0)
				{
					AddEntryButton(20, PageLeftID1, PageLeftID2, 1, PageLeftWidth, PageLeftHeight);
				}
				else
				{
					AddEntryHeader(20);
				}

				AddEntryHtml(160, SetCenter($"Page {m_Page + 1} of {(m_Towns.Count + EntriesPerPage - 1) / EntriesPerPage}"));

				if ((m_Page + 1) * EntriesPerPage < m_Towns.Count)
				{
					AddEntryButton(20, PageRightID1, PageRightID2, 2, PageRightWidth, PageRightHeight);
				}
				else
				{
					AddEntryHeader(20);
				}

				for (int i = m_Page * EntriesPerPage, line = 0; line < EntriesPerPage && i < m_Towns.Count; ++i, ++line)
				{
					AddNewLine();

					AddEntryHtml(20 + OffsetSize + 160, m_Towns[i].Definition.FriendlyName);

					AddEntryButton(20, PageRightID1, PageRightID2, 3 + i, PageRightWidth, PageRightHeight);
				}

				FinishPage();
			}

			public override void OnResponse(NetState sender, RelayInfo info)
			{
				switch (info.ButtonID)
				{
					case 0:
						{
							m_Player.CloseGump(typeof(HomeTownGump));

							return;
						}
					case 1:
						{
							if (m_Page > 0)
							{
								m_Player.SendGump(new HomeTownGump(m_Player, m_Page - 1, m_Towns));
							}
							else
							{
								m_Player.SendGump(new HomeTownGump(m_Player, m_Page, m_Towns));
							}

							return;
						}
					case 2:
						{
							if ((m_Page + 1) * EntriesPerPage < m_Towns.Count)
							{
								m_Player.SendGump(new HomeTownGump(m_Player, m_Page + 1, m_Towns));
							}
							else
							{
								m_Player.SendGump(new HomeTownGump(m_Player, m_Page, m_Towns));
							}

							return;
						}
				}

				var v = info.ButtonID - 3;

				if (v >= 0 && v < m_Towns.Count)
				{
					var town = m_Towns[v];

					if (m_Player.HomeTown != town)
					{
						m_Player.HomeTown = town;

						m_Player.SendMessage($"You declare {town.Definition.FriendlyName} as your home town.");
					}
					else
					{
						m_Player.SendMessage($"{town.Definition.FriendlyName} is your home town.");
					}
				}
				else
				{
					m_Player.SendGump(new HomeTownGump(m_Player, m_Page, m_Towns));
				}
			}
		}

		#endregion

		public static Direction GetDirection4(Point3D from, Point3D to)
		{
			var dx = from.X - to.X;
			var dy = from.Y - to.Y;

			var rx = dx - dy;
			var ry = dx + dy;

			Direction ret;

			if (rx >= 0 && ry >= 0)
			{
				ret = Direction.West;
			}
			else if (rx >= 0 && ry < 0)
			{
				ret = Direction.South;
			}
			else if (rx < 0 && ry < 0)
			{
				ret = Direction.East;
			}
			else
			{
				ret = Direction.North;
			}

			return ret;
		}

		public override bool OnDroppedItemToWorld(Item item, Point3D location)
		{
			if (!base.OnDroppedItemToWorld(item, location))
			{
				return false;
			}

			if (Core.AOS)
			{
				IPooledEnumerable mobiles = Map.GetMobilesInRange(location, 0);

				foreach (Mobile m in mobiles)
				{
					if (m.Z >= location.Z && m.Z < location.Z + 16 && (!m.Hidden || m.AccessLevel == AccessLevel.Player))
					{
						mobiles.Free();
						return false;
					}
				}

				mobiles.Free();
			}

			var bi = item.GetBounce();

			if (bi != null)
			{
				var type = item.GetType();

				if (type.IsDefined(typeof(FurnitureAttribute), true) || type.IsDefined(typeof(DynamicFlipingAttribute), true))
				{
					var objs = type.GetCustomAttributes(typeof(FlipableAttribute), true);

					if (objs != null && objs.Length > 0)
					{
						var fp = objs[0] as FlipableAttribute;

						if (fp != null)
						{
							var itemIDs = fp.ItemIDs;

							var oldWorldLoc = bi.m_WorldLoc;
							var newWorldLoc = location;

							if (oldWorldLoc.X != newWorldLoc.X || oldWorldLoc.Y != newWorldLoc.Y)
							{
								var dir = GetDirection4(oldWorldLoc, newWorldLoc);

								if (itemIDs.Length == 2)
								{
									switch (dir)
									{
										case Direction.North:
										case Direction.South: item.ItemID = itemIDs[0]; break;
										case Direction.East:
										case Direction.West: item.ItemID = itemIDs[1]; break;
									}
								}
								else if (itemIDs.Length == 4)
								{
									switch (dir)
									{
										case Direction.South: item.ItemID = itemIDs[0]; break;
										case Direction.East: item.ItemID = itemIDs[1]; break;
										case Direction.North: item.ItemID = itemIDs[2]; break;
										case Direction.West: item.ItemID = itemIDs[3]; break;
									}
								}
							}
						}
					}
				}
			}

			return true;
		}

		public override int GetPacketFlags()
		{
			var flags = base.GetPacketFlags();

			if (m_IgnoreMobiles)
			{
				flags |= 0x10;
			}

			return flags;
		}

		public override int GetOldPacketFlags()
		{
			var flags = base.GetOldPacketFlags();

			if (m_IgnoreMobiles)
			{
				flags |= 0x10;
			}

			return flags;
		}

		public bool GetFlag(PlayerFlag flag)
		{
			return ((m_Flags & flag) != 0);
		}

		public void SetFlag(PlayerFlag flag, bool value)
		{
			if (value)
			{
				m_Flags |= flag;
			}
			else
			{
				m_Flags &= ~flag;
			}
		}

		public DesignContext DesignContext
		{
			get => m_DesignContext;
			set => m_DesignContext = value;
		}

		public static void Initialize()
		{
			if (FastwalkPrevention)
			{
				PacketHandlers.RegisterThrottler(0x02, new ThrottlePacketCallback(MovementThrottle_Callback));
			}

			EventSink.Login += new LoginEventHandler(OnLogin);
			EventSink.Logout += new LogoutEventHandler(OnLogout);
			EventSink.Connected += new ConnectedEventHandler(EventSink_Connected);
			EventSink.Disconnected += new DisconnectedEventHandler(EventSink_Disconnected);

			if (Core.SE)
			{
				Timer.DelayCall(TimeSpan.Zero, CheckPets);
			}
		}

		private static void CheckPets()
		{
			foreach (var m in World.Mobiles.Values)
			{
				if (m is PlayerMobile)
				{
					var pm = (PlayerMobile)m;

					if (((!pm.Mounted || (pm.Mount != null && pm.Mount is EtherealMount)) && (pm.AllFollowers.Count > pm.AutoStabled.Count)) ||
						(pm.Mounted && (pm.AllFollowers.Count > (pm.AutoStabled.Count + 1))))
					{
						pm.AutoStablePets(); /* autostable checks summons, et al: no need here */
					}
				}
			}
		}

		private MountBlock m_MountBlock;

		public BlockMountType MountBlockReason => (CheckBlock(m_MountBlock)) ? m_MountBlock.m_Type : BlockMountType.None;

		private static bool CheckBlock(MountBlock block)
		{
			return ((block is MountBlock) && block.m_Timer.Running);
		}

		private class MountBlock
		{
			public BlockMountType m_Type;
			public Timer m_Timer;

			public MountBlock(TimeSpan duration, BlockMountType type, Mobile mobile)
			{
				m_Type = type;

				m_Timer = Timer.DelayCall(duration, RemoveBlock, mobile);
			}

			private void RemoveBlock(Mobile mobile)
			{
				(mobile as PlayerMobile).m_MountBlock = null;
			}
		}

		public void SetMountBlock(BlockMountType type, TimeSpan duration, bool dismount)
		{
			if (dismount)
			{
				if (Mount != null)
				{
					Mount.Rider = null;
				}
				else if (AnimalFormSpell.UnderTransformation(this))
				{
					AnimalFormSpell.RemoveContext(this, true);
				}
			}

			if ((m_MountBlock == null) || !m_MountBlock.m_Timer.Running || (m_MountBlock.m_Timer.Next < (DateTime.UtcNow + duration)))
			{
				m_MountBlock = new MountBlock(duration, type, this);
			}
		}

		public override void OnSkillInvalidated(Skill skill)
		{
			if (Core.AOS && skill.SkillName == SkillName.MagicResist)
			{
				UpdateResistances();
			}
		}

		public override int GetMaxResistance(ResistanceType type)
		{
			if (AccessLevel > AccessLevel.Player)
			{
				return 100;
			}

			var max = base.GetMaxResistance(type);

			if (type != ResistanceType.Physical && 60 < max && CurseSpell.UnderEffect(this))
			{
				max = 60;
			}

			if (Core.ML && Race == Race.Elf && type == ResistanceType.Energy)
			{
				max += 5; //Intended to go after the 60 max from curse
			}

			return max;
		}

		protected override void OnRaceChange(Race oldRace)
		{
			base.OnRaceChange(oldRace);

			if (oldRace == Race.Gargoyle && Flying)
			{
				Flying = false;
			}
			else if (oldRace != Race.Gargoyle && Race == Race.Gargoyle && Mounted)
			{
				Mount.Rider = null;
			}

			ValidateEquipment();
			UpdateResistances();
		}

		public override int MaxWeight => (((Core.ML && Race == Race.Human) ? 100 : 40) + (int)(3.5 * Str));

		private int m_LastGlobalLight = -1, m_LastPersonalLight = -1;

		public override void OnNetStateChanged()
		{
			m_LastGlobalLight = -1;
			m_LastPersonalLight = -1;
		}

		public override void ComputeBaseLightLevels(out int global, out int personal)
		{
			global = LightCycle.ComputeLevelFor(this);

			var racialNightSight = (Core.ML && Race == Race.Elf);

			if (LightLevel < 21 && (AosAttributes.GetValue(this, AosAttribute.NightSight) > 0 || racialNightSight))
			{
				personal = 21;
			}
			else
			{
				personal = LightLevel;
			}
		}

		public override void CheckLightLevels(bool forceResend)
		{
			var ns = NetState;

			if (ns == null)
			{
				return;
			}

			int global, personal;

			ComputeLightLevels(out global, out personal);

			if (!forceResend)
			{
				forceResend = (global != m_LastGlobalLight || personal != m_LastPersonalLight);
			}

			if (!forceResend)
			{
				return;
			}

			m_LastGlobalLight = global;
			m_LastPersonalLight = personal;

			ns.Send(GlobalLightLevel.Instantiate(global));
			ns.Send(new PersonalLightLevel(this, personal));
		}

		public override int GetMinResistance(ResistanceType type)
		{
			var magicResist = (int)(Skills[SkillName.MagicResist].Value * 10);
			var min = Int32.MinValue;

			if (magicResist >= 1000)
			{
				min = 40 + ((magicResist - 1000) / 50);
			}
			else if (magicResist >= 400)
			{
				min = (magicResist - 400) / 15;
			}

			if (min > MaxPlayerResistance)
			{
				min = MaxPlayerResistance;
			}

			var baseMin = base.GetMinResistance(type);

			if (min < baseMin)
			{
				min = baseMin;
			}

			return min;
		}

		public override void OnManaChange(int oldValue)
		{
			base.OnManaChange(oldValue);
			if (m_ExecutesLightningStrike > 0)
			{
				if (Mana < m_ExecutesLightningStrike)
				{
					LightningStrikeAbility.ClearCurrentMove(this);
				}
			}
		}

		private static void OnLogin(LoginEventArgs e)
		{
			var from = e.Mobile;

			CheckAtrophies(from);

			if (AccountHandler.LockdownLevel > AccessLevel.Player)
			{
				string notice;

				var acct = from.Account as Accounting.Account;

				if (acct == null || !acct.HasAccess(from.NetState))
				{
					if (from.AccessLevel == AccessLevel.Player)
					{
						notice = "The server is currently under lockdown. No players are allowed to log in at this time.";
					}
					else
					{
						notice = "The server is currently under lockdown. You do not have sufficient access level to connect.";
					}

					Timer.DelayCall(TimeSpan.FromSeconds(1.0), Disconnect, from);
				}
				else if (from.AccessLevel >= AccessLevel.Administrator)
				{
					notice = "The server is currently under lockdown. As you are an administrator, you may change this from the [Admin gump.";
				}
				else
				{
					notice = "The server is currently under lockdown. You have sufficient access level to connect.";
				}

				from.SendGump(new NoticeGump(1060637, 30720, notice, 0xFFC000, 300, 140, null, null));
				return;
			}

			if (from is PlayerMobile player)
			{
				player.ClaimAutoStabledPets();

				if (HomeTownCheckAtLogin)
				{
					player.CheckHomeTown(false);
				}
			}
		}

		private bool m_NoDeltaRecursion;

		public void ValidateEquipment()
		{
			if (m_NoDeltaRecursion || Map == null || Map == Map.Internal)
			{
				return;
			}

			if (Items == null)
			{
				return;
			}

			m_NoDeltaRecursion = true;
			Timer.DelayCall(TimeSpan.Zero, ValidateEquipment_Sandbox);
		}

		private void ValidateEquipment_Sandbox()
		{
			try
			{
				if (Map == null || Map == Map.Internal)
				{
					return;
				}

				var items = Items;

				if (items == null)
				{
					return;
				}

				var moved = false;

				var str = Str;
				var dex = Dex;
				var intel = Int;

				#region Factions
				var factionItemCount = 0;
				#endregion

				Mobile from = this;

				#region Ethics
				var ethic = Ethics.Ethic.Find(from);
				#endregion

				for (var i = items.Count - 1; i >= 0; --i)
				{
					if (i >= items.Count)
					{
						continue;
					}

					var item = items[i];

					#region Ethics
					if ((item.SavedFlags & 0x100) != 0)
					{
						if (item.Hue != Ethics.Ethic.Hero.Definition.PrimaryHue)
						{
							item.SavedFlags &= ~0x100;
						}
						else if (ethic != Ethics.Ethic.Hero)
						{
							from.AddToBackpack(item);
							moved = true;
							continue;
						}
					}
					else if ((item.SavedFlags & 0x200) != 0)
					{
						if (item.Hue != Ethics.Ethic.Evil.Definition.PrimaryHue)
						{
							item.SavedFlags &= ~0x200;
						}
						else if (ethic != Ethics.Ethic.Evil)
						{
							from.AddToBackpack(item);
							moved = true;
							continue;
						}
					}
					#endregion

					if (item is BaseWeapon)
					{
						var weapon = (BaseWeapon)item;

						var drop = false;

						if (dex < weapon.DexRequirement)
						{
							drop = true;
						}
						else if (str < AOS.Scale(weapon.StrRequirement, 100 - weapon.GetLowerStatReq()))
						{
							drop = true;
						}
						else if (intel < weapon.IntRequirement)
						{
							drop = true;
						}
						else if (weapon.RequiredRace != null && weapon.RequiredRace != Race)
						{
							drop = true;
						}

						if (drop)
						{
							var name = weapon.Name;

							if (name == null)
							{
								name = String.Format("#{0}", weapon.LabelNumber);
							}

							from.SendLocalizedMessage(1062001, name); // You can no longer wield your ~1_WEAPON~
							from.AddToBackpack(weapon);
							moved = true;
						}
					}
					else if (item is BaseArmor)
					{
						var armor = (BaseArmor)item;

						var drop = false;

						if (!armor.AllowMaleWearer && !from.Female && from.AccessLevel < AccessLevel.GameMaster)
						{
							drop = true;
						}
						else if (!armor.AllowFemaleWearer && from.Female && from.AccessLevel < AccessLevel.GameMaster)
						{
							drop = true;
						}
						else if (armor.RequiredRace != null && armor.RequiredRace != Race)
						{
							drop = true;
						}
						else
						{
							int strBonus = armor.ComputeStatBonus(StatType.Str), strReq = armor.ComputeStatReq(StatType.Str);
							int dexBonus = armor.ComputeStatBonus(StatType.Dex), dexReq = armor.ComputeStatReq(StatType.Dex);
							int intBonus = armor.ComputeStatBonus(StatType.Int), intReq = armor.ComputeStatReq(StatType.Int);

							if (dex < dexReq || (dex + dexBonus) < 1)
							{
								drop = true;
							}
							else if (str < strReq || (str + strBonus) < 1)
							{
								drop = true;
							}
							else if (intel < intReq || (intel + intBonus) < 1)
							{
								drop = true;
							}
						}

						if (drop)
						{
							var name = armor.Name;

							if (name == null)
							{
								name = String.Format("#{0}", armor.LabelNumber);
							}

							if (armor is BaseShield)
							{
								from.SendLocalizedMessage(1062003, name); // You can no longer equip your ~1_SHIELD~
							}
							else
							{
								from.SendLocalizedMessage(1062002, name); // You can no longer wear your ~1_ARMOR~
							}

							from.AddToBackpack(armor);
							moved = true;
						}
					}
					else if (item is BaseClothing)
					{
						var clothing = (BaseClothing)item;

						var drop = false;

						if (!clothing.AllowMaleWearer && !from.Female && from.AccessLevel < AccessLevel.GameMaster)
						{
							drop = true;
						}
						else if (!clothing.AllowFemaleWearer && from.Female && from.AccessLevel < AccessLevel.GameMaster)
						{
							drop = true;
						}
						else if (clothing.RequiredRace != null && clothing.RequiredRace != Race)
						{
							drop = true;
						}
						else
						{
							var strBonus = clothing.ComputeStatBonus(StatType.Str);
							var strReq = clothing.ComputeStatReq(StatType.Str);

							if (str < strReq || (str + strBonus) < 1)
							{
								drop = true;
							}
						}

						if (drop)
						{
							var name = clothing.Name;

							if (name == null)
							{
								name = String.Format("#{0}", clothing.LabelNumber);
							}

							from.SendLocalizedMessage(1062002, name); // You can no longer wear your ~1_ARMOR~

							from.AddToBackpack(clothing);
							moved = true;
						}
					}

					var factionItem = FactionItem.Find(item);

					if (factionItem != null)
					{
						var drop = false;

						var ourFaction = Faction.Find(this);

						if (ourFaction == null || ourFaction != factionItem.Faction)
						{
							drop = true;
						}
						else if (++factionItemCount > FactionItem.GetMaxWearables(this))
						{
							drop = true;
						}

						if (drop)
						{
							from.AddToBackpack(item);
							moved = true;
						}
					}
				}

				if (moved)
				{
					from.SendLocalizedMessage(500647); // Some equipment has been moved to your backpack.
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			finally
			{
				m_NoDeltaRecursion = false;
			}
		}

		public override void Delta(MobileDelta flag)
		{
			base.Delta(flag);

			if ((flag & MobileDelta.Stat) != 0)
			{
				ValidateEquipment();
			}

			if ((flag & (MobileDelta.Name | MobileDelta.Hue)) != 0)
			{
				InvalidateMyRunUO();
			}
		}

		private static void Disconnect(object state)
		{
			var ns = ((Mobile)state).NetState;

			if (ns != null)
			{
				ns.Dispose();
			}
		}

		private static void OnLogout(LogoutEventArgs e)
		{
			if (e.Mobile is PlayerMobile)
			{
				((PlayerMobile)e.Mobile).AutoStablePets();
			}
		}

		private static void EventSink_Connected(ConnectedEventArgs e)
		{
			var pm = e.Mobile as PlayerMobile;

			if (pm != null)
			{
				pm.m_SessionStart = DateTime.UtcNow;

				pm.BedrollLogout = false;
				pm.LastOnline = DateTime.UtcNow;
			}

			DisguiseTimers.StartTimer(e.Mobile);

			Timer.DelayCall(TimeSpan.Zero, ClearSpecialMovesCallback, e.Mobile);
		}

		private static void ClearSpecialMovesCallback(object state)
		{
			var from = (Mobile)state;

			SpecialMove.ClearAllMoves(from);
		}

		private static void EventSink_Disconnected(DisconnectedEventArgs e)
		{
			var from = e.Mobile;
			var context = DesignContext.Find(from);

			if (context != null)
			{
				/* Client disconnected
				 *  - Remove design context
				 *  - Eject all from house
				 *  - Restore relocated entities
				 */

				// Remove design context
				DesignContext.Remove(from);

				// Eject all from house
				from.RevealingAction();

				foreach (var item in context.Foundation.GetItems())
				{
					item.Location = context.Foundation.BanLocation;
				}

				foreach (var mobile in context.Foundation.GetMobiles())
				{
					mobile.Location = context.Foundation.BanLocation;
				}

				// Restore relocated entities
				context.Foundation.RestoreRelocatedEntities();
			}

			var pm = e.Mobile as PlayerMobile;

			if (pm != null)
			{
				pm.m_GameTime += DateTime.UtcNow - pm.m_SessionStart;

				pm.m_SpeechLog = null;
				pm.LastOnline = DateTime.UtcNow;
			}

			DisguiseTimers.StopTimer(from);
		}

		public override void RevealingAction()
		{
			if (m_DesignContext != null)
			{
				return;
			}

			InvisibilitySpell.RemoveTimer(this);

			base.RevealingAction();

			m_IsStealthing = false; // IsStealthing should be moved to Server.Mobiles
		}

		public override void OnHiddenChanged()
		{
			base.OnHiddenChanged();

			RemoveBuff(BuffIcon.Invisibility);  //Always remove, default to the hiding icon EXCEPT in the invis spell where it's explicitly set

			if (!Hidden)
			{
				RemoveBuff(BuffIcon.HidingAndOrStealth);
			}
			else// if( !InvisibilitySpell.HasTimer( this ) )
			{
				BuffInfo.AddBuff(this, new BuffInfo(BuffIcon.HidingAndOrStealth, 1075655)); //Hidden/Stealthing & You Are Hidden
			}
		}

		public override void OnSubItemAdded(Item item)
		{
			if (AccessLevel < AccessLevel.GameMaster && item.IsChildOf(Backpack))
			{
				var maxWeight = WeightOverloading.GetMaxWeight(this);
				var curWeight = Mobile.BodyWeight + TotalWeight;

				if (curWeight > maxWeight)
				{
					SendLocalizedMessage(1019035, true, String.Format(" : {0} / {1}", curWeight, maxWeight));
				}
			}

			base.OnSubItemAdded(item);
		}

		public override bool CanBeHarmful(Mobile target, bool message, bool ignoreOurBlessedness)
		{
			if (m_DesignContext != null || (target is PlayerMobile && ((PlayerMobile)target).m_DesignContext != null))
			{
				return false;
			}

			if ((target is BaseCreature && ((BaseCreature)target).IsInvulnerable) || target is PlayerVendor || target is TownCrier)
			{
				if (message)
				{
					if (target.Title == null)
					{
						SendMessage("{0} cannot be harmed.", target.Name);
					}
					else
					{
						SendMessage("{0} {1} cannot be harmed.", target.Name, target.Title);
					}
				}

				return false;
			}

			return base.CanBeHarmful(target, message, ignoreOurBlessedness);
		}

		public override bool CanBeBeneficial(Mobile target, bool message, bool allowDead)
		{
			if (m_DesignContext != null || (target is PlayerMobile && ((PlayerMobile)target).m_DesignContext != null))
			{
				return false;
			}

			return base.CanBeBeneficial(target, message, allowDead);
		}

		public override bool CheckContextMenuDisplay(IEntity target)
		{
			return (m_DesignContext == null);
		}

		public override void OnItemAdded(Item item)
		{
			base.OnItemAdded(item);

			if (item is BaseArmor || item is BaseWeapon)
			{
				Hits = Hits; Stam = Stam; Mana = Mana;
			}

			if (NetState != null)
			{
				CheckLightLevels(false);
			}

			InvalidateMyRunUO();
		}

		public override void OnItemRemoved(Item item)
		{
			base.OnItemRemoved(item);

			if (item is BaseArmor || item is BaseWeapon)
			{
				Hits = Hits; Stam = Stam; Mana = Mana;
			}

			if (NetState != null)
			{
				CheckLightLevels(false);
			}

			InvalidateMyRunUO();
		}

		public override double ArmorRating
		{
			get
			{
				//BaseArmor ar;
				var rating = 0.0;

				AddArmorRating(ref rating, NeckArmor);
				AddArmorRating(ref rating, HandArmor);
				AddArmorRating(ref rating, HeadArmor);
				AddArmorRating(ref rating, ArmsArmor);
				AddArmorRating(ref rating, LegsArmor);
				AddArmorRating(ref rating, ChestArmor);
				AddArmorRating(ref rating, ShieldArmor);

				return VirtualArmor + VirtualArmorMod + rating;
			}
		}

		private void AddArmorRating(ref double rating, Item armor)
		{
			var ar = armor as BaseArmor;

			if (ar != null && (!Core.AOS || ar.ArmorAttributes.MageArmor == 0))
			{
				rating += ar.ArmorRatingScaled;
			}
		}

		#region [Stats]Max
		[CommandProperty(AccessLevel.GameMaster)]
		public override int HitsMax
		{
			get
			{
				int strBase;
				var strOffs = GetStatOffset(StatType.Str);

				if (Core.AOS)
				{
					strBase = Str; //this.Str already includes GetStatOffset/str
					strOffs = AosAttributes.GetValue(this, AosAttribute.BonusHits);

					if (Core.ML && strOffs > 25 && AccessLevel <= AccessLevel.Player)
					{
						strOffs = 25;
					}

					if (AnimalFormSpell.UnderTransformation(this, typeof(BakeKitsune)) || AnimalFormSpell.UnderTransformation(this, typeof(GreyWolf)))
					{
						strOffs += 20;
					}
				}
				else
				{
					strBase = RawStr;
				}

				return (strBase / 2) + 50 + strOffs;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public override int StamMax => base.StamMax + AosAttributes.GetValue(this, AosAttribute.BonusStam);

		[CommandProperty(AccessLevel.GameMaster)]
		public override int ManaMax => base.ManaMax + AosAttributes.GetValue(this, AosAttribute.BonusMana) + ((Core.ML && Race == Race.Elf) ? 20 : 0);
		#endregion

		#region Stat Getters/Setters

		[CommandProperty(AccessLevel.GameMaster)]
		public override int Str
		{
			get
			{
				if (Core.ML && AccessLevel == AccessLevel.Player)
				{
					return Math.Min(base.Str, 150);
				}

				return base.Str;
			}
			set => base.Str = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public override int Int
		{
			get
			{
				if (Core.ML && AccessLevel == AccessLevel.Player)
				{
					return Math.Min(base.Int, 150);
				}

				return base.Int;
			}
			set => base.Int = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public override int Dex
		{
			get
			{
				if (Core.ML && AccessLevel == AccessLevel.Player)
				{
					return Math.Min(base.Dex, 150);
				}

				return base.Dex;
			}
			set => base.Dex = value;
		}

		#endregion

		public override bool Move(Direction d)
		{
			var ns = NetState;

			if (ns != null)
			{
				if (HasGump(typeof(ResurrectGump)))
				{
					if (Alive)
					{
						CloseGump(typeof(ResurrectGump));
					}
					else
					{
						SendLocalizedMessage(500111); // You are frozen and cannot move.
						return false;
					}
				}
			}

			var speed = ComputeMovementSpeed(d);

			bool res;

			if (!Alive)
			{
				Server.Movement.MovementImpl.IgnoreMovableImpassables = true;
			}

			res = base.Move(d);

			Server.Movement.MovementImpl.IgnoreMovableImpassables = false;

			if (!res)
			{
				return false;
			}

			m_NextMovementTime += speed;

			return true;
		}

		public override bool CheckMovement(Direction d, out int newZ)
		{
			var context = m_DesignContext;

			if (context == null)
			{
				return base.CheckMovement(d, out newZ);
			}

			var foundation = context.Foundation;

			newZ = foundation.Z + HouseFoundation.GetLevelZ(context.Level, context.Foundation);

			int newX = X, newY = Y;
			Movement.Movement.Offset(d, ref newX, ref newY);

			var startX = foundation.X + foundation.Components.Min.X + 1;
			var startY = foundation.Y + foundation.Components.Min.Y + 1;
			var endX = startX + foundation.Components.Width - 1;
			var endY = startY + foundation.Components.Height - 2;

			return (newX >= startX && newY >= startY && newX < endX && newY < endY && Map == foundation.Map);
		}

		public override bool AllowItemUse(Item item)
		{
			#region Dueling
			if (m_DuelContext != null && !m_DuelContext.AllowItemUse(this, item))
			{
				return false;
			}
			#endregion

			return DesignContext.Check(this);
		}

		public SkillName[] AnimalFormRestrictedSkills => m_AnimalFormRestrictedSkills;

		private readonly SkillName[] m_AnimalFormRestrictedSkills = new SkillName[]
		{
			SkillName.ArmsLore, SkillName.Begging, SkillName.Discordance, SkillName.Forensics,
			SkillName.Inscribe, SkillName.ItemID, SkillName.Meditation, SkillName.Peacemaking,
			SkillName.Provocation, SkillName.RemoveTrap, SkillName.SpiritSpeak, SkillName.Stealing,
			SkillName.TasteID
		};

		public override bool AllowSkillUse(SkillName skill)
		{
			if (AnimalFormSpell.UnderTransformation(this))
			{
				for (var i = 0; i < m_AnimalFormRestrictedSkills.Length; i++)
				{
					if (m_AnimalFormRestrictedSkills[i] == skill)
					{
						SendLocalizedMessage(1070771); // You cannot use that skill in this form.
						return false;
					}
				}
			}

			#region Dueling
			if (m_DuelContext != null && !m_DuelContext.AllowSkillUse(this, skill))
			{
				return false;
			}
			#endregion

			return DesignContext.Check(this);
		}

		private bool m_LastProtectedMessage;
		private int m_NextProtectionCheck = 10;

		public virtual void RecheckTownProtection()
		{
			m_NextProtectionCheck = 10;

			var reg = Region.GetRegion<Regions.GuardedRegion>();
			var isProtected = (reg != null && !reg.Disabled);

			if (isProtected != m_LastProtectedMessage)
			{
				if (isProtected)
				{
					SendLocalizedMessage(500112); // You are now under the protection of the town guards.
				}
				else
				{
					SendLocalizedMessage(500113); // You have left the protection of the town guards.
				}

				m_LastProtectedMessage = isProtected;
			}
		}

		public override void MoveToWorld(Point3D loc, Map map)
		{
			base.MoveToWorld(loc, map);

			RecheckTownProtection();
		}

		public override void SetLocation(Point3D loc, bool isTeleport)
		{
			if (!isTeleport && !Flying && AccessLevel < AccessLevel.Counselor)
			{
				// moving, not teleporting
				var zDrop = (Location.Z - loc.Z);

				if (zDrop > 20) // we fell more than one story
				{
					Hits -= ((zDrop / 20) * 10) - 5; // deal some damage; does not kill, disrupt, etc
				}

				if (BlockQuery != null)
				{
					m_PreviousMapBlock = BlockQuery.QueryMobile(this, m_PreviousMapBlock);
				}
			}

			base.SetLocation(loc, isTeleport);

			if (isTeleport || --m_NextProtectionCheck == 0)
			{
				RecheckTownProtection();
			}
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			if (from == this)
			{
				if (Alive)
				{
					if (InsuranceEnabled)
					{
						if (Core.SA)
						{
							list.Add(new CallbackEntry(1114299, OpenItemInsuranceMenu)); // Open Item Insurance Menu
						}

						list.Add(new CallbackEntry(6201, ToggleItemInsurance)); // Toggle Item Insurance

						if (!Core.SA)
						{
							if (AutoRenewInsurance)
							{
								list.Add(new CallbackEntry(6202, CancelRenewInventoryInsurance)); // Cancel Renewing Inventory Insurance
							}
							else
							{
								list.Add(new CallbackEntry(6200, AutoRenewInventoryInsurance)); // Auto Renew Inventory Insurance
							}
						}
					}
				}

				var house = BaseHouse.FindHouseAt(this);

				if (house != null)
				{
					if (Alive && house.InternalizedVendors.Count > 0 && house.IsOwner(this))
					{
						list.Add(new CallbackEntry(6204, GetVendor));
					}

					if (house.IsAosRules && !Region.IsPartOf<SafeZone>()) // Dueling
					{
						list.Add(new CallbackEntry(6207, LeaveHouse));
					}
				}

				if (m_JusticeProtectors.Count > 0)
				{
					list.Add(new CallbackEntry(6157, CancelProtection));
				}

				if (Alive)
				{
					list.Add(new CallbackEntry(6210, ToggleChampionTitleDisplay));
				}

				if (Core.HS)
				{
					var ns = from.NetState;

					if (ns != null && ns.ExtendedStatus)
					{
						list.Add(new CallbackEntry(RefuseTrades ? 1154112 : 1154113, ToggleTrades)); // Allow Trades / Refuse Trades
					}
				}
			}
			else
			{
				if (Core.TOL && from.InRange(this, 2))
				{
					list.Add(new CallbackEntry(1077728, () => OpenTrade(from))); // Trade
				}

				if (Alive && Core.Expansion >= Expansion.AOS)
				{
					var theirParty = from.Party as Party;
					var ourParty = Party as Party;

					if (theirParty == null && ourParty == null)
					{
						list.Add(new AddToPartyEntry(from, this));
					}
					else if (theirParty != null && theirParty.Leader == from)
					{
						if (ourParty == null)
						{
							list.Add(new AddToPartyEntry(from, this));
						}
						else if (ourParty == theirParty)
						{
							list.Add(new RemoveFromPartyEntry(from, this));
						}
					}
				}

				var curhouse = BaseHouse.FindHouseAt(this);

				if (curhouse != null)
				{
					if (Alive && Core.Expansion >= Expansion.AOS && curhouse.IsAosRules && curhouse.IsFriend(from))
					{
						list.Add(new EjectPlayerEntry(from, this));
					}
				}
			}
		}

		private void CancelProtection()
		{
			for (var i = 0; i < m_JusticeProtectors.Count; ++i)
			{
				var prot = m_JusticeProtectors[i];

				var args = String.Format("{0}\t{1}", Name, prot.Name);

				prot.SendLocalizedMessage(1049371, args); // The protective relationship between ~1_PLAYER1~ and ~2_PLAYER2~ has been ended.
				SendLocalizedMessage(1049371, args); // The protective relationship between ~1_PLAYER1~ and ~2_PLAYER2~ has been ended.
			}

			m_JusticeProtectors.Clear();
		}

		#region Insurance

		private static int GetInsuranceCost(Item item)
		{
			return 600; // TODO
		}

		private void ToggleItemInsurance()
		{
			if (!CheckAlive())
			{
				return;
			}

			BeginTarget(-1, false, TargetFlags.None, new TargetCallback(ToggleItemInsurance_Callback));
			SendLocalizedMessage(1060868); // Target the item you wish to toggle insurance status on <ESC> to cancel
		}

		private bool CanInsure(Item item)
		{
			if ((item is Container && !(item is BaseQuiver)) || item is BagOfSending || item is KeyRing || item is PotionKeg || item is Sigil)
			{
				return false;
			}

			if (item.Stackable)
			{
				return false;
			}

			if (item.LootType == LootType.Cursed)
			{
				return false;
			}

			if (item.ItemID == 0x204E) // death shroud
			{
				return false;
			}

			if (item.Layer == Layer.Mount)
			{
				return false;
			}

			if (item.LootType == LootType.Blessed || item.LootType == LootType.Newbied || item.BlessedFor == this)
			{
				//SendLocalizedMessage( 1060870, "", 0x23 ); // That item is blessed and does not need to be insured
				return false;
			}

			return true;
		}

		private void ToggleItemInsurance_Callback(Mobile from, object obj)
		{
			if (!CheckAlive())
			{
				return;
			}

			ToggleItemInsurance_Callback(from, obj as Item, true);
		}

		private void ToggleItemInsurance_Callback(Mobile from, Item item, bool target)
		{
			if (item == null || !item.IsChildOf(this))
			{
				if (target)
				{
					BeginTarget(-1, false, TargetFlags.None, new TargetCallback(ToggleItemInsurance_Callback));
				}

				SendLocalizedMessage(1060871, "", 0x23); // You can only insure items that you have equipped or that are in your backpack
			}
			else if (item.Insured)
			{
				item.Insured = false;

				SendLocalizedMessage(1060874, "", 0x35); // You cancel the insurance on the item

				if (target)
				{
					BeginTarget(-1, false, TargetFlags.None, new TargetCallback(ToggleItemInsurance_Callback));
					SendLocalizedMessage(1060868, "", 0x23); // Target the item you wish to toggle insurance status on <ESC> to cancel
				}
			}
			else if (!CanInsure(item))
			{
				if (target)
				{
					BeginTarget(-1, false, TargetFlags.None, new TargetCallback(ToggleItemInsurance_Callback));
				}

				SendLocalizedMessage(1060869, "", 0x23); // You cannot insure that
			}
			else
			{
				if (!item.PayedInsurance)
				{
					var cost = GetInsuranceCost(item);

					if (Banker.Withdraw(from, cost))
					{
						SendLocalizedMessage(1060398, cost.ToString()); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
						item.PayedInsurance = true;
					}
					else
					{
						SendLocalizedMessage(1061079, "", 0x23); // You lack the funds to purchase the insurance
						return;
					}
				}

				item.Insured = true;

				SendLocalizedMessage(1060873, "", 0x23); // You have insured the item

				if (target)
				{
					BeginTarget(-1, false, TargetFlags.None, new TargetCallback(ToggleItemInsurance_Callback));
					SendLocalizedMessage(1060868, "", 0x23); // Target the item you wish to toggle insurance status on <ESC> to cancel
				}
			}
		}

		private void AutoRenewInventoryInsurance()
		{
			if (!CheckAlive())
			{
				return;
			}

			SendLocalizedMessage(1060881, "", 0x23); // You have selected to automatically reinsure all insured items upon death
			AutoRenewInsurance = true;
		}

		private void CancelRenewInventoryInsurance()
		{
			if (!CheckAlive())
			{
				return;
			}

			if (Core.SE)
			{
				if (!HasGump(typeof(CancelRenewInventoryInsuranceGump)))
				{
					SendGump(new CancelRenewInventoryInsuranceGump(this, null));
				}
			}
			else
			{
				SendLocalizedMessage(1061075, "", 0x23); // You have cancelled automatically reinsuring all insured items upon death
				AutoRenewInsurance = false;
			}
		}

		private class CancelRenewInventoryInsuranceGump : Gump
		{
			private readonly PlayerMobile m_Player;
			private readonly ItemInsuranceMenuGump m_InsuranceGump;

			public CancelRenewInventoryInsuranceGump(PlayerMobile player, ItemInsuranceMenuGump insuranceGump) : base(250, 200)
			{
				m_Player = player;
				m_InsuranceGump = insuranceGump;

				AddBackground(0, 0, 240, 142, 0x13BE);
				AddImageTiled(6, 6, 228, 100, 0xA40);
				AddImageTiled(6, 116, 228, 20, 0xA40);
				AddAlphaRegion(6, 6, 228, 142);

				AddHtmlLocalized(8, 8, 228, 100, 1071021, 0x7FFF, false, false); // You are about to disable inventory insurance auto-renewal.

				AddButton(6, 116, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
				AddHtmlLocalized(40, 118, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL

				AddButton(114, 116, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
				AddHtmlLocalized(148, 118, 450, 20, 1071022, 0x7FFF, false, false); // DISABLE IT!
			}

			public override void OnResponse(NetState sender, RelayInfo info)
			{
				if (!m_Player.CheckAlive())
				{
					return;
				}

				if (info.ButtonID == 1)
				{
					m_Player.SendLocalizedMessage(1061075, "", 0x23); // You have cancelled automatically reinsuring all insured items upon death
					m_Player.AutoRenewInsurance = false;
				}
				else
				{
					m_Player.SendLocalizedMessage(1042021); // Cancelled.
				}

				if (m_InsuranceGump != null)
				{
					m_Player.SendGump(m_InsuranceGump.NewInstance());
				}
			}
		}

		private void OpenItemInsuranceMenu()
		{
			if (!CheckAlive())
			{
				return;
			}

			var items = new List<Item>();

			foreach (var item in Items)
			{
				if (DisplayInItemInsuranceGump(item))
				{
					items.Add(item);
				}
			}

			var pack = Backpack;

			if (pack != null)
			{
				items.AddRange(pack.FindItemsByType<Item>(true, DisplayInItemInsuranceGump));
			}

			// TODO: Investigate item sorting

			CloseGump(typeof(ItemInsuranceMenuGump));

			if (items.Count == 0)
			{
				SendLocalizedMessage(1114915, "", 0x35); // None of your current items meet the requirements for insurance.
			}
			else
			{
				SendGump(new ItemInsuranceMenuGump(this, items.ToArray()));
			}
		}

		private bool DisplayInItemInsuranceGump(Item item)
		{
			return ((item.Visible || AccessLevel >= AccessLevel.GameMaster) && (item.Insured || CanInsure(item)));
		}

		private class ItemInsuranceMenuGump : Gump
		{
			private readonly PlayerMobile m_From;
			private readonly Item[] m_Items;
			private readonly bool[] m_Insure;
			private readonly int m_Page;

			public ItemInsuranceMenuGump(PlayerMobile from, Item[] items)
				: this(from, items, null, 0)
			{
			}

			public ItemInsuranceMenuGump(PlayerMobile from, Item[] items, bool[] insure, int page)
				: base(25, 50)
			{
				m_From = from;
				m_Items = items;

				if (insure == null)
				{
					insure = new bool[items.Length];

					for (var i = 0; i < items.Length; ++i)
					{
						insure[i] = items[i].Insured;
					}
				}

				m_Insure = insure;
				m_Page = page;

				AddPage(0);

				AddBackground(0, 0, 520, 510, 0x13BE);
				AddImageTiled(10, 10, 500, 30, 0xA40);
				AddImageTiled(10, 50, 500, 355, 0xA40);
				AddImageTiled(10, 415, 500, 80, 0xA40);
				AddAlphaRegion(10, 10, 500, 485);

				AddButton(15, 470, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
				AddHtmlLocalized(50, 472, 80, 20, 1011012, 0x7FFF, false, false); // CANCEL

				if (from.AutoRenewInsurance)
				{
					AddButton(360, 10, 9723, 9724, 1, GumpButtonType.Reply, 0);
				}
				else
				{
					AddButton(360, 10, 9720, 9722, 1, GumpButtonType.Reply, 0);
				}

				AddHtmlLocalized(395, 14, 105, 20, 1114122, 0x7FFF, false, false); // AUTO REINSURE

				AddButton(395, 470, 0xFA5, 0xFA6, 2, GumpButtonType.Reply, 0);
				AddHtmlLocalized(430, 472, 50, 20, 1006044, 0x7FFF, false, false); // OK

				AddHtmlLocalized(10, 14, 150, 20, 1114121, 0x7FFF, false, false); // <CENTER>ITEM INSURANCE MENU</CENTER>

				AddHtmlLocalized(45, 54, 70, 20, 1062214, 0x7FFF, false, false); // Item
				AddHtmlLocalized(250, 54, 70, 20, 1061038, 0x7FFF, false, false); // Cost
				AddHtmlLocalized(400, 54, 70, 20, 1114311, 0x7FFF, false, false); // Insured

				var balance = Banker.GetBalance(from);
				var cost = 0;

				for (var i = 0; i < items.Length; ++i)
				{
					if (insure[i])
					{
						cost += GetInsuranceCost(items[i]);
					}
				}

				AddHtmlLocalized(15, 420, 300, 20, 1114310, 0x7FFF, false, false); // GOLD AVAILABLE:
				AddLabel(215, 420, 0x481, balance.ToString());
				AddHtmlLocalized(15, 435, 300, 20, 1114123, 0x7FFF, false, false); // TOTAL COST OF INSURANCE:
				AddLabel(215, 435, 0x481, cost.ToString());

				if (cost != 0)
				{
					AddHtmlLocalized(15, 450, 300, 20, 1114125, 0x7FFF, false, false); // NUMBER OF DEATHS PAYABLE:
					AddLabel(215, 450, 0x481, (balance / cost).ToString());
				}

				for (int i = page * 4, y = 72; i < (page + 1) * 4 && i < items.Length; ++i, y += 75)
				{
					var item = items[i];
					var b = ItemBounds.Table[item.ItemID];

					AddImageTiledButton(40, y, 0x918, 0x918, 0, GumpButtonType.Page, 0, item.ItemID, item.Hue, 40 - b.Width / 2 - b.X, 30 - b.Height / 2 - b.Y);
					AddItemProperty(item.Serial);

					if (insure[i])
					{
						AddButton(400, y, 9723, 9724, 100 + i, GumpButtonType.Reply, 0);
						AddLabel(250, y, 0x481, GetInsuranceCost(item).ToString());
					}
					else
					{
						AddButton(400, y, 9720, 9722, 100 + i, GumpButtonType.Reply, 0);
						AddLabel(250, y, 0x66C, GetInsuranceCost(item).ToString());
					}
				}

				if (page >= 1)
				{
					AddButton(15, 380, 0xFAE, 0xFAF, 3, GumpButtonType.Reply, 0);
					AddHtmlLocalized(50, 380, 450, 20, 1044044, 0x7FFF, false, false); // PREV PAGE
				}

				if ((page + 1) * 4 < items.Length)
				{
					AddButton(400, 380, 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0);
					AddHtmlLocalized(435, 380, 70, 20, 1044045, 0x7FFF, false, false); // NEXT PAGE
				}
			}

			public ItemInsuranceMenuGump NewInstance()
			{
				return new ItemInsuranceMenuGump(m_From, m_Items, m_Insure, m_Page);
			}

			public override void OnResponse(NetState sender, RelayInfo info)
			{
				if (info.ButtonID == 0 || !m_From.CheckAlive())
				{
					return;
				}

				switch (info.ButtonID)
				{
					case 1: // Auto Reinsure
						{
							if (m_From.AutoRenewInsurance)
							{
								if (!m_From.HasGump(typeof(CancelRenewInventoryInsuranceGump)))
								{
									m_From.SendGump(new CancelRenewInventoryInsuranceGump(m_From, this));
								}
							}
							else
							{
								m_From.AutoRenewInventoryInsurance();
								m_From.SendGump(new ItemInsuranceMenuGump(m_From, m_Items, m_Insure, m_Page));
							}

							break;
						}
					case 2: // OK
						{
							m_From.SendGump(new ItemInsuranceMenuConfirmGump(m_From, m_Items, m_Insure, m_Page));

							break;
						}
					case 3: // Prev
						{
							if (m_Page >= 1)
							{
								m_From.SendGump(new ItemInsuranceMenuGump(m_From, m_Items, m_Insure, m_Page - 1));
							}

							break;
						}
					case 4: // Next
						{
							if ((m_Page + 1) * 4 < m_Items.Length)
							{
								m_From.SendGump(new ItemInsuranceMenuGump(m_From, m_Items, m_Insure, m_Page + 1));
							}

							break;
						}
					default:
						{
							var idx = info.ButtonID - 100;

							if (idx >= 0 && idx < m_Items.Length)
							{
								m_Insure[idx] = !m_Insure[idx];
							}

							m_From.SendGump(new ItemInsuranceMenuGump(m_From, m_Items, m_Insure, m_Page));

							break;
						}
				}
			}
		}

		private class ItemInsuranceMenuConfirmGump : Gump
		{
			private readonly PlayerMobile m_From;
			private readonly Item[] m_Items;
			private readonly bool[] m_Insure;
			private readonly int m_Page;

			public ItemInsuranceMenuConfirmGump(PlayerMobile from, Item[] items, bool[] insure, int page)
				: base(250, 200)
			{
				m_From = from;
				m_Items = items;
				m_Insure = insure;
				m_Page = page;

				AddBackground(0, 0, 240, 142, 0x13BE);
				AddImageTiled(6, 6, 228, 100, 0xA40);
				AddImageTiled(6, 116, 228, 20, 0xA40);
				AddAlphaRegion(6, 6, 228, 142);

				AddHtmlLocalized(8, 8, 228, 100, 1114300, 0x7FFF, false, false); // Do you wish to insure all newly selected items?

				AddButton(6, 116, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
				AddHtmlLocalized(40, 118, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL

				AddButton(114, 116, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
				AddHtmlLocalized(148, 118, 450, 20, 1073996, 0x7FFF, false, false); // ACCEPT
			}

			public override void OnResponse(NetState sender, RelayInfo info)
			{
				if (!m_From.CheckAlive())
				{
					return;
				}

				if (info.ButtonID == 1)
				{
					for (var i = 0; i < m_Items.Length; ++i)
					{
						var item = m_Items[i];

						if (item.Insured != m_Insure[i])
						{
							m_From.ToggleItemInsurance_Callback(m_From, item, false);
						}
					}
				}
				else
				{
					m_From.SendLocalizedMessage(1042021); // Cancelled.
					m_From.SendGump(new ItemInsuranceMenuGump(m_From, m_Items, m_Insure, m_Page));
				}
			}
		}

		#endregion

		private void ToggleTrades()
		{
			RefuseTrades = !RefuseTrades;
		}

		private void GetVendor()
		{
			var house = BaseHouse.FindHouseAt(this);

			if (CheckAlive() && house != null && house.IsOwner(this) && house.InternalizedVendors.Count > 0)
			{
				CloseGump(typeof(ReclaimVendorGump));
				SendGump(new ReclaimVendorGump(house));
			}
		}

		private void LeaveHouse()
		{
			var house = BaseHouse.FindHouseAt(this);

			if (house != null)
			{
				Location = house.BanLocation;
			}
		}

		private delegate void ContextCallback();

		private class CallbackEntry : ContextMenuEntry
		{
			private readonly ContextCallback m_Callback;

			public CallbackEntry(int number, ContextCallback callback) : this(number, -1, callback)
			{
			}

			public CallbackEntry(int number, int range, ContextCallback callback) : base(number, range)
			{
				m_Callback = callback;
			}

			public override void OnClick()
			{
				if (m_Callback != null)
				{
					m_Callback();
				}
			}
		}

		public override void DisruptiveAction()
		{
			if (Meditating)
			{
				RemoveBuff(BuffIcon.ActiveMeditation);
			}

			base.DisruptiveAction();
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (this == from && !Warmode)
			{
				var mount = Mount;

				if (mount != null && !DesignContext.Check(this))
				{
					return;
				}
			}

			base.OnDoubleClick(from);
		}

		public override void DisplayPaperdollTo(Mobile to)
		{
			if (DesignContext.Check(this))
			{
				base.DisplayPaperdollTo(to);
			}
		}

		private static bool m_NoRecursion;

		public override bool CheckEquip(Item item)
		{
			if (!base.CheckEquip(item))
			{
				return false;
			}

			#region Dueling
			if (m_DuelContext != null && !m_DuelContext.AllowItemEquip(this, item))
			{
				return false;
			}
			#endregion

			#region Factions
			var factionItem = FactionItem.Find(item);

			if (factionItem != null)
			{
				var faction = Faction.Find(this);

				if (faction == null)
				{
					SendLocalizedMessage(1010371); // You cannot equip a faction item!
					return false;
				}
				else if (faction != factionItem.Faction)
				{
					SendLocalizedMessage(1010372); // You cannot equip an opposing faction's item!
					return false;
				}
				else
				{
					var maxWearables = FactionItem.GetMaxWearables(this);

					for (var i = 0; i < Items.Count; ++i)
					{
						var equiped = Items[i];

						if (item != equiped && FactionItem.Find(equiped) != null)
						{
							if (--maxWearables == 0)
							{
								SendLocalizedMessage(1010373); // You do not have enough rank to equip more faction items!
								return false;
							}
						}
					}
				}
			}
			#endregion

			if (AccessLevel < AccessLevel.GameMaster && item.Layer != Layer.Mount && HasTrade)
			{
				var bounce = item.GetBounce();

				if (bounce != null)
				{
					if (bounce.m_Parent is Item)
					{
						var parent = (Item)bounce.m_Parent;

						if (parent == Backpack || parent.IsChildOf(Backpack))
						{
							return true;
						}
					}
					else if (bounce.m_Parent == this)
					{
						return true;
					}
				}

				SendLocalizedMessage(1004042); // You can only equip what you are already carrying while you have a trade pending.
				return false;
			}

			return true;
		}

		public override bool CheckTrade(Mobile to, Item item, SecureTradeContainer cont, bool message, bool checkItems, int plusItems, int plusWeight)
		{
			var msgNum = 0;

			if (cont == null)
			{
				if (to.Holding != null)
				{
					msgNum = 1062727; // You cannot trade with someone who is dragging something.
				}
				else if (HasTrade)
				{
					msgNum = 1062781; // You are already trading with someone else!
				}
				else if (to.HasTrade)
				{
					msgNum = 1062779; // That person is already involved in a trade
				}
				else if (to is PlayerMobile && ((PlayerMobile)to).RefuseTrades)
				{
					msgNum = 1154111; // ~1_NAME~ is refusing all trades.
				}
			}

			if (msgNum == 0 && item != null)
			{
				if (cont != null)
				{
					plusItems += cont.TotalItems;
					plusWeight += cont.TotalWeight;
				}

				if (Backpack == null || !Backpack.CheckHold(this, item, false, checkItems, plusItems, plusWeight))
				{
					msgNum = 1004040; // You would not be able to hold this if the trade failed.
				}
				else if (to.Backpack == null || !to.Backpack.CheckHold(to, item, false, checkItems, plusItems, plusWeight))
				{
					msgNum = 1004039; // The recipient of this trade would not be able to carry this.
				}
				else
				{
					msgNum = CheckContentForTrade(item);
				}
			}

			if (msgNum != 0)
			{
				if (message)
				{
					if (msgNum == 1154111)
					{
						SendLocalizedMessage(msgNum, to.Name);
					}
					else
					{
						SendLocalizedMessage(msgNum);
					}
				}

				return false;
			}

			return true;
		}

		private static int CheckContentForTrade(Item item)
		{
			if (item is TrapableContainer && ((TrapableContainer)item).TrapType != TrapType.None)
			{
				return 1004044; // You may not trade trapped items.
			}

			if (SkillHandlers.StolenItem.IsStolen(item))
			{
				return 1004043; // You may not trade recently stolen items.
			}

			if (item is Container)
			{
				foreach (var subItem in item.Items)
				{
					var msg = CheckContentForTrade(subItem);

					if (msg != 0)
					{
						return msg;
					}
				}
			}

			return 0;
		}

		public override bool CheckNonlocalDrop(Mobile from, Item item, Item target)
		{
			if (!base.CheckNonlocalDrop(from, item, target))
			{
				return false;
			}

			if (from.AccessLevel >= AccessLevel.GameMaster)
			{
				return true;
			}

			var pack = Backpack;
			if (from == this && HasTrade && (target == pack || target.IsChildOf(pack)))
			{
				var bounce = item.GetBounce();

				if (bounce != null && bounce.m_Parent is Item)
				{
					var parent = (Item)bounce.m_Parent;

					if (parent == pack || parent.IsChildOf(pack))
					{
						return true;
					}
				}

				SendLocalizedMessage(1004041); // You can't do that while you have a trade pending.
				return false;
			}

			return true;
		}

		protected override void OnLocationChange(Point3D oldLocation)
		{
			CheckLightLevels(false);

			#region Dueling
			if (m_DuelContext != null)
			{
				m_DuelContext.OnLocationChanged(this);
			}
			#endregion

			var context = m_DesignContext;

			if (context == null || m_NoRecursion)
			{
				return;
			}

			m_NoRecursion = true;

			var foundation = context.Foundation;

			int newX = X, newY = Y;
			var newZ = foundation.Z + HouseFoundation.GetLevelZ(context.Level, context.Foundation);

			var startX = foundation.X + foundation.Components.Min.X + 1;
			var startY = foundation.Y + foundation.Components.Min.Y + 1;
			var endX = startX + foundation.Components.Width - 1;
			var endY = startY + foundation.Components.Height - 2;

			if (newX >= startX && newY >= startY && newX < endX && newY < endY && Map == foundation.Map)
			{
				if (Z != newZ)
				{
					Location = new Point3D(X, Y, newZ);
				}

				m_NoRecursion = false;
				return;
			}

			Location = new Point3D(foundation.X, foundation.Y, newZ);
			Map = foundation.Map;

			m_NoRecursion = false;
		}

		public override bool OnMoveOver(Mobile m)
		{
			if (m is BaseCreature && !((BaseCreature)m).Controlled)
			{
				return (!Alive || !m.Alive || IsDeadBondedPet || m.IsDeadBondedPet) || (Hidden && AccessLevel > AccessLevel.Player);
			}

			#region Dueling
			if (Region.IsPartOf(typeof(Engines.ConPVP.SafeZone)) && m is PlayerMobile)
			{
				var pm = (PlayerMobile)m;

				if (pm.DuelContext == null || pm.DuelPlayer == null || !pm.DuelContext.Started || pm.DuelContext.Finished || pm.DuelPlayer.Eliminated)
				{
					return true;
				}
			}
			#endregion

			return base.OnMoveOver(m);
		}

		public override bool CheckShove(Mobile shoved)
		{
			if (m_IgnoreMobiles || TransformationSpellHelper.UnderTransformation(shoved, typeof(WraithFormSpell)))
			{
				return true;
			}
			else
			{
				return base.CheckShove(shoved);
			}
		}

		protected override void OnMapChange(Map oldMap)
		{
			if (BlockQuery != null)
			{
				m_PreviousMapBlock = BlockQuery.QueryMobile(this, m_PreviousMapBlock);
			}

			if ((Map != Faction.Facet && oldMap == Faction.Facet) || (Map == Faction.Facet && oldMap != Faction.Facet))
			{
				InvalidateProperties();
			}

			#region Dueling
			if (m_DuelContext != null)
			{
				m_DuelContext.OnMapChanged(this);
			}
			#endregion

			var context = m_DesignContext;

			if (context == null || m_NoRecursion)
			{
				return;
			}

			m_NoRecursion = true;

			var foundation = context.Foundation;

			if (Map != foundation.Map)
			{
				Map = foundation.Map;
			}

			m_NoRecursion = false;
		}

		public override void OnBeneficialAction(Mobile target, bool isCriminal)
		{
			if (m_SentHonorContext != null)
			{
				m_SentHonorContext.OnSourceBeneficialAction(target);
			}

			base.OnBeneficialAction(target, isCriminal);
		}

		public override void OnDamage(int amount, Mobile from, bool willKill)
		{
			int disruptThreshold;

			if (!Core.AOS)
			{
				disruptThreshold = 0;
			}
			else if (from != null && from.Player)
			{
				disruptThreshold = 18;
			}
			else
			{
				disruptThreshold = 25;
			}

			if (amount > disruptThreshold)
			{
				var c = BandageContext.GetContext(this);

				if (c != null)
				{
					c.Slip();
				}
			}

			if (ConfidenceSpell.IsRegenerating(this))
			{
				ConfidenceSpell.StopRegenerating(this);
			}

			SleepSpell.OnDamage(this);

			WeightOverloading.FatigueOnDamage(this, amount);

			if (m_ReceivedHonorContext != null)
			{
				m_ReceivedHonorContext.OnTargetDamaged(from, amount);
			}

			if (m_SentHonorContext != null)
			{
				m_SentHonorContext.OnSourceDamaged(from, amount);
			}

			if (willKill && from is PlayerMobile pm)
			{
				Timer.DelayCall(TimeSpan.FromSeconds(10), pm.RecoverAmmo);
			}

			base.OnDamage(amount, from, willKill);
		}

		public override void Resurrect()
		{
			var wasAlive = Alive;

			base.Resurrect();

			if (Alive && !wasAlive)
			{
				Item deathRobe = new DeathRobe();

				if (!EquipItem(deathRobe))
				{
					deathRobe.Delete();
				}
			}
		}

		public override double RacialSkillBonus
		{
			get
			{
				if (Core.ML && Race == Race.Human)
				{
					return 20.0;
				}

				return 0;
			}
		}

		public override void OnWarmodeChanged()
		{
			AutoToggleWeapon.From(this);

			if (!Warmode)
			{
				Timer.DelayCall(TimeSpan.FromSeconds(10), RecoverAmmo);
			}
		}

		private Mobile m_InsuranceAward;
		private int m_InsuranceCost;
		private int m_InsuranceBonus;

		private List<Item> m_EquipSnapshot;

		public List<Item> EquipSnapshot => m_EquipSnapshot;

		public override bool OnBeforeDeath()
		{
			var state = NetState;

			if (state != null)
			{
				state.CancelAllTrades();
			}

			DropHolding();

			if (Core.AOS && Backpack != null && !Backpack.Deleted)
			{
				var ilist = Backpack.FindItemsByType<Item>(item =>
				{
					if (!item.Deleted && (item.LootType == LootType.Blessed || item.Insured))
					{
						if (Backpack != item.Parent)
						{
							return true;
						}
					}

					return false;
				});

				for (var i = 0; i < ilist.Length; i++)
				{
					Backpack.AddItem(ilist[i]);
				}
			}

			m_EquipSnapshot = new List<Item>(Items);

			m_NonAutoreinsuredItems = 0;
			m_InsuranceCost = 0;
			m_InsuranceAward = base.FindMostRecentDamager(false);

			if (m_InsuranceAward is BaseCreature)
			{
				var master = ((BaseCreature)m_InsuranceAward).GetMaster();

				if (master != null)
				{
					m_InsuranceAward = master;
				}
			}

			if (m_InsuranceAward != null && (!m_InsuranceAward.Player || m_InsuranceAward == this))
			{
				m_InsuranceAward = null;
			}

			if (m_InsuranceAward is PlayerMobile)
			{
				((PlayerMobile)m_InsuranceAward).m_InsuranceBonus = 0;
			}

			if (m_ReceivedHonorContext != null)
			{
				m_ReceivedHonorContext.OnTargetKilled();
			}

			if (m_SentHonorContext != null)
			{
				m_SentHonorContext.OnSourceKilled();
			}

			RecoverAmmo();

			return base.OnBeforeDeath();
		}

		private bool CheckInsuranceOnDeath(Item item)
		{
			if (InsuranceEnabled && item.Insured)
			{
				#region Dueling
				if (m_DuelPlayer != null && m_DuelContext != null && m_DuelContext.Registered && m_DuelContext.Started && !m_DuelPlayer.Eliminated)
				{
					return true;
				}
				#endregion

				if (AutoRenewInsurance)
				{
					var cost = GetInsuranceCost(item);

					if (m_InsuranceAward != null)
					{
						cost /= 2;
					}

					if (Banker.Withdraw(this, cost))
					{
						m_InsuranceCost += cost;
						item.PayedInsurance = true;
						SendLocalizedMessage(1060398, cost.ToString()); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
					}
					else
					{
						SendLocalizedMessage(1061079, "", 0x23); // You lack the funds to purchase the insurance
						item.PayedInsurance = false;
						item.Insured = false;
						m_NonAutoreinsuredItems++;
					}
				}
				else
				{
					item.PayedInsurance = false;
					item.Insured = false;
				}

				if (m_InsuranceAward != null)
				{
					if (Banker.Deposit(m_InsuranceAward, 300))
					{
						if (m_InsuranceAward is PlayerMobile)
						{
							((PlayerMobile)m_InsuranceAward).m_InsuranceBonus += 300;
						}
					}
				}

				return true;
			}

			return false;
		}

		public override DeathMoveResult GetParentMoveResultFor(Item item)
		{
			if (CheckInsuranceOnDeath(item))
			{
				return DeathMoveResult.MoveToBackpack;
			}

			var res = base.GetParentMoveResultFor(item);

			if (res == DeathMoveResult.MoveToCorpse && item.Movable && Young)
			{
				res = DeathMoveResult.MoveToBackpack;
			}

			return res;
		}

		public override DeathMoveResult GetInventoryMoveResultFor(Item item)
		{
			if (CheckInsuranceOnDeath(item))
			{
				return DeathMoveResult.MoveToBackpack;
			}

			var res = base.GetInventoryMoveResultFor(item);

			if (res == DeathMoveResult.MoveToCorpse && item.Movable && Young)
			{
				res = DeathMoveResult.MoveToBackpack;
			}

			return res;
		}

		public override void OnDeath(Container c)
		{
			if (m_NonAutoreinsuredItems > 0)
			{
				SendLocalizedMessage(1061115);
			}

			base.OnDeath(c);

			m_EquipSnapshot = null;

			HueMod = -1;
			NameMod = null;
			SavagePaintExpiration = TimeSpan.Zero;

			SetHairMods(-1, -1);

			DisguiseTimers.RemoveTimer(this);

			IncognitoSpell.EndIncognito(this);
			PolymorphSpell.EndPolymorph(this);

			MeerMage.StopEffect(this, false);

			#region Stygian Abyss
			if (Flying)
			{
				Flying = false;
				BuffInfo.RemoveBuff(this, BuffIcon.Fly);
			}
			#endregion

			SkillHandlers.StolenItem.ReturnOnDeath(this, c);

			if (m_PermaFlags.Count > 0)
			{
				m_PermaFlags.Clear();

				if (c is Corpse)
				{
					((Corpse)c).Criminal = true;
				}

				if (SkillHandlers.Stealing.ClassicMode)
				{
					Criminal = true;
				}
			}

			if (Murderer && DateTime.UtcNow >= m_NextJustAward)
			{
				var m = FindMostRecentDamager(false);

				if (m is BaseCreature)
				{
					m = ((BaseCreature)m).GetMaster();
				}

				if (m != null && m is PlayerMobile && m != this)
				{
					var gainedPath = false;

					var pointsToGain = 0;

					pointsToGain += (int)Math.Sqrt(GameTime.TotalSeconds * 4);
					pointsToGain *= 5;
					pointsToGain += (int)Math.Pow(Skills.Total / 250, 2);

					if (VirtueHelper.Award(m, VirtueName.Justice, pointsToGain, ref gainedPath))
					{
						if (gainedPath)
						{
							m.SendLocalizedMessage(1049367); // You have gained a path in Justice!
						}
						else
						{
							m.SendLocalizedMessage(1049363); // You have gained in Justice.
						}

						m.FixedParticles(0x375A, 9, 20, 5027, EffectLayer.Waist);
						m.PlaySound(0x1F7);

						m_NextJustAward = DateTime.UtcNow + TimeSpan.FromMinutes(pointsToGain / 3);
					}
				}
			}

			if (m_InsuranceAward is PlayerMobile)
			{
				var pm = (PlayerMobile)m_InsuranceAward;

				if (pm.m_InsuranceBonus > 0)
				{
					pm.SendLocalizedMessage(1060397, pm.m_InsuranceBonus.ToString()); // ~1_AMOUNT~ gold has been deposited into your bank box.
				}
			}

			var killer = FindMostRecentDamager(true);

			if (killer is BaseCreature)
			{
				var bc = (BaseCreature)killer;

				var master = bc.GetMaster();
				if (master != null)
				{
					killer = master;
				}
			}

			if (Young && m_DuelContext == null)
			{
				if (YoungDeathTeleport())
				{
					Timer.DelayCall(TimeSpan.FromSeconds(2.5), SendYoungDeathNotice);
				}
			}

			if (m_DuelContext == null || !m_DuelContext.Registered || !m_DuelContext.Started || m_DuelPlayer == null || m_DuelPlayer.Eliminated)
			{
				Faction.HandleDeath(this, killer);
			}

			if (killer is PlayerMobile pk)
			{
				Reputation.HandleDeath(this, pk);
			}

			Server.Guilds.Guild.HandleDeath(this, killer);

			#region Dueling
			if (m_DuelContext != null)
			{
				m_DuelContext.OnDeath(this, c);
			}
			#endregion

			if (m_BuffTable != null)
			{
				var list = new List<BuffInfo>();

				foreach (var buff in m_BuffTable.Values)
				{
					if (!buff.RetainThroughDeath)
					{
						list.Add(buff);
					}
				}

				for (var i = 0; i < list.Count; i++)
				{
					RemoveBuff(list[i]);
				}
			}
		}

		#region Stuck Menu
		private DateTime[] m_StuckMenuUses;

		public bool CanUseStuckMenu()
		{
			if (m_StuckMenuUses == null)
			{
				return true;
			}
			else
			{
				for (var i = 0; i < m_StuckMenuUses.Length; ++i)
				{
					if ((DateTime.UtcNow - m_StuckMenuUses[i]) > TimeSpan.FromDays(1.0))
					{
						return true;
					}
				}

				return false;
			}
		}

		public void UsedStuckMenu()
		{
			if (m_StuckMenuUses == null)
			{
				m_StuckMenuUses = new DateTime[2];
			}

			for (var i = 0; i < m_StuckMenuUses.Length; ++i)
			{
				if ((DateTime.UtcNow - m_StuckMenuUses[i]) > TimeSpan.FromDays(1.0))
				{
					m_StuckMenuUses[i] = DateTime.UtcNow;
					return;
				}
			}
		}
		#endregion

		private List<Mobile> m_PermaFlags;
		private readonly List<Mobile> m_VisList;
		private readonly Hashtable m_AntiMacroTable;
		private TimeSpan m_GameTime;
		private TimeSpan m_ShortTermElapse;
		private TimeSpan m_LongTermElapse;
		private DateTime m_SessionStart;
		private DateTime m_LastEscortTime;
		private DateTime m_LastPetBallTime;
		private DateTime m_NextSmithBulkOrder;
		private DateTime m_NextTailorBulkOrder;
		private DateTime m_SavagePaintExpiration;
		private SkillName m_Learning = (SkillName)(-1);

		public SkillName Learning
		{
			get => m_Learning;
			set => m_Learning = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan SavagePaintExpiration
		{
			get
			{
				var ts = m_SavagePaintExpiration - DateTime.UtcNow;

				if (ts < TimeSpan.Zero)
				{
					ts = TimeSpan.Zero;
				}

				return ts;
			}
			set => m_SavagePaintExpiration = DateTime.UtcNow + value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan NextSmithBulkOrder
		{
			get
			{
				var ts = m_NextSmithBulkOrder - DateTime.UtcNow;

				if (ts < TimeSpan.Zero)
				{
					ts = TimeSpan.Zero;
				}

				return ts;
			}
			set
			{
				try { m_NextSmithBulkOrder = DateTime.UtcNow + value; }
				catch { }
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan NextTailorBulkOrder
		{
			get
			{
				var ts = m_NextTailorBulkOrder - DateTime.UtcNow;

				if (ts < TimeSpan.Zero)
				{
					ts = TimeSpan.Zero;
				}

				return ts;
			}
			set
			{
				try { m_NextTailorBulkOrder = DateTime.UtcNow + value; }
				catch { }
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastEscortTime
		{
			get => m_LastEscortTime;
			set => m_LastEscortTime = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastPetBallTime
		{
			get => m_LastPetBallTime;
			set => m_LastPetBallTime = value;
		}

		public PlayerMobile()
		{
			m_AutoStabled = new List<Mobile>();

			m_VisList = new List<Mobile>();
			m_PermaFlags = new List<Mobile>();
			m_AntiMacroTable = new Hashtable();
			m_RecentlyReported = new List<Mobile>();

			m_BOBFilter = new Engines.BulkOrders.BOBFilter();

			m_GameTime = TimeSpan.Zero;
			m_ShortTermElapse = TimeSpan.FromHours(8.0);
			m_LongTermElapse = TimeSpan.FromHours(40.0);

			m_JusticeProtectors = new List<Mobile>();
			m_GuildRank = Guilds.RankDefinition.Lowest;

			m_ChampionTitles = new ChampionTitleInfo();

			InvalidateMyRunUO();
		}

		public override bool MutateSpeech(HashSet<Mobile> hears, ref string text, ref object context)
		{
			if (Alive)
			{
				return false;
			}

			if (Core.ML && Skills[SkillName.SpiritSpeak].Value >= 100.0)
			{
				return false;
			}

			if (Core.AOS)
			{
				foreach (var m in hears)
				{
					if (m != this && m.Skills[SkillName.SpiritSpeak].Value >= 100.0)
					{
						return false;
					}
				}
			}

			return base.MutateSpeech(hears, ref text, ref context);
		}

		public override void DoSpeech(string text, int[] keywords, MessageType type, int hue)
		{
			if (Guilds.Guild.NewGuildSystem && (type == MessageType.Guild || type == MessageType.Alliance))
			{
				var g = Guild as Guilds.Guild;
				if (g == null)
				{
					SendLocalizedMessage(1063142); // You are not in a guild!
				}
				else if (type == MessageType.Alliance)
				{
					if (g.Alliance != null && g.Alliance.IsMember(g))
					{
						//g.Alliance.AllianceTextMessage( hue, "[Alliance][{0}]: {1}", this.Name, text );
						g.Alliance.AllianceChat(this, text);
						SendToStaffMessage(this, "[Alliance]: {0}", text);

						m_AllianceMessageHue = hue;
					}
					else
					{
						SendLocalizedMessage(1071020); // You are not in an alliance!
					}
				}
				else    //Type == MessageType.Guild
				{
					m_GuildMessageHue = hue;

					g.GuildChat(this, text);
					SendToStaffMessage(this, "[Guild]: {0}", text);
				}
			}
			else
			{
				base.DoSpeech(text, keywords, type, hue);
			}
		}

		private static void SendToStaffMessage(Mobile from, string text)
		{
			Packet p = null;

			foreach (var ns in from.GetClientsInRange(8))
			{
				var mob = ns.Mobile;

				if (mob != null && mob.AccessLevel >= AccessLevel.GameMaster && mob.AccessLevel > from.AccessLevel)
				{
					if (p == null)
					{
						p = Packet.Acquire(new UnicodeMessage(from.Serial, from.Body, MessageType.Regular, from.SpeechHue, 3, from.Language, from.Name, text));
					}

					ns.Send(p);
				}
			}

			Packet.Release(p);
		}

		private static void SendToStaffMessage(Mobile from, string format, params object[] args)
		{
			SendToStaffMessage(from, String.Format(format, args));
		}

		public override void Damage(int amount, Mobile from)
		{
			if (Spells.Necromancy.EvilOmenSpell.TryEndEffect(this))
			{
				amount = (int)(amount * 1.25);
			}

			var oath = Spells.Necromancy.BloodOathSpell.GetBloodOath(from);

			/* Per EA's UO Herald Pub48 (ML):
			 * ((resist spellsx10)/20 + 10=percentage of damage resisted)
			 */

			if (oath == this)
			{
				amount = (int)(amount * 1.1);

				if (amount > 35 && from is PlayerMobile)  /* capped @ 35, seems no expansion */
				{
					amount = 35;
				}

				if (Core.ML)
				{
					from.Damage((int)(amount * (1 - (((from.Skills.MagicResist.Value * .5) + 10) / 100))), this);
				}
				else
				{
					from.Damage(amount, this);
				}
			}

			if (from != null && Talisman is BaseTalisman)
			{
				var talisman = (BaseTalisman)Talisman;

				if (talisman.Protection != null && talisman.Protection.Type != null)
				{
					var type = talisman.Protection.Type;

					if (type.IsAssignableFrom(from.GetType()))
					{
						amount = (int)(amount * (1 - (double)talisman.Protection.Amount / 100));
					}
				}
			}

			base.Damage(amount, from);
		}

		#region Poison

		public override ApplyPoisonResult ApplyPoison(Mobile from, Poison poison)
		{
			if (!Alive)
			{
				return ApplyPoisonResult.Immune;
			}

			if (Spells.Necromancy.EvilOmenSpell.TryEndEffect(this))
			{
				poison = PoisonImpl.IncreaseLevel(poison);
			}

			var result = base.ApplyPoison(from, poison);

			if (from != null && result == ApplyPoisonResult.Poisoned && PoisonTimer is PoisonImpl.PoisonTimer)
			{
				(PoisonTimer as PoisonImpl.PoisonTimer).From = from;
			}

			return result;
		}

		public override bool CheckPoisonImmunity(Mobile from, Poison poison)
		{
			if (Young && (DuelContext == null || !DuelContext.Started || DuelContext.Finished))
			{
				return true;
			}

			return base.CheckPoisonImmunity(from, poison);
		}

		public override void OnPoisonImmunity(Mobile from, Poison poison)
		{
			if (Young && (DuelContext == null || !DuelContext.Started || DuelContext.Finished))
			{
				SendLocalizedMessage(502808); // You would have been poisoned, were you not new to the land of Britannia. Be careful in the future.
			}
			else
			{
				base.OnPoisonImmunity(from, poison);
			}
		}

		#endregion

		public PlayerMobile(Serial s) : base(s)
		{
			m_VisList = new List<Mobile>();
			m_AntiMacroTable = new Hashtable();
			InvalidateMyRunUO();
		}

		public List<Mobile> VisibilityList => m_VisList;

		public List<Mobile> PermaFlags => m_PermaFlags;

		public override int Luck => AosAttributes.GetValue(this, AosAttribute.Luck);

		public override bool IsHarmfulCriminal(Mobile target)
		{
			if (SkillHandlers.Stealing.ClassicMode && target is PlayerMobile && ((PlayerMobile)target).m_PermaFlags.Count > 0)
			{
				var noto = Notoriety.Compute(this, target);

				if (noto == Notoriety.Innocent)
				{
					target.Delta(MobileDelta.Noto);
				}

				return false;
			}

			if (target is BaseCreature && ((BaseCreature)target).InitialInnocent && !((BaseCreature)target).Controlled)
			{
				return false;
			}

			if (Core.ML && target is BaseCreature && ((BaseCreature)target).Controlled && this == ((BaseCreature)target).ControlMaster)
			{
				return false;
			}

			return base.IsHarmfulCriminal(target);
		}

		public bool AntiMacroCheck(Skill skill, object obj)
		{
			if (obj == null || m_AntiMacroTable == null || AccessLevel != AccessLevel.Player)
			{
				return true;
			}

			var tbl = (Hashtable)m_AntiMacroTable[skill];
			if (tbl == null)
			{
				m_AntiMacroTable[skill] = tbl = new Hashtable();
			}

			var count = (CountAndTimeStamp)tbl[obj];
			if (count != null)
			{
				if (count.TimeStamp + SkillCheck.AntiMacroExpire <= DateTime.UtcNow)
				{
					count.Count = 1;
					return true;
				}
				else
				{
					++count.Count;
					if (count.Count <= SkillCheck.Allowance)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			else
			{
				tbl[obj] = count = new CountAndTimeStamp();
				count.Count = 1;

				return true;
			}
		}

		private void RevertHair()
		{
			SetHairMods(-1, -1);
		}

		private Engines.BulkOrders.BOBFilter m_BOBFilter;

		public Engines.BulkOrders.BOBFilter BOBFilter => m_BOBFilter;

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			var version = reader.ReadInt();

			switch (version)
			{
				case 32:
					{
						HomeTown = Town.ReadReference(reader);

						goto case 31;
					}
				case 31:
					{
						m_PromoGiftLast = reader.ReadDateTime();

						goto case 30;
					}
				case 30:
					{
						m_LastTimePaged = reader.ReadDateTime();

						goto case 29;
					}
				case 29:
					{
						if (reader.ReadBool())
						{
							m_StuckMenuUses = new DateTime[reader.ReadInt()];

							for (var i = 0; i < m_StuckMenuUses.Length; ++i)
							{
								m_StuckMenuUses[i] = reader.ReadDateTime();
							}
						}
						else
						{
							m_StuckMenuUses = null;
						}

						goto case 28;
					}
				case 28:
					{
						m_PeacedUntil = reader.ReadDateTime();

						goto case 27;
					}
				case 27:
					{
						m_AnkhNextUse = reader.ReadDateTime();

						goto case 26;
					}
				case 26:
					{
						m_AutoStabled = reader.ReadStrongMobileList();

						goto case 25;
					}
				case 25:
					{
						var recipeCount = reader.ReadInt();

						if (recipeCount > 0)
						{
							m_AcquiredRecipes ??= new Dictionary<int, bool>();

							while (--recipeCount >= 0)
							{
								var r = reader.ReadInt();

								if (reader.ReadBool()) // Don't add in recipies which we haven't gotten or have been removed
								{
									m_AcquiredRecipes[r] = true;
								}
							}
						}

						goto case 24;
					}
				case 24:
					{
						m_LastHonorLoss = reader.ReadDeltaTime();

						goto case 23;
					}
				case 23:
					{
						m_ChampionTitles = new ChampionTitleInfo(reader);

						goto case 22;
					}
				case 22:
					{
						m_LastValorLoss = reader.ReadDateTime();

						goto case 21;
					}
				case 21:
					{
						m_ToTItemsTurnedIn = reader.ReadEncodedInt();
						m_ToTTotalMonsterFame = reader.ReadInt();

						goto case 20;
					}
				case 20:
					{
						m_AllianceMessageHue = reader.ReadEncodedInt();
						m_GuildMessageHue = reader.ReadEncodedInt();

						goto case 19;
					}
				case 19:
					{
						var rank = reader.ReadEncodedInt();

						var maxRank = Guilds.RankDefinition.Ranks.Length - 1;

						if (rank > maxRank)
						{
							rank = maxRank;
						}

						m_GuildRank = Guilds.RankDefinition.Ranks[rank];
						m_LastOnline = reader.ReadDateTime();

						goto case 18;
					}
				case 18:
					{
						m_SolenFriendship = reader.ReadEnum<SolenFriendship>();

						goto case 17;
					}
				case 17:
				case 16:
					{						
						m_Profession = reader.ReadEncodedInt();

						goto case 15;
					}
				case 15:
					{
						m_LastCompassionLoss = reader.ReadDeltaTime();

						goto case 14;
					}
				case 14:
					{
						m_CompassionGains = reader.ReadEncodedInt();

						if (m_CompassionGains > 0)
						{
							m_NextCompassionDay = reader.ReadDeltaTime();
						}

						goto case 13;
					}
				case 13:
				case 12:
					{
						m_BOBFilter = new Engines.BulkOrders.BOBFilter(reader);

						goto case 11;
					}
				case 11:
					{
						if (version < 13)
						{
							foreach (var item in reader.ReadStrongItemList())
							{
								if (item?.Deleted == false)
								{
									item.PayedInsurance = true;
								}
							}
						}

						goto case 10;
					}
				case 10:
					{
						if (reader.ReadBool())
						{
							m_HairModID = reader.ReadInt();
							m_HairModHue = reader.ReadInt();
							m_BeardModID = reader.ReadInt();
							m_BeardModHue = reader.ReadInt();
						}

						goto case 9;
					}
				case 9:
					{
						SavagePaintExpiration = reader.ReadTimeSpan();

						if (SavagePaintExpiration > TimeSpan.Zero)
						{
							BodyMod = Female ? 184 : 183;
							HueMod = 0;
						}

						goto case 8;
					}
				case 8:
					{
						m_NpcGuild = reader.ReadEnum<NpcGuild>();
						m_NpcGuildJoinTime = reader.ReadDateTime();
						m_NpcGuildGameTime = reader.ReadTimeSpan();

						goto case 7;
					}
				case 7:
					{
						m_PermaFlags = reader.ReadStrongMobileList();

						goto case 6;
					}
				case 6:
					{
						NextTailorBulkOrder = reader.ReadTimeSpan();

						goto case 5;
					}
				case 5:
					{
						NextSmithBulkOrder = reader.ReadTimeSpan();

						goto case 4;
					}
				case 4:
					{
						m_LastJusticeLoss = reader.ReadDeltaTime();
						m_JusticeProtectors = reader.ReadStrongMobileList();

						goto case 3;
					}
				case 3:
					{
						m_LastSacrificeGain = reader.ReadDeltaTime();
						m_LastSacrificeLoss = reader.ReadDeltaTime();
						m_AvailableResurrects = reader.ReadInt();

						goto case 2;
					}
				case 2:
					{
						m_Flags = reader.ReadEnum<PlayerFlag>();

						goto case 1;
					}
				case 1:
					{
						m_LongTermElapse = reader.ReadTimeSpan();
						m_ShortTermElapse = reader.ReadTimeSpan();
						m_GameTime = reader.ReadTimeSpan();

						goto case 0;
					}
				case 0:
					{
						break;
					}
			}

			m_AutoStabled ??= new List<Mobile>();
			m_RecentlyReported ??= new List<Mobile>();
			m_PermaFlags ??= new List<Mobile>();
			m_JusticeProtectors ??= new List<Mobile>();

			m_BOBFilter ??= new Engines.BulkOrders.BOBFilter();
			m_ChampionTitles ??= new ChampionTitleInfo();

			m_GuildRank ??= Guilds.RankDefinition.Member; //Default to member if going from older version to new version (only time it should be null)
			
			// Professions weren't verified on 1.0 RC0
			if (!CharacterCreation.VerifyProfession(m_Profession))
			{
				m_Profession = 0;
			}

			if (m_LastOnline == DateTime.MinValue && Account != null)
			{
				m_LastOnline = Account.LastLogin;
			}
						
			if (AccessLevel > AccessLevel.Player)
			{
				m_IgnoreMobiles = true;
			}

			foreach (var m in Stabled)
			{
				if (m is BaseCreature bc)
				{
					bc.IsStabled = true;
					bc.StabledBy = this;
				}
			}

			CheckAtrophies(this);

			if (Hidden) // Hiding is the only buff where it has an effect that's serialized.
			{
				AddBuff(new BuffInfo(BuffIcon.HidingAndOrStealth, 1075655));
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			// cleanup our anti-macro table
			foreach (Hashtable t in m_AntiMacroTable.Values)
			{
				var remove = new ArrayList();

				foreach (CountAndTimeStamp time in t.Values)
				{
					if (time.TimeStamp + SkillCheck.AntiMacroExpire <= DateTime.UtcNow)
					{
						remove.Add(time);
					}
				}

				for (var i = 0; i < remove.Count; ++i)
				{
					t.Remove(remove[i]);
				}
			}

			CheckKillDecay();

			CheckAtrophies(this);

			base.Serialize(writer);

			writer.Write(32); // version

			Town.WriteReference(writer, HomeTown);

			writer.Write(m_PromoGiftLast);

			writer.Write(m_LastTimePaged);

			if (m_StuckMenuUses != null)
			{
				writer.Write(true);

				writer.Write(m_StuckMenuUses.Length);

				for (var i = 0; i < m_StuckMenuUses.Length; ++i)
				{
					writer.Write(m_StuckMenuUses[i]);
				}
			}
			else
			{
				writer.Write(false);
			}

			writer.Write(m_PeacedUntil);
			writer.Write(m_AnkhNextUse);
			writer.Write(m_AutoStabled, true);

			if (m_AcquiredRecipes == null)
			{
				writer.Write(0);
			}
			else
			{
				writer.Write(m_AcquiredRecipes.Count);

				foreach (var kvp in m_AcquiredRecipes)
				{
					writer.Write(kvp.Key);
					writer.Write(kvp.Value);
				}
			}

			writer.WriteDeltaTime(m_LastHonorLoss);

			ChampionTitleInfo.Serialize(writer, m_ChampionTitles);

			writer.Write(m_LastValorLoss);
			writer.WriteEncodedInt(m_ToTItemsTurnedIn);
			writer.Write(m_ToTTotalMonsterFame);    //This ain't going to be a small #.

			writer.WriteEncodedInt(m_AllianceMessageHue);
			writer.WriteEncodedInt(m_GuildMessageHue);

			writer.WriteEncodedInt(m_GuildRank.Rank);
			writer.Write(m_LastOnline);

			writer.Write(m_SolenFriendship);

			writer.WriteEncodedInt(m_Profession);

			writer.WriteDeltaTime(m_LastCompassionLoss);

			writer.WriteEncodedInt(m_CompassionGains);

			if (m_CompassionGains > 0)
			{
				writer.WriteDeltaTime(m_NextCompassionDay);
			}

			m_BOBFilter.Serialize(writer);

			var useMods = m_HairModID != -1 || m_BeardModID != -1;

			writer.Write(useMods);

			if (useMods)
			{
				writer.Write(m_HairModID);
				writer.Write(m_HairModHue);
				writer.Write(m_BeardModID);
				writer.Write(m_BeardModHue);
			}

			writer.Write(SavagePaintExpiration);

			writer.Write(m_NpcGuild);
			writer.Write(m_NpcGuildJoinTime);
			writer.Write(m_NpcGuildGameTime);

			writer.Write(m_PermaFlags, true);

			writer.Write(NextTailorBulkOrder);

			writer.Write(NextSmithBulkOrder);

			writer.WriteDeltaTime(m_LastJusticeLoss);
			writer.Write(m_JusticeProtectors, true);

			writer.WriteDeltaTime(m_LastSacrificeGain);
			writer.WriteDeltaTime(m_LastSacrificeLoss);
			writer.Write(m_AvailableResurrects);

			writer.Write(m_Flags);

			writer.Write(m_LongTermElapse);
			writer.Write(m_ShortTermElapse);
			writer.Write(GameTime);
		}

		public static void CheckAtrophies(Mobile m)
		{
			SacrificeVirtue.CheckAtrophy(m);
			JusticeVirtue.CheckAtrophy(m);
			CompassionVirtue.CheckAtrophy(m);
			ValorVirtue.CheckAtrophy(m);

			if (m is PlayerMobile pm)
			{
				ChampionTitleInfo.CheckAtrophy(pm);
			}
		}

		public void CheckKillDecay()
		{
			if (m_ShortTermElapse < GameTime)
			{
				m_ShortTermElapse += TimeSpan.FromHours(8);

				if (ShortTermMurders > 0)
				{
					--ShortTermMurders;
				}
			}

			if (m_LongTermElapse < GameTime)
			{
				m_LongTermElapse += TimeSpan.FromHours(40);

				if (Kills > 0)
				{
					--Kills;
				}
			}
		}

		public void ResetKillTime()
		{
			m_ShortTermElapse = GameTime + TimeSpan.FromHours(8);
			m_LongTermElapse = GameTime + TimeSpan.FromHours(40);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime SessionStart => m_SessionStart;

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan GameTime
		{
			get
			{
				if (NetState != null)
				{
					return m_GameTime + (DateTime.UtcNow - m_SessionStart);
				}
				else
				{
					return m_GameTime;
				}
			}
		}

		public override bool CanSee(Mobile m)
		{
			if (m is CharacterStatue cs)
			{
				cs.OnRequestedAnimation(this);
			}

			if (m is PlayerMobile pm && pm.m_VisList.Contains(this))
			{
				return true;
			}

			if (m_DuelContext != null && m_DuelPlayer != null && !m_DuelContext.Finished && m_DuelContext.m_Tournament != null && !m_DuelPlayer.Eliminated)
			{
				var owner = m;

				if (owner is BaseCreature bc)
				{
					var master = bc.GetMaster();

					if (master != null)
					{
						owner = master;
					}
				}

				if (m.AccessLevel == AccessLevel.Player && owner is PlayerMobile pmo && pmo.DuelContext != m_DuelContext)
				{
					return false;
				}
			}

			return base.CanSee(m);
		}

		public virtual void CheckedAnimate(int action, int frameCount, int repeatCount, bool forward, bool repeat, int delay)
		{
			if (!Mounted)
			{
				base.Animate(action, frameCount, repeatCount, forward, repeat, delay);
			}
		}

		public override void Animate(int action, int frameCount, int repeatCount, bool forward, bool repeat, int delay)
		{
			base.Animate(action, frameCount, repeatCount, forward, repeat, delay);
		}

		public override bool CanSee(Item item)
		{
			if (m_DesignContext != null && m_DesignContext.Foundation.IsHiddenToCustomizer(item))
			{
				return false;
			}

			return base.CanSee(item);
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			Faction.HandleDeletion(this);

			Reputation.HandleDeletion(this);

			BaseHouse.HandleDeletion(this);

			DisguiseTimers.RemoveTimer(this);
		}

		public override bool NewGuildDisplay => Server.Guilds.Guild.NewGuildSystem;

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (Map == Faction.Facet)
			{
				var pl = PlayerState.Find(this);

				if (pl != null)
				{
					var faction = pl.Faction;

					if (faction.Commander == this)
					{
						list.Add(1042733, faction.Definition.PropName); // Commanding Lord of the ~1_FACTION_NAME~
					}
					else if (pl.Sheriff != null)
					{
						list.Add(1042734, "{0}\t{1}", pl.Sheriff.Definition.FriendlyName, faction.Definition.PropName); // The Sheriff of  ~1_CITY~, ~2_FACTION_NAME~
					}
					else if (pl.Finance != null)
					{
						list.Add(1042735, "{0}\t{1}", pl.Finance.Definition.FriendlyName, faction.Definition.PropName); // The Finance Minister of ~1_CITY~, ~2_FACTION_NAME~
					}
					else if (pl.MerchantTitle != MerchantTitle.None)
					{
						list.Add(1060776, "{0}\t{1}", MerchantTitles.GetInfo(pl.MerchantTitle).Title, faction.Definition.PropName); // ~1_val~, ~2_val~
					}
					else
					{
						list.Add(1060776, "{0}\t{1}", pl.Rank.Title, faction.Definition.PropName); // ~1_val~, ~2_val~
					}
				}
			}

			Reputation.AddProperties(this, list);

			if (Core.ML)
			{
				var i = AllFollowers.Count;

				while (--i >= 0)
				{
					if (AllFollowers[i] is BaseCreature c && c.ControlOrder == OrderType.Guard)
					{
						list.Add(501129); // guarded
						break;
					}
				}
			}
		}

		public override void OnSingleClick(Mobile from)
		{
			if (Map == Faction.Facet)
			{
				var pl = PlayerState.Find(this);

				if (pl != null)
				{
					string text;
					var ascii = false;

					var faction = pl.Faction;

					if (faction.Commander == this)
					{
						text = String.Concat(Female ? "(Commanding Lady of the " : "(Commanding Lord of the ", faction.Definition.FriendlyName, ")");
					}
					else if (pl.Sheriff != null)
					{
						text = String.Concat("(The Sheriff of ", pl.Sheriff.Definition.FriendlyName, ", ", faction.Definition.FriendlyName, ")");
					}
					else if (pl.Finance != null)
					{
						text = String.Concat("(The Finance Minister of ", pl.Finance.Definition.FriendlyName, ", ", faction.Definition.FriendlyName, ")");
					}
					else
					{
						ascii = true;

						if (pl.MerchantTitle != MerchantTitle.None)
						{
							text = String.Concat("(", MerchantTitles.GetInfo(pl.MerchantTitle).Title.String, ", ", faction.Definition.FriendlyName, ")");
						}
						else
						{
							text = String.Concat("(", pl.Rank.Title.String, ", ", faction.Definition.FriendlyName, ")");
						}
					}

					var hue = (Faction.Find(from) == faction ? 98 : 38);

					PrivateOverheadMessage(MessageType.Label, hue, ascii, text, from.NetState);
				}
			}

			base.OnSingleClick(from);
		}

		protected override bool OnMove(Direction d)
		{
			if (!Core.SE)
			{
				return base.OnMove(d);
			}

			if (AccessLevel != AccessLevel.Player)
			{
				return true;
			}

			if (Hidden && DesignContext.Find(this) == null) //Hidden & NOT customizing a house
			{
				if (!Mounted && Skills.Stealth.Value >= 25.0)
				{
					var running = (d & Direction.Running) != 0;

					if (running)
					{
						if ((AllowedStealthSteps -= 2) <= 0)
						{
							RevealingAction();
						}
					}
					else if (AllowedStealthSteps-- <= 0)
					{
						SkillHandlers.Stealth.OnUse(this);
					}
				}
				else
				{
					RevealingAction();
				}
			}

			return true;
		}

		private bool m_BedrollLogout;

		public bool BedrollLogout
		{
			get => m_BedrollLogout;
			set => m_BedrollLogout = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public override bool Paralyzed
		{
			get => base.Paralyzed;
			set
			{
				base.Paralyzed = value;

				if (value)
				{
					AddBuff(new BuffInfo(BuffIcon.Paralyze, 1075827));  //Paralyze/You are frozen and can not move
				}
				else
				{
					RemoveBuff(BuffIcon.Paralyze);
				}
			}
		}

		#region Ethics
		private Ethics.Player m_EthicPlayer;

		[CommandProperty(AccessLevel.GameMaster)]
		public Ethics.Player EthicPlayer
		{
			get => m_EthicPlayer;
			set => m_EthicPlayer = value;
		}
		#endregion

		#region Factions
		private PlayerState m_FactionPlayerState;

		public PlayerState FactionPlayerState
		{
			get => m_FactionPlayerState;
			set => m_FactionPlayerState = value;
		}
		#endregion

		#region Dueling
		private Engines.ConPVP.DuelContext m_DuelContext;
		private Engines.ConPVP.DuelPlayer m_DuelPlayer;

		public Engines.ConPVP.DuelContext DuelContext => m_DuelContext;

		public Engines.ConPVP.DuelPlayer DuelPlayer
		{
			get => m_DuelPlayer;
			set
			{
				var wasInTourny = (m_DuelContext != null && !m_DuelContext.Finished && m_DuelContext.m_Tournament != null);

				m_DuelPlayer = value;

				if (m_DuelPlayer == null)
				{
					m_DuelContext = null;
				}
				else
				{
					m_DuelContext = m_DuelPlayer.Participant.Context;
				}

				var isInTourny = (m_DuelContext != null && !m_DuelContext.Finished && m_DuelContext.m_Tournament != null);

				if (wasInTourny != isInTourny)
				{
					SendEverything();
				}
			}
		}
		#endregion

		private SolenFriendship m_SolenFriendship;

		[CommandProperty(AccessLevel.GameMaster)]
		public SolenFriendship SolenFriendship
		{
			get => m_SolenFriendship;
			set => m_SolenFriendship = value;
		}

		#region MyRunUO Invalidation
		private bool m_ChangedMyRunUO;

		public bool ChangedMyRunUO
		{
			get => m_ChangedMyRunUO;
			set => m_ChangedMyRunUO = value;
		}

		public void InvalidateMyRunUO()
		{
			if (!Deleted && !m_ChangedMyRunUO)
			{
				m_ChangedMyRunUO = true;
				Engines.MyRunUO.MyRunUO.QueueMobileUpdate(this);
			}
		}

		public override void OnKillsChange(int oldValue)
		{
			if (Young && Kills > oldValue)
			{
				var acc = Account as Account;

				if (acc != null)
				{
					acc.RemoveYoungStatus(0);
				}
			}

			InvalidateMyRunUO();
		}

		public override void OnGenderChanged(bool oldFemale)
		{
			InvalidateMyRunUO();
		}

		public override void OnGuildChange(Server.Guilds.BaseGuild oldGuild)
		{
			InvalidateMyRunUO();
		}

		public override void OnGuildTitleChange(string oldTitle)
		{
			InvalidateMyRunUO();
		}

		public override void OnKarmaChange(int oldValue)
		{
			InvalidateMyRunUO();
		}

		public override void OnFameChange(int oldValue)
		{
			InvalidateMyRunUO();
		}

		public override void OnSkillChange(SkillName skill, double oldBase)
		{
			if (Young && SkillsTotal >= 4500)
			{
				if (Account is Account acc)
				{
					acc.RemoveYoungStatus(1019036); // You have successfully obtained a respectable skill level, and have outgrown your status as a young player!
				}
			}

			InvalidateMyRunUO();
		}

		public override void OnAccessLevelChanged(AccessLevel oldLevel)
		{
			if (AccessLevel == AccessLevel.Player)
			{
				IgnoreMobiles = false;
			}
			else
			{
				IgnoreMobiles = true;
			}

			InvalidateMyRunUO();
		}

		public override void OnRawStatChange(StatType stat, int oldValue)
		{
			InvalidateMyRunUO();
		}

		public override void OnDelete()
		{
			if (m_ReceivedHonorContext != null)
			{
				m_ReceivedHonorContext.Cancel();
			}

			if (m_SentHonorContext != null)
			{
				m_SentHonorContext.Cancel();
			}

			InvalidateMyRunUO();
		}

		#endregion

		#region Fastwalk Prevention
		private static readonly bool FastwalkPrevention = true; // Is fastwalk prevention enabled?
		private static readonly int FastwalkThreshold = 400; // Fastwalk prevention will become active after 0.4 seconds

		private long m_NextMovementTime;
		private bool m_HasMoved;

		public virtual bool UsesFastwalkPrevention => (AccessLevel < AccessLevel.Counselor);

		public override int ComputeMovementSpeed(Direction dir, bool checkTurning)
		{
			if (checkTurning && (dir & Direction.Mask) != (Direction & Direction.Mask))
			{
				return Mobile.RunMount; // We are NOT actually moving (just a direction change)
			}

			var context = TransformationSpellHelper.GetContext(this);

			if (context != null && context.Type == typeof(ReaperFormSpell))
			{
				return Mobile.WalkFoot;
			}

			var running = ((dir & Direction.Running) != 0);

			var onHorse = Mounted || Flying;

			var animalContext = AnimalFormSpell.GetContext(this);

			if (onHorse || (animalContext != null && animalContext.SpeedBoost))
			{
				return (running ? Mobile.RunMount : Mobile.WalkMount);
			}

			return (running ? Mobile.RunFoot : Mobile.WalkFoot);
		}

		public static bool MovementThrottle_Callback(NetState ns)
		{
			var pm = ns.Mobile as PlayerMobile;

			if (pm == null || !pm.UsesFastwalkPrevention)
			{
				return true;
			}

			if (!pm.m_HasMoved)
			{
				// has not yet moved
				pm.m_NextMovementTime = Core.TickCount;
				pm.m_HasMoved = true;
				return true;
			}

			var ts = pm.m_NextMovementTime - Core.TickCount;

			if (ts < 0)
			{
				// been a while since we've last moved
				pm.m_NextMovementTime = Core.TickCount;
				return true;
			}

			return (ts < FastwalkThreshold);
		}

		#endregion

		#region Enemy of One
		private Type m_EnemyOfOneType;
		private bool m_WaitingForEnemy;

		public Type EnemyOfOneType
		{
			get => m_EnemyOfOneType;
			set
			{
				var oldType = m_EnemyOfOneType;
				var newType = value;

				if (oldType == newType)
				{
					return;
				}

				m_EnemyOfOneType = value;

				DeltaEnemies(oldType, newType);
			}
		}

		public bool WaitingForEnemy
		{
			get => m_WaitingForEnemy;
			set => m_WaitingForEnemy = value;
		}

		private void DeltaEnemies(Type oldType, Type newType)
		{
			foreach (var m in GetMobilesInRange(18))
			{
				var t = m.GetType();

				if (t == oldType || t == newType)
				{
					var ns = NetState;

					if (ns != null)
					{
						if (ns.StygianAbyss)
						{
							ns.Send(new MobileMoving(m, Notoriety.Compute(this, m)));
						}
						else
						{
							ns.Send(new MobileMovingOld(m, Notoriety.Compute(this, m)));
						}
					}
				}
			}
		}

		#endregion

		#region Hair and beard mods
		private int m_HairModID = -1, m_HairModHue;
		private int m_BeardModID = -1, m_BeardModHue;

		public void SetHairMods(int hairID, int beardID)
		{
			if (hairID == -1)
			{
				InternalRestoreHair(true, ref m_HairModID, ref m_HairModHue);
			}
			else if (hairID != -2)
			{
				InternalChangeHair(true, hairID, ref m_HairModID, ref m_HairModHue);
			}

			if (beardID == -1)
			{
				InternalRestoreHair(false, ref m_BeardModID, ref m_BeardModHue);
			}
			else if (beardID != -2)
			{
				InternalChangeHair(false, beardID, ref m_BeardModID, ref m_BeardModHue);
			}
		}

		private void CreateHair(bool hair, int id, int hue)
		{
			if (hair)
			{
				//TODO Verification?
				HairItemID = id;
				HairHue = hue;
			}
			else
			{
				FacialHairItemID = id;
				FacialHairHue = hue;
			}
		}

		private void InternalRestoreHair(bool hair, ref int id, ref int hue)
		{
			if (id == -1)
			{
				return;
			}

			if (hair)
			{
				HairItemID = 0;
			}
			else
			{
				FacialHairItemID = 0;
			}

			//if( id != 0 )
			CreateHair(hair, id, hue);

			id = -1;
			hue = 0;
		}

		private void InternalChangeHair(bool hair, int id, ref int storeID, ref int storeHue)
		{
			if (storeID == -1)
			{
				storeID = hair ? HairItemID : FacialHairItemID;
				storeHue = hair ? HairHue : FacialHairHue;
			}
			CreateHair(hair, id, 0);
		}

		#endregion

		#region Virtues
		private DateTime m_LastSacrificeGain;
		private DateTime m_LastSacrificeLoss;
		private int m_AvailableResurrects;

		public DateTime LastSacrificeGain { get => m_LastSacrificeGain; set => m_LastSacrificeGain = value; }
		public DateTime LastSacrificeLoss { get => m_LastSacrificeLoss; set => m_LastSacrificeLoss = value; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int AvailableResurrects { get => m_AvailableResurrects; set => m_AvailableResurrects = value; }

		private DateTime m_NextJustAward;
		private DateTime m_LastJusticeLoss;
		private List<Mobile> m_JusticeProtectors;

		public DateTime LastJusticeLoss { get => m_LastJusticeLoss; set => m_LastJusticeLoss = value; }
		public List<Mobile> JusticeProtectors { get => m_JusticeProtectors; set => m_JusticeProtectors = value; }

		private DateTime m_LastCompassionLoss;
		private DateTime m_NextCompassionDay;
		private int m_CompassionGains;

		public DateTime LastCompassionLoss { get => m_LastCompassionLoss; set => m_LastCompassionLoss = value; }
		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime NextCompassionDay { get => m_NextCompassionDay; set => m_NextCompassionDay = value; }
		[CommandProperty(AccessLevel.GameMaster)]
		public int CompassionGains { get => m_CompassionGains; set => m_CompassionGains = value; }

		private DateTime m_LastValorLoss;

		public DateTime LastValorLoss { get => m_LastValorLoss; set => m_LastValorLoss = value; }

		private DateTime m_LastHonorLoss;
		private DateTime m_LastHonorUse;
		private bool m_HonorActive;
		private HonorContext m_ReceivedHonorContext;
		private HonorContext m_SentHonorContext;
		public DateTime m_hontime;

		public DateTime LastHonorLoss { get => m_LastHonorLoss; set => m_LastHonorLoss = value; }
		public DateTime LastHonorUse { get => m_LastHonorUse; set => m_LastHonorUse = value; }
		public bool HonorActive { get => m_HonorActive; set => m_HonorActive = value; }
		public HonorContext ReceivedHonorContext { get => m_ReceivedHonorContext; set => m_ReceivedHonorContext = value; }
		public HonorContext SentHonorContext { get => m_SentHonorContext; set => m_SentHonorContext = value; }
		#endregion

		#region Young system
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Young
		{
			get => GetFlag(PlayerFlag.Young);
			set { SetFlag(PlayerFlag.Young, value); InvalidateProperties(); }
		}

		public override string ApplyNameSuffix(string suffix)
		{
			suffix = ApplyTownSuffix(suffix);

			if (Young)
			{
				if (suffix.Length == 0)
				{
					suffix = "(Young)";
				}
				else
				{
					suffix = String.Concat(suffix, " (Young)");
				}
			}

			#region Ethics
			if (m_EthicPlayer != null)
			{
				if (suffix.Length == 0)
				{
					suffix = m_EthicPlayer.Ethic.Definition.Adjunct.String;
				}
				else
				{
					suffix = String.Concat(suffix, " ", m_EthicPlayer.Ethic.Definition.Adjunct.String);
				}
			}
			#endregion

			if (Core.ML && Map == Faction.Facet)
			{
				var faction = Faction.Find(this);

				if (faction != null)
				{
					var adjunct = String.Format("[{0}]", faction.Definition.Abbreviation);
					if (suffix.Length == 0)
					{
						suffix = adjunct;
					}
					else
					{
						suffix = String.Concat(suffix, " ", adjunct);
					}
				}
			}

			return base.ApplyNameSuffix(suffix);
		}

		public override TimeSpan GetLogoutDelay()
		{
			if (Young || BedrollLogout || TestCenter.Enabled)
			{
				return TimeSpan.Zero;
			}

			return base.GetLogoutDelay();
		}

		private DateTime m_LastYoungMessage = DateTime.MinValue;

		public bool CheckYoungProtection(Mobile from)
		{
			if (!Young)
			{
				return false;
			}

			if (Region is BaseRegion br && !br.YoungProtected && br.OnRuleEnforced(RegionFlags.AllowYoungAggro, from, this, true))
			{
				return false;
			}

			if (from is BaseCreature bc && bc.IgnoreYoungProtection)
			{
				return false;
			}

			if (DateTime.UtcNow - m_LastYoungMessage > TimeSpan.FromMinutes(1.0))
			{
				m_LastYoungMessage = DateTime.UtcNow;
				SendLocalizedMessage(1019067); // A monster looks at you menacingly but does not attack.  You would be under attack now if not for your status as a new citizen of Britannia.
			}

			return true;
		}

		private DateTime m_LastYoungHeal = DateTime.MinValue;

		public bool CheckYoungHealTime()
		{
			if (DateTime.UtcNow - m_LastYoungHeal > TimeSpan.FromMinutes(5.0))
			{
				m_LastYoungHeal = DateTime.UtcNow;
				return true;
			}

			return false;
		}

		private static readonly Point3D[] m_TrammelDeathDestinations = new Point3D[]
			{
				new Point3D( 1481, 1612, 20 ),
				new Point3D( 2708, 2153,  0 ),
				new Point3D( 2249, 1230,  0 ),
				new Point3D( 5197, 3994, 37 ),
				new Point3D( 1412, 3793,  0 ),
				new Point3D( 3688, 2232, 20 ),
				new Point3D( 2578,  604,  0 ),
				new Point3D( 4397, 1089,  0 ),
				new Point3D( 5741, 3218, -2 ),
				new Point3D( 2996, 3441, 15 ),
				new Point3D(  624, 2225,  0 ),
				new Point3D( 1916, 2814,  0 ),
				new Point3D( 2929,  854,  0 ),
				new Point3D(  545,  967,  0 ),
				new Point3D( 3665, 2587,  0 )
			};

		private static readonly Point3D[] m_IlshenarDeathDestinations = new Point3D[]
			{
				new Point3D( 1216,  468, -13 ),
				new Point3D(  723, 1367, -60 ),
				new Point3D(  745,  725, -28 ),
				new Point3D(  281, 1017,   0 ),
				new Point3D(  986, 1011, -32 ),
				new Point3D( 1175, 1287, -30 ),
				new Point3D( 1533, 1341,  -3 ),
				new Point3D(  529,  217, -44 ),
				new Point3D( 1722,  219,  96 )
			};

		private static readonly Point3D[] m_MalasDeathDestinations = new Point3D[]
			{
				new Point3D( 2079, 1376, -70 ),
				new Point3D(  944,  519, -71 )
			};

		private static readonly Point3D[] m_TokunoDeathDestinations = new Point3D[]
			{
				new Point3D( 1166,  801, 27 ),
				new Point3D(  782, 1228, 25 ),
				new Point3D(  268,  624, 15 )
			};

		public bool YoungDeathTeleport()
		{
			if (Region.IsPartOf(typeof(Jail))
				|| Region.IsPartOf("Samurai start location")
				|| Region.IsPartOf("Ninja start location")
				|| Region.IsPartOf("Ninja cave"))
			{
				return false;
			}

			Point3D loc;
			Map map;

			var dungeon = (DungeonRegion)Region.GetRegion(typeof(DungeonRegion));
			if (dungeon != null && dungeon.EntranceLocation != Point3D.Zero)
			{
				loc = dungeon.EntranceLocation;
				map = dungeon.EntranceMap;
			}
			else
			{
				loc = Location;
				map = Map;
			}

			Point3D[] list;

			if (map == Map.Trammel)
			{
				list = m_TrammelDeathDestinations;
			}
			else if (map == Map.Ilshenar)
			{
				list = m_IlshenarDeathDestinations;
			}
			else if (map == Map.Malas)
			{
				list = m_MalasDeathDestinations;
			}
			else if (map == Map.Tokuno)
			{
				list = m_TokunoDeathDestinations;
			}
			else
			{
				return false;
			}

			var dest = Point3D.Zero;
			var sqDistance = Int32.MaxValue;

			for (var i = 0; i < list.Length; i++)
			{
				var curDest = list[i];

				var width = loc.X - curDest.X;
				var height = loc.Y - curDest.Y;
				var curSqDistance = width * width + height * height;

				if (curSqDistance < sqDistance)
				{
					dest = curDest;
					sqDistance = curSqDistance;
				}
			}

			MoveToWorld(dest, map);
			return true;
		}

		private void SendYoungDeathNotice()
		{
			SendGump(new YoungDeathNotice());
		}

		#endregion

		#region Speech log
		private SpeechLog m_SpeechLog;

		public SpeechLog SpeechLog => m_SpeechLog;

		public override void OnSpeech(SpeechEventArgs e)
		{
			if (SpeechLog.Enabled && NetState != null)
			{
				if (m_SpeechLog == null)
				{
					m_SpeechLog = new SpeechLog();
				}

				m_SpeechLog.Add(e.Mobile, e.Speech);
			}
		}

		#endregion

		#region Champion Titles
		[CommandProperty(AccessLevel.GameMaster)]
		public bool DisplayChampionTitle
		{
			get => GetFlag(PlayerFlag.DisplayChampionTitle);
			set => SetFlag(PlayerFlag.DisplayChampionTitle, value);
		}

		private ChampionTitleInfo m_ChampionTitles;

		[CommandProperty(AccessLevel.GameMaster)]
		public ChampionTitleInfo ChampionTitles { get => m_ChampionTitles; set { } }

		private void ToggleChampionTitleDisplay()
		{
			if (!CheckAlive())
			{
				return;
			}

			if (DisplayChampionTitle)
			{
				SendLocalizedMessage(1062419, "", 0x23); // You have chosen to hide your monster kill title.
			}
			else
			{
				SendLocalizedMessage(1062418, "", 0x23); // You have chosen to display your monster kill title.
			}

			DisplayChampionTitle = !DisplayChampionTitle;
		}

		[PropertyObject]
		public class ChampionTitleInfo
		{
			public static TimeSpan LossDelay = TimeSpan.FromDays(1.0);
			public const int LossAmount = 90;

			private class TitleInfo
			{
				private int m_Value;
				private DateTime m_LastDecay;

				public int Value { get => m_Value; set => m_Value = value; }
				public DateTime LastDecay { get => m_LastDecay; set => m_LastDecay = value; }

				public TitleInfo()
				{
				}

				public TitleInfo(GenericReader reader)
				{
					var version = reader.ReadEncodedInt();

					switch (version)
					{
						case 0:
							{
								m_Value = reader.ReadEncodedInt();
								m_LastDecay = reader.ReadDateTime();
								break;
							}
					}
				}

				public static void Serialize(GenericWriter writer, TitleInfo info)
				{
					writer.WriteEncodedInt(0); // version

					writer.WriteEncodedInt(info.m_Value);
					writer.Write(info.m_LastDecay);
				}
			}

			private TitleInfo[] m_Values;

			private int m_Harrower; //Harrower titles do NOT decay

			public int GetValue(ChampionSpawnType type)
			{
				return GetValue((int)type);
			}

			public void SetValue(ChampionSpawnType type, int value)
			{
				SetValue((int)type, value);
			}

			public void Award(ChampionSpawnType type, int value)
			{
				Award((int)type, value);
			}

			public int GetValue(int index)
			{
				if (m_Values == null || index < 0 || index >= m_Values.Length)
				{
					return 0;
				}

				if (m_Values[index] == null)
				{
					m_Values[index] = new TitleInfo();
				}

				return m_Values[index].Value;
			}

			public DateTime GetLastDecay(int index)
			{
				if (m_Values == null || index < 0 || index >= m_Values.Length)
				{
					return DateTime.MinValue;
				}

				if (m_Values[index] == null)
				{
					m_Values[index] = new TitleInfo();
				}

				return m_Values[index].LastDecay;
			}

			public void SetValue(int index, int value)
			{
				if (m_Values == null)
				{
					m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];
				}

				if (value < 0)
				{
					value = 0;
				}

				if (index < 0 || index >= m_Values.Length)
				{
					return;
				}

				if (m_Values[index] == null)
				{
					m_Values[index] = new TitleInfo();
				}

				m_Values[index].Value = value;
			}

			public void Award(int index, int value)
			{
				if (m_Values == null)
				{
					m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];
				}

				if (index < 0 || index >= m_Values.Length || value <= 0)
				{
					return;
				}

				if (m_Values[index] == null)
				{
					m_Values[index] = new TitleInfo();
				}

				m_Values[index].Value += value;
			}

			public void Atrophy(int index, int value)
			{
				if (m_Values == null)
				{
					m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];
				}

				if (index < 0 || index >= m_Values.Length || value <= 0)
				{
					return;
				}

				if (m_Values[index] == null)
				{
					m_Values[index] = new TitleInfo();
				}

				var before = m_Values[index].Value;

				if ((m_Values[index].Value - value) < 0)
				{
					m_Values[index].Value = 0;
				}
				else
				{
					m_Values[index].Value -= value;
				}

				if (before != m_Values[index].Value)
				{
					m_Values[index].LastDecay = DateTime.UtcNow;
				}
			}

			public override string ToString()
			{
				return "...";
			}

			[CommandProperty(AccessLevel.GameMaster)]
			public int Pestilence { get => GetValue(ChampionSpawnType.Pestilence); set => SetValue(ChampionSpawnType.Pestilence, value); }

			[CommandProperty(AccessLevel.GameMaster)]
			public int Abyss { get => GetValue(ChampionSpawnType.Abyss); set => SetValue(ChampionSpawnType.Abyss, value); }

			[CommandProperty(AccessLevel.GameMaster)]
			public int Arachnid { get => GetValue(ChampionSpawnType.Arachnid); set => SetValue(ChampionSpawnType.Arachnid, value); }

			[CommandProperty(AccessLevel.GameMaster)]
			public int ColdBlood { get => GetValue(ChampionSpawnType.ColdBlood); set => SetValue(ChampionSpawnType.ColdBlood, value); }

			[CommandProperty(AccessLevel.GameMaster)]
			public int ForestLord { get => GetValue(ChampionSpawnType.ForestLord); set => SetValue(ChampionSpawnType.ForestLord, value); }

			[CommandProperty(AccessLevel.GameMaster)]
			public int SleepingDragon { get => GetValue(ChampionSpawnType.SleepingDragon); set => SetValue(ChampionSpawnType.SleepingDragon, value); }

			[CommandProperty(AccessLevel.GameMaster)]
			public int UnholyTerror { get => GetValue(ChampionSpawnType.UnholyTerror); set => SetValue(ChampionSpawnType.UnholyTerror, value); }

			[CommandProperty(AccessLevel.GameMaster)]
			public int VerminHorde { get => GetValue(ChampionSpawnType.VerminHorde); set => SetValue(ChampionSpawnType.VerminHorde, value); }

			[CommandProperty(AccessLevel.GameMaster)]
			public int Harrower { get => m_Harrower; set => m_Harrower = value; }

			public ChampionTitleInfo()
			{
			}

			public ChampionTitleInfo(GenericReader reader)
			{
				var version = reader.ReadEncodedInt();

				switch (version)
				{
					case 0:
						{
							m_Harrower = reader.ReadEncodedInt();

							var length = reader.ReadEncodedInt();
							m_Values = new TitleInfo[length];

							for (var i = 0; i < length; i++)
							{
								m_Values[i] = new TitleInfo(reader);
							}

							if (m_Values.Length != ChampionSpawnInfo.Table.Length)
							{
								var oldValues = m_Values;
								m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

								for (var i = 0; i < m_Values.Length && i < oldValues.Length; i++)
								{
									m_Values[i] = oldValues[i];
								}
							}
							break;
						}
				}
			}

			public static void Serialize(GenericWriter writer, ChampionTitleInfo titles)
			{
				writer.WriteEncodedInt(0); // version

				writer.WriteEncodedInt(titles.m_Harrower);

				var length = titles.m_Values.Length;
				writer.WriteEncodedInt(length);

				for (var i = 0; i < length; i++)
				{
					if (titles.m_Values[i] == null)
					{
						titles.m_Values[i] = new TitleInfo();
					}

					TitleInfo.Serialize(writer, titles.m_Values[i]);
				}
			}

			public static void CheckAtrophy(PlayerMobile pm)
			{
				var t = pm.m_ChampionTitles;
				if (t == null)
				{
					return;
				}

				if (t.m_Values == null)
				{
					t.m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];
				}

				for (var i = 0; i < t.m_Values.Length; i++)
				{
					if ((t.GetLastDecay(i) + LossDelay) < DateTime.UtcNow)
					{
						t.Atrophy(i, LossAmount);
					}
				}
			}

			public static void AwardHarrowerTitle(PlayerMobile pm)  //Called when killing a harrower.  Will give a minimum of 1 point.
			{
				var t = pm.m_ChampionTitles;
				if (t == null)
				{
					return;
				}

				if (t.m_Values == null)
				{
					t.m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];
				}

				var count = 1;

				for (var i = 0; i < t.m_Values.Length; i++)
				{
					if (t.m_Values[i].Value > 900)
					{
						count++;
					}
				}

				t.m_Harrower = Math.Max(count, t.m_Harrower);   //Harrower titles never decay.
			}
		}

		#endregion

		#region Recipes

		private Dictionary<int, bool> m_AcquiredRecipes;

		public virtual bool HasRecipe(Recipe r)
		{
			if (r == null)
			{
				return false;
			}

			return HasRecipe(r.ID);
		}

		public virtual bool HasRecipe(int recipeID)
		{
			if (m_AcquiredRecipes != null && m_AcquiredRecipes.ContainsKey(recipeID))
			{
				return m_AcquiredRecipes[recipeID];
			}

			return false;
		}

		public virtual void AcquireRecipe(Recipe r)
		{
			if (r != null)
			{
				AcquireRecipe(r.ID);
			}
		}

		public virtual void AcquireRecipe(int recipeID)
		{
			if (m_AcquiredRecipes == null)
			{
				m_AcquiredRecipes = new Dictionary<int, bool>();
			}

			m_AcquiredRecipes[recipeID] = true;
		}

		public virtual void ResetRecipes()
		{
			m_AcquiredRecipes = null;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int KnownRecipes
		{
			get
			{
				if (m_AcquiredRecipes == null)
				{
					return 0;
				}

				return m_AcquiredRecipes.Count;
			}
		}

		#endregion

		#region Buff Icons

		public void ResendBuffs()
		{
			if (!BuffInfo.Enabled || m_BuffTable == null)
			{
				return;
			}

			var state = NetState;

			if (state != null && state.BuffIcon)
			{
				foreach (var info in m_BuffTable.Values)
				{
					state.Send(new AddBuffPacket(this, info));
				}
			}
		}

		private Dictionary<BuffIcon, BuffInfo> m_BuffTable;

		public void AddBuff(BuffInfo b)
		{
			if (!BuffInfo.Enabled || b == null)
			{
				return;
			}

			RemoveBuff(b);  //Check & subsequently remove the old one.

			if (m_BuffTable == null)
			{
				m_BuffTable = new Dictionary<BuffIcon, BuffInfo>();
			}

			m_BuffTable.Add(b.ID, b);

			var state = NetState;

			if (state != null && state.BuffIcon)
			{
				state.Send(new AddBuffPacket(this, b));
			}
		}

		public void RemoveBuff(BuffInfo b)
		{
			if (b == null)
			{
				return;
			}

			RemoveBuff(b.ID);
		}

		public void RemoveBuff(BuffIcon b)
		{
			if (m_BuffTable == null || !m_BuffTable.ContainsKey(b))
			{
				return;
			}

			var info = m_BuffTable[b];

			if (info.Timer != null && info.Timer.Running)
			{
				info.Timer.Stop();
			}

			m_BuffTable.Remove(b);

			var state = NetState;

			if (state != null && state.BuffIcon)
			{
				state.Send(new RemoveBuffPacket(this, b));
			}

			if (m_BuffTable.Count <= 0)
			{
				m_BuffTable = null;
			}
		}

		#endregion

		public void AutoStablePets()
		{
			if (Core.SE && AllFollowers.Count > 0)
			{
				for (var i = m_AllFollowers.Count - 1; i >= 0; --i)
				{
					var pet = AllFollowers[i] as BaseCreature;

					if (pet == null || pet.ControlMaster == null)
					{
						continue;
					}

					if (pet.Summoned)
					{
						if (pet.Map != Map)
						{
							pet.PlaySound(pet.GetAngerSound());
							Timer.DelayCall(TimeSpan.Zero, pet.Delete);
						}
						continue;
					}

					if (pet is IMount mnt && mnt.Rider != null)
					{
						continue;
					}

					if (pet is IPackAnimal && pet.Backpack?.TotalItems > 0)
					{
						continue;
					}

					pet.ControlTarget = null;
					pet.ControlOrder = OrderType.Stay;
					pet.Internalize();

					pet.SetControlMaster(null);
					pet.SummonMaster = null;

					pet.IsStabled = true;
					pet.StabledBy = this;

					pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully happy

					Stabled.Add(pet);
					m_AutoStabled.Add(pet);
				}
			}
		}

		public void ClaimAutoStabledPets()
		{
			if (!Core.SE || m_AutoStabled.Count <= 0)
			{
				return;
			}

			if (!Alive)
			{
				SendLocalizedMessage(1076251); // Your pet was unable to join you while you are a ghost.  Please re-login once you have ressurected to claim your pets.
				return;
			}

			for (var i = m_AutoStabled.Count - 1; i >= 0; --i)
			{
				var pet = m_AutoStabled[i] as BaseCreature;

				if (pet == null || pet.Deleted)
				{
					pet.IsStabled = false;
					pet.StabledBy = null;

					if (Stabled.Contains(pet))
					{
						Stabled.Remove(pet);
					}

					continue;
				}

				if ((Followers + pet.ControlSlots) <= FollowersMax)
				{
					pet.SetControlMaster(this);

					if (pet.Summoned)
					{
						pet.SummonMaster = this;
					}

					pet.ControlTarget = this;
					pet.ControlOrder = OrderType.Follow;

					pet.MoveToWorld(Location, Map);

					pet.IsStabled = false;
					pet.StabledBy = null;

					pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully Happy

					if (Stabled.Contains(pet))
					{
						Stabled.Remove(pet);
					}
				}
				else
				{
					SendLocalizedMessage(1049612, pet.Name); // ~1_NAME~ remained in the stables because you have too many followers.
				}
			}

			m_AutoStabled.Clear();
		}
	}
}