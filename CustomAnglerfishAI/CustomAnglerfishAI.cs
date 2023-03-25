using HarmonyLib;
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
			ModHelper.Console.WriteLine($"My mod {nameof(CustomAnglerfishAI)} is loaded!", MessageType.Success);
			Patches.ApplyPatches();

			// Example of accessing game code.
			LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
			{
				if (loadScene != OWScene.SolarSystem) return;
				ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
			};
		}
		public override void Configure(IModConfig config)
		{
			DebugLog("config changed; re-applying patches");
			/// AnglerPatches.acceleration = config.GetSettingsValue<int>("Acceleration");
			AnglerPatches.chaseSpeed = config.GetSettingsValue<int>("Chase Speed");
			AnglerPatches.investigateSpeed = config.GetSettingsValue<int>("Investigate Speed");
			AnglerPatches.turnSpeed = config.GetSettingsValue<int>("Turn Speed");
			AnglerPatches.quickTurnSpeed = config.GetSettingsValue<int>("Quick Turn Speed");
			/// AnglerPatches.consumeDeathDelay = config.GetSettingsValue<int>("Consume Death Delay");
			/// AnglerPatches.consumeShipCrushDelay = config.GetSettingsValue<int>("Consume Ship Crush Delay");
			AnglerPatches.deaf = config.GetSettingsValue<bool>("Deaf");
			AnglerPatches.mute = config.GetSettingsValue<bool>("Mute");
			AnglerPatches.meteorsHurt = config.GetSettingsValue<bool>("Meteor Launching Mod Integration");
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