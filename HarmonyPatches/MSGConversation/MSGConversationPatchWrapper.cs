using HarmonyLib;
using ScheduleOneMSGConversation = ScheduleOne.Messaging.MSGConversation;

namespace TwitchCustomers.HarmonyPatches.MSGConversation
{
  [HarmonyPatch(typeof(ScheduleOneMSGConversation))]
  public static class MSGConversationPatchWrapper
  {
    private static MSGConversationPatchLogic patchLogic;

    public static void Initialize(MSGConversationPatchLogic patchLogic)
    {
      MSGConversationPatchWrapper.patchLogic = patchLogic;
    }

    [HarmonyPatch(nameof(ScheduleOneMSGConversation.CreateUI))]
    [HarmonyPostfix]
    public static void CreateUI_Postfix_Wrapper(ScheduleOneMSGConversation __instance)
    {
      patchLogic?.CreateUI_Postfix(__instance);
    }
  }
}
