using OWML.Utils;
using UnityEngine;
using System.Reflection;
using HarmonyLib;
using OWML.ModHelper;

namespace CustomAnglerfishAI
{
	public static class Patches
	{
		public static void ApplyPatches() => Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
	}

	[HarmonyPatch]
	public static class AnglerPatches
	{
		public static float size = CustomAnglerfishAI.Instance.ModHelper.Config.GetSettingsValue<int>("Size (%)")/100f;
		public static float moveSpeed = CustomAnglerfishAI.Instance.ModHelper.Config.GetSettingsValue<int>("Move Speed (%)")/100f;
		public static float turnSpeed = CustomAnglerfishAI.Instance.ModHelper.Config.GetSettingsValue<int>("Turn Speed (%)")/100f;
		public static float distance = CustomAnglerfishAI.Instance.ModHelper.Config.GetSettingsValue<int>("Distance (%)")/100f;
		/// public static int consumeDeathDelay = CustomAnglerfishAI.Instance.ModHelper.Config.GetSettingsValue<int>("Consume Death Delay");
		/// public static int consumeShipCrushDelay = CustomAnglerfishAI.Instance.ModHelper.Config.GetSettingsValue<int>("Consume Ship Crush Delay");
		public static bool deaf = CustomAnglerfishAI.Instance.ModHelper.Config.GetSettingsValue<bool>("Deaf");
		public static bool mute = CustomAnglerfishAI.Instance.ModHelper.Config.GetSettingsValue<bool>("Mute");
		public static bool afraid = CustomAnglerfishAI.Instance.ModHelper.Config.GetSettingsValue<bool>("Afraid");
		public static string spinAxis = CustomAnglerfishAI.Instance.ModHelper.Config.GetSettingsValue<string>("Spin Axis");
		public static bool meteorsHurt = CustomAnglerfishAI.Instance.ModHelper.Config.GetSettingsValue<bool>("Meteor Launching Mod Integration");

		public static bool meteorLaunchingOn = CustomAnglerfishAI.Instance.ModHelper.Interaction.ModExists("12090113.MeteorLaunching");

		[HarmonyPrefix]
		[HarmonyPatch(typeof(AnglerfishController), nameof(AnglerfishController.UpdateMovement))]
		public static void UpdateMovement(AnglerfishController __instance)
		{
			__instance.transform.localScale = new Vector3(size, size, size);
			CustomAnglerfishAI.Instance.DebugLog("current chase spd: " + __instance._chaseSpeed + " | invest spd: " + __instance._investigateSpeed + " | accel: " + __instance._acceleration + " | turn spd: " + __instance._turnSpeed + " | quick turn spd: " + __instance._quickTurnSpeed + " | arrive dist: " + __instance._arrivalDistance + " | pursue dist: " + __instance._pursueDistance + " | escape dist: " + __instance._escapeDistance);
			__instance._chaseSpeed = 75 * moveSpeed; // default value * multiplier
			__instance._investigateSpeed = 20 * moveSpeed;
			__instance._acceleration = 40 * moveSpeed;
			__instance._turnSpeed = 90 * turnSpeed;
			__instance._quickTurnSpeed = 360 * turnSpeed;
			__instance._arrivalDistance = 100 * distance;
			__instance._pursueDistance = 300 * distance;
			__instance._escapeDistance = 400 * distance;
			/// __instance._consumeDeathDelay = consumeDeathDelay;
			/// __instance._consumeShipCrushDelay = consumeShipCrushDelay;
			
			if (spinAxis == "None")
			{
				// do nothing
			}
			else if (spinAxis == "X")
			{
				__instance.transform.Rotate(__instance._quickTurnSpeed * Time.deltaTime, 0, 0);
			}
			else if(spinAxis == "Y")
			{
				__instance.transform.Rotate(0, __instance._quickTurnSpeed * Time.deltaTime, 0);
			}
			else if (spinAxis == "Z")
			{
				__instance.transform.Rotate(0, 0, __instance._quickTurnSpeed * Time.deltaTime);
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(AnglerfishController), nameof(AnglerfishController.RotateTowardsTarget))]
		public static void RotateTowardsTarget(AnglerfishController __instance, ref Vector3 targetPos)
		{
			if (afraid && targetPos.Equals(__instance._targetPos))
			{
				targetPos = 2 * __instance._anglerBody.GetPosition() - targetPos;
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(AnglerfishController), nameof(AnglerfishController.MoveTowardsTarget))]
		public static void MoveTowardsTarget(AnglerfishController __instance, ref Vector3 targetPos)
		{
			if (afraid && targetPos.Equals(__instance._targetPos))
			{
				targetPos = 2 * __instance._anglerBody.GetPosition() - targetPos;
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(AnglerfishController), nameof(AnglerfishController.OnClosestAudibleNoise))]
		public static bool OnClosestAudibleNoise()
		{
			return !deaf;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(AnglerfishAudioController), nameof(AnglerfishAudioController.OnChangeAnglerState))]
		public static bool OnChangeAnglerState(AnglerfishAudioController __instance)
		{
			if(mute) __instance._loopSource.mute = true;
			/// CustomAnglerfishAI.Instance.DebugLog("muted angler audio");
			return !mute;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(AnglerfishAudioController), nameof(AnglerfishAudioController.UpdateLoopingAudio))]
		public static bool UpdateLoopingAudio()
		{
			return !mute;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(MeteorController), nameof(MeteorController.OnCollisionEnter))]
		public static void OnCollisionEnter(MeteorController __instance, Collision collision)
		{
			CustomAnglerfishAI.Instance.DebugLog("meteor mod detected?: " + meteorLaunchingOn);
			if (meteorLaunchingOn && meteorsHurt)
			{
				CustomAnglerfishAI.Instance.DebugLog("METEOR HIT: " + collision.transform.name);
				if (collision.gameObject.name == "Anglerfish_Body")
				{
					AnglerfishController angler = collision.gameObject.GetComponent<AnglerfishController>();
					CustomAnglerfishAI.Instance.DebugLog(angler != null ? "angler controller FOUND" : "no angler controller found");
					angler.ChangeState(AnglerfishController.AnglerState.Stunned);
					var colliderName = collision.collider.name;
					switch (colliderName)
					{
						case "Beast_Anglerfish_Collider_MouthFloor":
							angler._stunTimer = 3f;
							break;
						case "Beast_Anglerfish_Collider_Mouth":
							angler._stunTimer = 3f;
							break;
						case "Beast_Anglerfish_Collider_LeftCheek":
							angler._stunTimer = 2f;
							break;
						case "Beast_Anglerfish_Collider_RightCheek":
							angler._stunTimer = 2f;
							break;
						case "Beast_Anglerfish_Collider_Body":
							angler._stunTimer = 1f;
							break;
					}
				}
			}
		}
	}
}