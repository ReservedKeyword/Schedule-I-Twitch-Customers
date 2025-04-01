using BepInEx.Logging;
using ScheduleOne.Messaging;
using TwitchCustomers.Coroutines;
using UnityEngine;
using UnityEngine.UI;

namespace TwitchCustomers.NPC
{
  public class CachedNPC
  {
    private static readonly Plugin plugin = Plugin.Instance;
    private static readonly ManualLogSource log = plugin.Log;
    private static readonly ConversationNameCoroutineRunner conversationNameCoroutineRunner =
      ConversationNameCoroutineRunner.Instance;

    public ScheduleOne.NPCs.NPC GameNPC;
    public RectTransform UnityRectEntry;
    public OriginalNPCName OriginalNPCName;

    public CachedNPC(ScheduleOne.NPCs.NPC gameNPC)
    {
      GameNPC = gameNPC;
      OriginalNPCName = new(GameNPC.FirstName, GameNPC.LastName);
    }

    public CachedNPC(ScheduleOne.NPCs.NPC gameNPC, RectTransform unityRectEntry)
    {
      GameNPC = gameNPC;
      UnityRectEntry = unityRectEntry;
      OriginalNPCName = new(GameNPC.FirstName, GameNPC.LastName);
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
      GameNPC.FirstName = firstName;
      GameNPC.LastName = lastName;
    }

    public void UpdateConversationDisplayName(string contactName)
    {
      if (MessagingManager.instance == null)
      {
        log.LogError("MessagingManager.instance is null!");
        return;
      }

      MSGConversation msgConversation = MessagingManager.instance.GetConversation(GameNPC);

      if (msgConversation == null)
      {
        log.LogWarning(
          $"Could not get MSGConversation for {GameNPC?.GUID}. Cannot update contactName."
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
