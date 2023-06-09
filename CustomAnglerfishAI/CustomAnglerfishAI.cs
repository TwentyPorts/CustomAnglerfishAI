﻿using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Reflection;

namespace CustomAnglerfishAI
{
	public class CustomAnglerfishAI : ModBehaviour
	{
		public static CustomAnglerfishAI Instance;
		private void Awake()
		{
			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
			Instance = this;
		}

		private void Start()
		{
			// Starting here, you'll have access to OWML's mod helper.
			ModHelper.Console.WriteLine($"Mod {nameof(CustomAnglerfishAI)} is loaded!", MessageType.Success);
			AnglerPatches.meteorLaunchingOn = ModHelper.Interaction.ModExists("12090113.MeteorLaunching");
			Patches.ApplyPatches();
		}
		public override void Configure(IModConfig config)
		{
			DebugLog("config changed; re-applying patches");
			AnglerPatches.size = config.GetSettingsValue<int>("Size (%)")/100f;
			AnglerPatches.moveSpeed = config.GetSettingsValue<int>("Move Speed (%)")/100f;
			AnglerPatches.turnSpeed = config.GetSettingsValue<int>("Turn Speed (%)")/100f;
			AnglerPatches.distance = config.GetSettingsValue<int>("Distance (%)")/100f;
			/// AnglerPatches.consumeDeathDelay = config.GetSettingsValue<int>("Consume Death Delay");
			/// AnglerPatches.consumeShipCrushDelay = config.GetSettingsValue<int>("Consume Ship Crush Delay");
			AnglerPatches.deaf = config.GetSettingsValue<bool>("Deaf");
			AnglerPatches.mute = config.GetSettingsValue<bool>("Mute");
			AnglerPatches.afraid = config.GetSettingsValue<bool>("Afraid");
			AnglerPatches.rainbowLights = config.GetSettingsValue<bool>("Rainbow Lights");
			AnglerPatches.spinAxis = config.GetSettingsValue<string>("Spin Axis");
			AnglerPatches.meteorsHurt = config.GetSettingsValue<bool>("Meteor Launching Mod Integration");
			AnglerPatches.configChangedUpdateMovement = true;
		}

		internal void DebugLog(string str)
		{
#if DEBUG
			ModHelper.Console.WriteLine(str);
#endif
		}

		internal void DebugLog(string str, MessageType messageType)
		{
#if DEBUG
			ModHelper.Console.WriteLine(str, messageType);
#endif
		}
	}
}