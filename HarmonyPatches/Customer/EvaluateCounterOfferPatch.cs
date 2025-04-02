using BepInEx.Logging;
using HarmonyLib;
using ScheduleOne.Product;
using TwitchCustomers.NPC;
using ScheduleOneCustomer = ScheduleOne.Economy.Customer;
using ScheduleOneNPC = ScheduleOne.NPCs.NPC;

namespace TwitchCustomers.HarmonyPatches.Customer
{
  [HarmonyPatch(typeof(ScheduleOneCustomer), nameof(ScheduleOneCustomer.EvaluateCounteroffer))]
  public static class EvaluateCounterOfferPatch
  {
    private static readonly Plugin plugin = Plugin.Instance;
    private static readonly ManualLogSource log = plugin.Log;
    private static readonly PluginConfig pluginConfig = plugin.PluginConfig;
    private static readonly CachedNPCManager cachedNpcManager = plugin.CachedNPCManager;

    public static void Postfix(
      ScheduleOneCustomer __instance,
      ProductDefinition product,
      int quantity,
      float price,
      ref bool __result
    )
    {
      if (!pluginConfig.PreserveOriginalNPCName.Value)
        return;

      if (!__result)
      {
        ScheduleOneNPC gameNpc = __instance?.NPC;
        CachedNPC cachedNpc = cachedNpcManager.GetFromGameNPC(__instance?.NPC);

        if (cachedNpc == null)
        {
          log.LogWarning(
            $"Failed to find cached NPC {gameNpc.GUID}. Unable to reset after customer rejected deal."
          );
          return;
        }

        cachedNpc.ResetCharacterName();
        cachedNpc.ResetConversationDisplayName();
      }
    }
  }
}
