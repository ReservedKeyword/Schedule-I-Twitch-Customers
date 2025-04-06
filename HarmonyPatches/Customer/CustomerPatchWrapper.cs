using HarmonyLib;
using ScheduleOneCustomer = Il2CppScheduleOne.Economy.Customer;

namespace TwitchCustomers.HarmonyPatches.Customer
{
  [HarmonyPatch(typeof(ScheduleOneCustomer))]
  public static class CustomerPatchWrapper
  {
    private static CustomerPatchLogic patchLogic;

    public static void Initialize(CustomerPatchLogic patchLogic)
    {
      CustomerPatchWrapper.patchLogic = patchLogic;
    }

    [HarmonyPatch(nameof(ScheduleOneCustomer.ContractRejected))]
    [HarmonyPostfix]
    public static void ContractRejected_Postfix_Wrapper(ScheduleOneCustomer __instance)
    {
      patchLogic?.ContractRejected_Postfix(__instance);
    }

    [HarmonyPatch(nameof(ScheduleOneCustomer.CurrentContractEnded))]
    [HarmonyPostfix]
    public static void CurrentContractEnded_Postfix_Wrapper(ScheduleOneCustomer __instance)
    {
      patchLogic?.CurrentContractEnded_Postfix(__instance);
    }

    [HarmonyPatch(nameof(ScheduleOneCustomer.EvaluateCounteroffer))]
    [HarmonyPostfix]
    public static void EvaluateCounterOffer_Postfix_Wrapper(
      ScheduleOneCustomer __instance,
      bool __result
    )
    {
      patchLogic?.EvaluateCounterOffer_Postfix(__instance, __result);
    }

    [HarmonyPatch(nameof(ScheduleOneCustomer.ExpireOffer))]
    [HarmonyPrefix]
    public static void ExpireOffer_Prefix_Wrapper(ScheduleOneCustomer __instance)
    {
      patchLogic?.ExpireOffer_Prefix(__instance);
    }

    [HarmonyPatch(nameof(ScheduleOneCustomer.NotifyPlayerOfContract))]
    [HarmonyPrefix]
    public static void NotifyPlayerOfContract_Prefix_Wrapper(ScheduleOneCustomer __instance)
    {
      patchLogic?.NotifyPlayerOfContract_Prefix(__instance);
    }
  }
}
