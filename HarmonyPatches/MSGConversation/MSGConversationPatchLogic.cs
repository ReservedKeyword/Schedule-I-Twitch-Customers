using BepInEx.Logging;
using TwitchCustomers.NPC;
using UnityEngine;
using ScheduleOneMSGConversation = ScheduleOne.Messaging.MSGConversation;

namespace TwitchCustomers.HarmonyPatches.MSGConversation
{
  public class MSGConversationPatchLogic(Plugin plugin)
  {
    private readonly CachedNPCManager cachedNpcManager = plugin.CachedNPCManager;
    private readonly ManualLogSource log = plugin.Log;

    public void CreateUI_Postfix(ScheduleOneMSGConversation __instance)
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
