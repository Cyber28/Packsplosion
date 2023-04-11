using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace PacksplosionMod
{
	[BepInPlugin("Packsplosion", "Packsplosion", "1.0.0")]
	public class Packsplosion : BaseUnityPlugin
	{
		public static BepInEx.Logging.ManualLogSource L;
		public static Harmony Harmony;
		public static ConfigEntry<float> cfgDelay;

		private void Awake()
		{
			L = Logger;
			cfgDelay = Config.Bind("Settings", "Delay", 0.05f, "The time (in seconds) to wait before spawning the next card");
			Harmony = Harmony.CreateAndPatchAll(typeof(Patches), "Packsplosion");
		}

		public static IEnumerator Delay(Boosterpack bp)
		{
			yield return new WaitForSeconds(0.05f);
			bp.Clicked();
		}
	}

	public static class Patches
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(Boosterpack), "Clicked")]
		public static bool CheckShit(Boosterpack __instance)
		{
			return __instance.TimesOpened != __instance.TotalCardsInPack;
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Boosterpack), "Clicked")]
		public static void Explosion(Boosterpack __instance)
		{
			if (__instance.TimesOpened == __instance.TotalCardsInPack)
				return;
			WorldManager.instance.StartCoroutine(Packsplosion.Delay(__instance));
		}
	}
}