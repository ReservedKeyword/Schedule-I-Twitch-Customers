using BepInEx.Logging;
using HarmonyLib;
using TwitchCustomers.NPC;
using ScheduleOneCustomer = ScheduleOne.Economy.Customer;
using ScheduleOneNPC = ScheduleOne.NPCs.NPC;

namespace TwitchCustomers.HarmonyPatches.Customer
{
  [HarmonyPatch(typeof(ScheduleOneCustomer), nameof(ScheduleOneCustomer.ExpireOffer))]
  public static class ExpireOfferPatch
  {
    private static readonly Plugin plugin = Plugin.Instance;
    private static readonly ManualLogSource log = plugin.Log;
    private static readonly PluginConfig pluginConfig = plugin.PluginConfig;
    private static readonly CachedNPCManager cachedNpcManager = plugin.CachedNPCManager;

    public static void Prefix(ScheduleOneCustomer __instance)
    {
      if (!pluginConfig.PreserveOriginalNPCName.Value)
        return;

      ScheduleOneNPC gameNpc = __instance?.NPC;
      CachedNPC cachedNpc = cachedNpcManager.GetFromGameNPC(gameNpc);

      if (cachedNpc == null)
      {
        log.LogWarning(
          $"Failed to find cached NPC {gameNpc.GUID}. Unable to reset after offer expired."
        );
        return;
      }

      cachedNpc.ResetCharacterName();
      cachedNpc.ResetConversationDisplayName();
    }
  }
}
