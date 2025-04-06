using HarmonyLib;
using ScheduleOneLoadManager = Il2CppScheduleOne.Persistence.LoadManager;

namespace TwitchCustomers.HarmonyPatches.LoadManager
{
  [HarmonyPatch(typeof(ScheduleOneLoadManager))]
  public static class LoadManagerPatchWrapper
  {
    private static LoadManagerPatchLogic patchLogic;

    public static void Initialize(LoadManagerPatchLogic patchLogic)
    {
      LoadManagerPatchWrapper.patchLogic = patchLogic;
    }

    [HarmonyPatch(nameof(ScheduleOneLoadManager.ExitToMenu))]
    [HarmonyPostfix]
    public static void ExitToMenu_Postfix_Wrapper()
    {
      patchLogic?.ExitToMenu_Postfix();
    }
  }
}
