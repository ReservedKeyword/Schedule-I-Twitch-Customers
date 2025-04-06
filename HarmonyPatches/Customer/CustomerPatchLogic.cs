using MelonLoader;
using TwitchCustomers.NPC;
using TwitchCustomers.TwitchIntegration;
using ScheduleOneCustomer = Il2CppScheduleOne.Economy.Customer;
using ScheduleOneNPC = Il2CppScheduleOne.NPCs.NPC;

namespace TwitchCustomers.HarmonyPatches.Customer
{
  public class CustomerPatchLogic(Mod mod)
  {
    private readonly CachedNPCManager cachedNpcManager = mod.CachedNPCManger;
    private readonly ChatterManager chatterManager = mod.ChatterManager;
    private readonly MelonLogger.Instance log = mod.LoggerInstance;
    private readonly ModConfig modConfig = mod.ModConfig;

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

    public void ExpireOffer_Prefix(ScheduleOneCustomer __instance)
    {
      ResetCustomerIfPreserveOriginalNPC(__instance);
    }

    public void NotifyPlayerOfContract_Prefix(ScheduleOneCustomer __instance)
    {
      string randomChatter = chatterManager.GetRandomChatter();

      if (string.IsNullOrEmpty(randomChatter))
      {
        log.Warning("No chatter found for contract notification, skipping name update.");
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
      log.Msg($"Updated in-game NPC with Twitch chatter: {randomChatter}.");
    }

    private void ResetCustomerIfPreserveOriginalNPC(ScheduleOneCustomer customer)
    {
      if (!modConfig.PreserveOriginalNPCName)
        return;

      ScheduleOneNPC gameNpc = customer.NPC;
      CachedNPC cachedNpc = cachedNpcManager.GetFromGameNPC(gameNpc);

      if (cachedNpc == null)
      {
        log.Warning($"Failed to find cached NPC {gameNpc.GUID}. Unable to reset customer.");
        return;
      }

      cachedNpc.ResetCharacterName();
      cachedNpc.ResetConversationDisplayName();
    }
  }
}
