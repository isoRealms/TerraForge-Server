﻿using Server.Network;

using System;
using System.Collections.Generic;

namespace Server.Misc
{
	public static partial class Assistants
	{
		private static class Settings
		{
			public static readonly bool Enabled = false;
			public static readonly bool KickOnFailure = true; // It will also kick clients running without assistants

			public static readonly TimeSpan HandshakeTimeout = TimeSpan.FromSeconds(30.0);
			public static readonly TimeSpan DisconnectDelay = TimeSpan.FromSeconds(15.0);

			public const string WarningMessage = "The server was unable to negotiate features with your assistant. "
								+ "You must download and run an updated version of <A HREF=\"http://uosteam.com\">UOSteam</A>"
								+ " or <A HREF=\"https://bitbucket.org/msturgill/razor-releases/downloads\">Razor</A>."
								+ "<BR><BR>Make sure you've checked the option <B>Negotiate features with server</B>, "
								+ "once you have this box checked you may log in and play normally."
								+ "<BR><BR>You will be disconnected shortly.";

			public static void Configure()
			{
				//DisallowFeature( Features.FilterWeather );
			}

			[Flags]
			public enum Features : ulong
			{
				None = 0ul,

				FilterWeather = 1ul << 0,  // Weather Filter
				FilterLight = 1ul << 1,  // Light Filter
				SmartTarget = 1ul << 2,  // Smart Last Target
				RangedTarget = 1ul << 3,  // Range Check Last Target
				AutoOpenDoors = 1ul << 4,  // Automatically Open Doors
				DequipOnCast = 1ul << 5,  // Unequip Weapon on spell cast
				AutoPotionEquip = 1ul << 6,  // Un/re-equip weapon on potion use
				PoisonedChecks = 1ul << 7,  // Block heal If poisoned/Macro If Poisoned condition/Heal or Cure self
				LoopedMacros = 1ul << 8,  // Disallow looping or recursive macros
				UseOnceAgent = 1ul << 9,  // The use once agent
				RestockAgent = 1ul << 10, // The restock agent
				SellAgent = 1ul << 11, // The sell agent
				BuyAgent = 1ul << 12, // The buy agent
				PotionHotkeys = 1ul << 13, // All potion hotkeys
				RandomTargets = 1ul << 14, // All random target hotkeys (not target next, last target, target self)
				ClosestTargets = 1ul << 15, // All closest target hotkeys
				OverheadHealth = 1ul << 16, // Health and Mana/Stam messages shown over player's heads
				AutolootAgent = 1ul << 17, // The autoloot agent
				BoneCutterAgent = 1ul << 18, // The bone cutter agent
				AdvancedMacros = 1ul << 19, // Advanced macro engine
				AutoRemount = 1ul << 20, // Auto remount after dismount
				AutoBandage = 1ul << 21, // Auto bandage friends, self, last and mount option
				EnemyTargetShare = 1ul << 22, // Enemy target share on guild, party or alliance chat
				FilterSeason = 1ul << 23, // Season Filter
				SpellTargetShare = 1ul << 24, // Spell target share on guild, party or alliance chat

				All = ~None
			}

			private static Features m_DisallowedFeatures = Features.None;

			public static void DisallowFeature(Features feature)
			{
				SetDisallowed(feature, true);
			}

			public static void AllowFeature(Features feature)
			{
				SetDisallowed(feature, false);
			}

			public static void SetDisallowed(Features feature, bool value)
			{
				if (value)
				{
					m_DisallowedFeatures |= feature;
				}
				else
				{
					m_DisallowedFeatures &= ~feature;
				}
			}

			public static Features DisallowedFeatures => m_DisallowedFeatures;
		}

		private static class Negotiator
		{
			private static readonly Dictionary<Mobile, Timer> m_Dictionary = new Dictionary<Mobile, Timer>();

			public static void Initialize()
			{
				if (Settings.Enabled)
				{
					EventSink.Login += EventSink_Login;

					ProtocolExtensions.Register(0xFF, true, OnHandshakeResponse);
				}
			}

			private static void EventSink_Login(LoginEventArgs e)
			{
				var m = e.Mobile;

				if (m != null)
				{
					if (m_Dictionary.TryGetValue(m, out var t))
					{
						t?.Stop();

						m_Dictionary.Remove(m);
					}

					if (m.NetState != null && m.NetState.Running)
					{
						m.Send(new BeginHandshake());

						if (Settings.KickOnFailure)
						{
							m.Send(new BeginHandshake());
						}

						m_Dictionary[m] = Timer.DelayCall(Settings.HandshakeTimeout, OnHandshakeTimeout, m);
					}
				}
			}

			private static void OnHandshakeResponse(NetState state, PacketReader pvSrc)
			{
				if (state == null || !state.Running)
				{
					return;
				}

				var m = state.Mobile;

				if (m != null && m_Dictionary.TryGetValue(m, out var t))
				{
					t?.Stop();

					m_Dictionary.Remove(m);
				}
			}

			private static void OnHandshakeTimeout(Mobile m)
			{
				if (m_Dictionary.TryGetValue(m, out var t))
				{
					t?.Stop();
				}

				if (!Settings.KickOnFailure)
				{
					Console.WriteLine("Player '{0}' failed to negotiate features.", m);
				}
				else if (m.NetState != null && m.NetState.Running)
				{
					m.SendGump(new Gumps.WarningGump(1060635, 30720, Settings.WarningMessage, 0xFFC000, 420, 250, null, null));

					if (m.AccessLevel <= AccessLevel.Player)
					{
						m_Dictionary[m] = Timer.DelayCall(Settings.DisconnectDelay, OnForceDisconnect, m);
					}
				}
			}

			private static void OnForceDisconnect(Mobile m)
			{
				if (m.NetState != null && m.NetState.Running)
				{
					m.NetState.Dispose();
				}

				if (m_Dictionary.TryGetValue(m, out var t))
				{
					t?.Stop();

					m_Dictionary.Remove(m);
				}

				Console.WriteLine("Player {0} kicked (Failed assistant handshake)", m);
			}

			private sealed class BeginHandshake : ProtocolExtension
			{
				public BeginHandshake()
					: base(0xFE, 8)
				{
					m_Stream.Write((uint)((ulong)Settings.DisallowedFeatures >> 32));
					m_Stream.Write((uint)((ulong)Settings.DisallowedFeatures & 0xFFFFFFFF));
				}
			}
		}
	}
}