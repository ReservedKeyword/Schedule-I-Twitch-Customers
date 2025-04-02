using BepInEx.Logging;
using HarmonyLib;
using TwitchCustomers.NPC;
using UnityEngine;
using ScheduleOneMSGConversation = ScheduleOne.Messaging.MSGConversation;

namespace TwitchCustomers.HarmonyPatches.MSGConversation
{
  [HarmonyPatch(typeof(ScheduleOneMSGConversation), nameof(ScheduleOneMSGConversation.CreateUI))]
  public static class CreateUIPatch
  {
    private static readonly Plugin plugin = Plugin.Instance;
    private static readonly ManualLogSource log = plugin.Log;
    private static readonly CachedNPCManager cachedNpcManager = plugin.CachedNPCManager;

    [HarmonyPostfix]
    public static void Postfix(ScheduleOneMSGConversation __instance)
    {
      ScheduleOne.NPCs.NPC gameNpc = __instance.sender;
      RectTransform entryTransform = __instance.entry;
      CachedNPC cachedNpc = cachedNpcManager.GetFromGameNPC(gameNpc);

      if (cachedNpc != null)
      {
        cachedNpc.UnityRectEntry = entryTransform;
        log.LogInfo($"NPC {gameNpc.GUID} already found cached, updated Unity container.");
        return;
      }

      cachedNpcManager.AddCachedNPC(new(gameNpc, entryTransform));
      log.LogInfo($"NPC {gameNpc.GUID} not found cached, cached for future use.");
    }
  }
}
