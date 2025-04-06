using System.Collections;
using Il2CppScheduleOne.Messaging;
using Il2CppScheduleOne.NPCs;
using Il2CppSystem;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using ScheduleOneNPC = Il2CppScheduleOne.NPCs.NPC;

namespace TwitchCustomers.NPC
{
  public class CachedNPC
  {
    private static readonly Mod mod = Mod.Instance;
    private static readonly MelonLogger.Instance log = mod.LoggerInstance;

    public Guid NPCGUID { get; private set; }
    public RectTransform UnityRectEntry { get; set; }
    public OriginalNPCName OriginalNPCName { get; private set; }

    public CachedNPC(ScheduleOneNPC gameNpc)
    {
      NPCGUID = gameNpc.GUID;
      OriginalNPCName = new(gameNpc.FirstName, gameNpc.LastName);
    }

    public CachedNPC(ScheduleOneNPC gameNpc, RectTransform unityRectEntry)
    {
      NPCGUID = gameNpc.GUID;
      UnityRectEntry = unityRectEntry;
      OriginalNPCName = new(gameNpc.FirstName, gameNpc.LastName);
    }

    private Text GetConversationTextComponent()
    {
      if (UnityRectEntry == null)
        return null;

      Transform nameTransform = UnityRectEntry.Find(Constants.ENTRY_TRANSFORM_FIND_NAME);

      if (nameTransform == null)
      {
        log.Warning($"Could not find 'Name' child transform on {UnityRectEntry.name}.");
        return null;
      }

      Text nameTextComponent = nameTransform.GetComponent<Text>();

      if (nameTextComponent == null)
      {
        log.Warning($"'Name' transform on {UnityRectEntry.name} is missing Text component.");
        return null;
      }

      return nameTextComponent;
    }

    private ScheduleOneNPC GetGameNPC()
    {
      if (NPCManager.NPCRegistry == null)
      {
        log.Error($"Cannot fetch Game NPC, NPCRegistry was found null.");
        return null;
      }

      foreach (ScheduleOneNPC gameNpc in NPCManager.NPCRegistry)
      {
        if (gameNpc != null && gameNpc.GUID == NPCGUID)
        {
          return gameNpc;
        }
      }

      return null;
    }

    public void ResetCharacterName()
    {
      UpdateCharacterName(OriginalNPCName.FirstName, OriginalNPCName.LastName);
    }

    public void ResetConversationDisplayName()
    {
      UpdateConversationDisplayName($"{OriginalNPCName.FirstName} {OriginalNPCName.LastName}");
    }

    public void UpdateCharacterName(string firstName, string lastName = "")
    {
      ScheduleOneNPC gameNpc = GetGameNPC();

      if (gameNpc == null)
      {
        log.Error($"Failed to update character name, NPC not found in game registry.");
        return;
      }

      gameNpc.FirstName = firstName;
      gameNpc.LastName = lastName;
    }

    public void UpdateConversationDisplayName(string contactName)
    {
      if (MessagingManager.instance == null)
      {
        log.Error("MessagingManager.instance is null!");
        return;
      }

      ScheduleOneNPC gameNpc = GetGameNPC();

      if (gameNpc == null)
      {
        log.Error($"Failed to update conversation display name, NPC not found in game registry.");
        return;
      }

      MSGConversation msgConversation = MessagingManager.instance.GetConversation(gameNpc);

      if (msgConversation == null)
      {
        log.Warning(
          $"Could not get MSGConversation for {gameNpc?.GUID}. Cannot update contactName."
        );
        return;
      }

      msgConversation.contactName = contactName;
      MelonCoroutines.Start(UpdateConversationDisplayName_Coroutine(contactName));
    }

    private IEnumerator UpdateConversationDisplayName_Coroutine(string newDisplayName)
    {
      if (UnityRectEntry == null)
      {
        log.Warning($"Can't update conversation display name, UnityRectEntry was null.");
        yield break;
      }

      try
      {
        Text nameTextComponent = GetConversationTextComponent();

        if (nameTextComponent == null)
        {
          log.Warning($"Can't update conversation display name, nameTextComponent was null");
          yield break;
        }

        nameTextComponent.text = newDisplayName;
        UpdateConversationCategoryPadding();
        log.Msg($"NPC {NPCGUID} text component updated successfully!");
      }
      catch (System.Exception ex)
      {
        log.Error($"Exception while updating conversation display name: {ex}.");
      }
    }

    private void UpdateConversationCategoryPadding()
    {
      if (UnityRectEntry == null)
      {
        log.Warning($"Can't update conversation display name, UnityRectEntry was null.");
        return;
      }

      Text nameTextComponent = GetConversationTextComponent();

      if (nameTextComponent == null)
      {
        log.Warning($"Can't update conversation display name, nameTextComponent was null");
        return;
      }

      float nameComponentWidth = nameTextComponent.preferredWidth;
      RectTransform categoryTransform = UnityRectEntry
        .Find(Constants.ENTRY_TRANSFORM_FIND_CATEGORY)
        ?.GetComponent<RectTransform>();

      if (categoryTransform == null)
      {
        log.Warning(
          $"Could not find 'Category' child transform (or RectTransform) on {UnityRectEntry.name}."
        );
        return;
      }

      Vector2 currentPos = categoryTransform.anchoredPosition;
      currentPos.x = nameComponentWidth + Constants.ENTRY_TRANSFORM_CATEGORY_MARGIN_RIGHT;
      categoryTransform.anchoredPosition = currentPos;
    }
  }
}
