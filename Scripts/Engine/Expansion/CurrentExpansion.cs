﻿using Server.Accounting;
using Server.Network;

using System;

namespace Server
{
	public class CurrentExpansion
	{
		private static readonly Expansion Expansion = Expansion.EJ;

		[CallPriority(Int32.MinValue)]
		public static void Prepare()
		{
			Core.Expansion = Expansion;

			AccountGold.Enabled = Core.TOL;
			AccountGold.ConvertOnBank = true;
			AccountGold.ConvertOnTrade = false;
			VirtualCheck.UseEditGump = true;

			var Enabled = Core.AOS;

			Mobile.InsuranceEnabled = Enabled;
			ObjectPropertyList.Enabled = Enabled;
			Mobile.VisibleDamageType = Enabled ? VisibleDamageType.Related : VisibleDamageType.None;
			Mobile.GuildClickMessage = !Enabled;
			Mobile.AsciiClickMessage = !Enabled;

			if (Enabled)
			{
				AOS.DisableStatInfluences();

				if (ObjectPropertyList.Enabled)
				{
					PacketHandlers.SingleClickProps = true; // single click for everything is overriden to check object property list
				}

				Mobile.ActionDelay = 1000;
				Mobile.AOSStatusHandler = new AOSStatusHandler(AOS.GetStatus);
			}
		}
	}
}