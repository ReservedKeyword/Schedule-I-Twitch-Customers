using BepInEx.Logging;
using HarmonyLib;
using TwitchCustomers.NPC;
using ScheduleOneCustomer = ScheduleOne.Economy.Customer;
using ScheduleOneNPC = ScheduleOne.NPCs.NPC;

namespace TwitchCustomers.HarmonyPatches.Customer
{
  [HarmonyPatch(typeof(ScheduleOneCustomer), nameof(ScheduleOneCustomer.CurrentContractEnded))]
  public static class CurrentContractEndedPatch
  {
    private static readonly Plugin plugin = Plugin.Instance;
    private static readonly ManualLogSource log = plugin.Log;
    private static readonly PluginConfig pluginConfig = plugin.PluginConfig;
    private static readonly CachedNPCManager cachedNpcManager = plugin.CachedNPCManager;

    public static void Postfix(ScheduleOneCustomer __instance)
    {
      if (!pluginConfig.PreserveOriginalNPCName.Value)
        return;

      ScheduleOneNPC gameNpc = __instance?.NPC;
      CachedNPC cachedNpc = cachedNpcManager.GetFromGameNPC(__instance?.NPC);

      if (cachedNpc == null)
      {
        log.LogWarning(
          $"Failed to find cached NPC {gameNpc.GUID}. Unable to reset after contract ended."
        );
        return;
      }

      cachedNpc.ResetCharacterName();
      cachedNpc.ResetConversationDisplayName();
    }
  }
}
