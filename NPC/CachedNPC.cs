using BepInEx.Logging;
using ScheduleOne.Messaging;
using ScheduleOne.NPCs;
using TwitchCustomers.Coroutines;
using UnityEngine;
using UnityEngine.UI;
using ScheduleOneNPC = ScheduleOne.NPCs.NPC;

namespace TwitchCustomers.NPC
{
  public class CachedNPC
  {
    private static readonly Plugin plugin = Plugin.Instance;
    private static readonly ManualLogSource log = plugin.Log;
    private static readonly ConversationNameCoroutineRunner conversationNameCoroutineRunner =
      ConversationNameCoroutineRunner.Instance;

    public Il2CppSystem.Guid NPCGUID { get; private set; }
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

    public Text GetConversationTextComponent()
    {
      if (UnityRectEntry == null)
        return null;

      Transform nameTransform = UnityRectEntry.Find(Constants.ENTRY_TRANSFORM_FIND_NAME);

      if (nameTransform == null)
      {
        log.LogWarning($"Could not find 'Name' child transform on {UnityRectEntry.name}.");
        return null;
      }

      Text nameTextComponent = nameTransform.GetComponent<Text>();

      if (nameTextComponent == null)
      {
        log.LogWarning($"'Name' transform on {UnityRectEntry.name} is missing Text component.");
        return null;
      }
      return nameTextComponent;
    }

    private ScheduleOneNPC GetGameNPC()
    {
      if (NPCManager.NPCRegistry == null)
      {
        log.LogError($"Cannot fetch Game NPC, NPCRegistry was found null.");
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
        log.LogError($"Failed to update character name, NPC not found in game registry.");
        return;
      }

      gameNpc.FirstName = firstName;
      gameNpc.LastName = lastName;
    }

    public void UpdateConversationDisplayName(string contactName)
    {
      if (MessagingManager.instance == null)
      {
        log.LogError("MessagingManager.instance is null!");
        return;
      }

      ScheduleOneNPC gameNpc = GetGameNPC();

      if (gameNpc == null)
      {
        log.LogError(
          $"Failed to update conversation display name, NPC not found in game registry."
        );
        return;
      }

      MSGConversation msgConversation = MessagingManager.instance.GetConversation(gameNpc);

      if (msgConversation == null)
      {
        log.LogWarning(
          $"Could not get MSGConversation for {gameNpc?.GUID}. Cannot update contactName."
        );
        return;
      }

      msgConversation.contactName = contactName;

      if (UnityRectEntry != null && conversationNameCoroutineRunner != null)
        conversationNameCoroutineRunner.StartDelayedConversationNameUpdate(this, contactName);
    }

    public void UpdateConversationCategoryPadding()
    {
      if (UnityRectEntry == null)
        return;

      Text nameTextComponent = GetConversationTextComponent();

      if (nameTextComponent == null)
        return;

      float nameComponentWidth = nameTextComponent.preferredWidth;
      RectTransform categoryTransform = UnityRectEntry
        .Find(Constants.ENTRY_TRANSFORM_FIND_CATEGORY)
        ?.GetComponent<RectTransform>();

      if (categoryTransform == null)
      {
        log.LogWarning(
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
