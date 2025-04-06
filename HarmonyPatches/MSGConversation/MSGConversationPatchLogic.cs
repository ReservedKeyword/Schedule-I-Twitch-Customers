using MelonLoader;
using TwitchCustomers.NPC;
using UnityEngine;
using ScheduleOneMSGConversation = Il2CppScheduleOne.Messaging.MSGConversation;

namespace TwitchCustomers.HarmonyPatches.MSGConversation
{
  public class MSGConversationPatchLogic(Mod mod)
  {
    private readonly CachedNPCManager cachedNpcManager = mod.CachedNPCManger;
    private readonly MelonLogger.Instance log = mod.LoggerInstance;

    public void CreateUI_Postfix(ScheduleOneMSGConversation __instance)
    {
      Il2CppScheduleOne.NPCs.NPC gameNpc = __instance.sender;
      RectTransform entryTransform = __instance.entry;
      CachedNPC cachedNpc = cachedNpcManager.GetFromGameNPC(gameNpc);

      if (cachedNpc != null)
      {
        cachedNpc.UnityRectEntry = entryTransform;
        log.Msg($"NPC {gameNpc.GUID} already found cached, updated Unity container.");
        return;
      }

      cachedNpcManager.AddCachedNPC(new(gameNpc, entryTransform));
      log.Msg($"NPC {gameNpc.GUID} not found cached, cached for future use.");
    }
  }
}
