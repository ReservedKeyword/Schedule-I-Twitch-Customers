using System.Collections.Generic;
using BepInEx.Logging;

namespace TwitchCustomers.NPC
{
  public class CachedNPCManager(ManualLogSource log)
  {
    private readonly ManualLogSource Log = log;
    private readonly Dictionary<Il2CppSystem.Guid, CachedNPC> cachedNpcsByGuid = [];

    public bool AddCachedNPC(CachedNPC npc)
    {
      if (npc?.NPCGUID == null)
      {
        Log.LogWarning("Attempted to add a null CachedNPC or one with a null GameNPC.");
        return false;
      }

      Il2CppSystem.Guid guid = npc.NPCGUID;
      return cachedNpcsByGuid.TryAdd(guid, npc);
    }

    public void Clear()
    {
      cachedNpcsByGuid.Clear();
    }

    public CachedNPC GetFromGameNPC(ScheduleOne.NPCs.NPC gameNPC)
    {
      if (gameNPC == null)
        return null;

      Il2CppSystem.Guid guid = gameNPC.GUID;

      if (cachedNpcsByGuid.TryGetValue(guid, out CachedNPC cachedNPC))
        return cachedNPC;
      return null;
    }
  }
}
