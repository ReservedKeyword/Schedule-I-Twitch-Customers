using BepInEx.Logging;
using TwitchCustomers.NPC;
using TwitchCustomers.TwitchIntegration;
using ScheduleOneCustomer = ScheduleOne.Economy.Customer;
using ScheduleOneNPC = ScheduleOne.NPCs.NPC;

namespace TwitchCustomers.HarmonyPatches.Customer
{
  public class CustomerPatchLogic(Plugin plugin)
  {
    private readonly CachedNPCManager cachedNpcManager = plugin.CachedNPCManager;
    private readonly ChatterManager chatterManager = plugin.ChatterManager;
    private readonly ManualLogSource log = plugin.Log;
    private readonly PluginConfig pluginConfig = plugin.PluginConfig;

    public void ContractRejected_Postfix(ScheduleOneCustomer __instance)
    {
      ResetCustomerIfPreserveOriginalNPC(__instance);
    }

    public void CurrentContractEnded_Postfix(ScheduleOneCustomer __instance)
    {
      ResetCustomerIfPreserveOriginalNPC(__instance);
    }

    public void EvaluateCounterOffer_Postfix(ScheduleOneCustomer __instance, bool __result)
    {
      if (!__result)
      {
        ResetCustomerIfPreserveOriginalNPC(__instance);
      }
    }

    public void ExpireOffer_Postfix(ScheduleOneCustomer __instance)
    {
      ResetCustomerIfPreserveOriginalNPC(__instance);
    }

    public void NotifyPlayerOfContract_Prefix(ScheduleOneCustomer __instance)
    {
      string randomChatter = chatterManager.GetRandomChatter();

      if (string.IsNullOrEmpty(randomChatter))
      {
        log.LogWarning("No chatter found for contract notification, skipping name update.");
        return;
      }

      ScheduleOneNPC gameNpc = __instance?.NPC;
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

    private void ResetCustomerIfPreserveOriginalNPC(ScheduleOneCustomer customer)
    {
      if (!pluginConfig.PreserveOriginalNPCName.Value)
        return;

      ScheduleOneNPC gameNpc = customer.NPC;
      CachedNPC cachedNpc = cachedNpcManager.GetFromGameNPC(gameNpc);

      if (cachedNpc == null)
      {
        log.LogWarning($"Failed to find cached NPC {gameNpc.GUID}. Unable to reset customer.");
        return;
      }

      cachedNpc.ResetCharacterName();
      cachedNpc.ResetConversationDisplayName();
    }
  }
}
