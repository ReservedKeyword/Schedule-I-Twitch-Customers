using System.Collections;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP.Utils;
using TwitchCustomers.NPC;
using UnityEngine;
using UnityEngine.UI;

namespace TwitchCustomers.Coroutines
{
  public class ConversationNameCoroutineRunner : MonoBehaviour
  {
    public static ConversationNameCoroutineRunner Instance { get; private set; }

    private static readonly Plugin plugin = Plugin.Instance;
    private static readonly ManualLogSource log = plugin.Log;

    public void Awake()
    {
      if (Instance == null)
        Instance = this;
      else
        Destroy(gameObject);
    }

    public void StartDelayedConversationNameUpdate(CachedNPC npcToUpdate, string newDisplayName)
    {
      if (!gameObject.activeInHierarchy)
      {
        log.LogWarning("CoroutineRunner GameObject not active. Cannot start coroutine.");
        return;
      }

      if (npcToUpdate == null || npcToUpdate.GameNPC == null)
      {
        log.LogWarning("Attempted to start delayed UI update for a null NPC via CoroutineRunner.");
        return;
      }

      MonoBehaviourExtensions.StartCoroutine(
        this,
        DelayedConversationNameUpdateCoroutine(npcToUpdate, newDisplayName)
      );
    }

    private IEnumerator DelayedConversationNameUpdateCoroutine(
      CachedNPC npcToUpdate,
      string newDisplayName
    )
    {
      yield return new WaitForEndOfFrame();

      if (npcToUpdate == null || npcToUpdate.GameNPC == null || npcToUpdate.UnityRectEntry == null)
      {
        log.LogWarning(
          $"NPC or its UI became invalid before delayed UI update for {newDisplayName}."
        );
        yield break;
      }

      try
      {
        log.LogInfo(
          $"CoroutineRunner attempting delayed UI update for NPC {npcToUpdate.GameNPC.GUID} to name {newDisplayName}..."
        );
        Text nameTextComponent = npcToUpdate.GetConversationTextComponent();

        if (nameTextComponent != null)
        {
          nameTextComponent.text = newDisplayName;
          npcToUpdate.UpdateConversationCategoryPadding();
          log.LogInfo(
            $"CoroutineRunner delayed UI update successful for NPC {npcToUpdate.GameNPC.GUID}."
          );
          yield break;
        }
        log.LogWarning(
          $"Name Text component was null during CoroutineRunner delayed UI update for NPC {npcToUpdate.GameNPC.GUID}."
        );
      }
      catch (System.Exception ex)
      {
        log.LogError(
          $"Exception during CoroutineRunner delayed UI update for NPC {npcToUpdate.GameNPC?.GUID}: {ex}"
        );
      }
    }
  }
}
