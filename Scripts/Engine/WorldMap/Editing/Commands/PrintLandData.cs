﻿using Server;
using Server.Commands;
using Server.Commands.Generic;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Engine.Facet
{
	public partial class FacetEditingCommands
	{
		[Usage("PrintLandData")]
		[Description("Increases the Z value of a static.")]
		private static void printLandData_OnCommand(CommandEventArgs e)
		{
			Mobile from = e.Mobile;
			Map playerMap = from.Map;
			TileMatrix tm = playerMap.Tiles;

			int blocknum = (((from.Location.X >> 3) * tm.BlockHeight) + (from.Location.Y >> 3));
			byte[] data = BlockUtility.GetLandData(blocknum, playerMap.MapID);
			BlockUtility.WriteLandDataToConsole(data);
		}
	}
}