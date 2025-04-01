using BepInEx.Logging;
using HarmonyLib;
using ScheduleOne.Economy;
using TwitchCustomers.NPC;

namespace TwitchCustomers.HarmonyPatches
{
  [HarmonyPatch(typeof(Customer), nameof(Customer.ProcessHandover))]
  public static class ProcessHandoverPatch
  {
    private static readonly Plugin plugin = Plugin.Instance;
    private static readonly ManualLogSource log = plugin.Log;
    private static readonly PluginConfig pluginConfig = plugin.PluginConfig;
    private static readonly CachedNPCManager cachedNpcManager = plugin.CachedNPCManager;

    public static void Postfix(Customer __instance)
    {
      if (!pluginConfig.PreserveOriginalNPCName.Value)
        return;

      ScheduleOne.NPCs.NPC gameNpc = __instance.NPC;
      CachedNPC cachedNpc = cachedNpcManager.GetFromGameNPC(gameNpc);

      if (cachedNpc == null)
      {
        log.LogWarning($"Failed to find cached NPC {gameNpc.GUID}, skipping...");
        return;
      }

      cachedNpc.ResetCharacterName();
      cachedNpc.ResetConversationDisplayName();
    }
  }
}
