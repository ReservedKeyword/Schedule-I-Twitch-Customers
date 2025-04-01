using BepInEx.Logging;
using HarmonyLib;
using ScheduleOne.Persistence;
using TwitchCustomers.NPC;

namespace TwitchCustomers.HarmonyPatches
{
  [HarmonyPatch(typeof(LoadManager), nameof(LoadManager.ExitToMenu))]
  public static class ExitToMenuPatch
  {
    private static readonly Plugin plugin = Plugin.Instance;
    private static readonly ManualLogSource log = plugin.Log;
    private static readonly CachedNPCManager cachedNpcManager = plugin.CachedNPCManager;

    public static void Postfix()
    {
      log.LogInfo("Player exited to main menu, clearing NPC cache...");
      cachedNpcManager.Clear();
    }
  }
}
