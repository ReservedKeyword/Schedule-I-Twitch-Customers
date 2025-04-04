using BepInEx.Logging;
using TwitchCustomers.NPC;

namespace TwitchCustomers.HarmonyPatches.LoadManager
{
  public class LoadManagerPatchLogic(Plugin plugin)
  {
    private readonly CachedNPCManager cachedNpcManager = plugin.CachedNPCManager;
    private readonly ManualLogSource log = plugin.Log;

    public void ExitToMenu_Postfix()
    {
      log.LogInfo("Player exited to main menu, clearing NPC cache...");
      cachedNpcManager.Clear();
    }
  }
}
