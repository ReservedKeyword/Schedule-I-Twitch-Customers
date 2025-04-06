using MelonLoader;
using TwitchCustomers.NPC;

namespace TwitchCustomers.HarmonyPatches.LoadManager
{
  public class LoadManagerPatchLogic(Mod mod)
  {
    private readonly CachedNPCManager cachedNpcManager = mod.CachedNPCManger;
    private readonly MelonLogger.Instance log = mod.LoggerInstance;

    public void ExitToMenu_Postfix()
    {
      log.Msg("Player exited to main menu, clearing NPC cache...");
      cachedNpcManager.Clear();
    }
  }
}
