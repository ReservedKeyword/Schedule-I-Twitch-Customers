using BepInEx.Logging;
using HarmonyLib;
using TwitchCustomers.NPC;
using TwitchCustomers.TwitchIntegration;
using ScheduleOneCustomer = ScheduleOne.Economy.Customer;

namespace TwitchCustomers.HarmonyPatches.Customer
{
  [HarmonyPatch(typeof(ScheduleOneCustomer), nameof(ScheduleOneCustomer.NotifyPlayerOfContract))]
  public static class NotifyPlayerOfContractPatch
  {
    private static readonly Plugin plugin = Plugin.Instance;
    private static readonly ManualLogSource log = plugin.Log;
    private static readonly ChatterManager chatterManager = plugin.ChatterManager;
    private static readonly CachedNPCManager cachedNpcManager = plugin.CachedNPCManager;

    public static void Prefix(ScheduleOneCustomer __instance)
    {
      string randomChatter = chatterManager.GetRandomChatter();

      if (string.IsNullOrEmpty(randomChatter))
      {
        log.LogWarning("No chatter found for contract notification, skipping name update.");
        return;
      }

      ScheduleOne.NPCs.NPC gameNpc = __instance?.NPC;
      CachedNPC cachedNpc = cachedNpcManager.GetFromGameNPC(gameNpc);

      if (cachedNpc == null)
      {
        cachedNpc = new CachedNPC(gameNpc);
        cachedNpcManager.AddCachedNPC(cachedNpc);
      }

      cachedNpc.UpdateCharacterName(randomChatter);
      cachedNpc.UpdateConversationDisplayName(randomChatter);
      log.LogInfo($"Updated in-game NPC with Twitch chatter: {randomChatter}.");
    }
  }
}
