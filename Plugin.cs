using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using TwitchCustomers.Coroutines;
using TwitchCustomers.HarmonyPatches;
using TwitchCustomers.NPC;
using TwitchCustomers.TwitchIntegration;

namespace TwitchCustomers
{
  [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
  public class Plugin : BasePlugin
  {
    public static Plugin Instance { get; private set; }

    public PluginConfig PluginConfig;
    public ChatterManager ChatterManager;
    public CachedNPCManager CachedNPCManager;
    private Harmony harmony;

    public override void Load()
    {
      Instance = this;
      PluginConfig = new(this);

      if (!PluginConfig.IsEnabled.Value)
      {
        Log.LogInfo("Plugin disabled in configuration file, won't proceed...");
        return;
      }

      RegisterConversationNameCoroutineRunner();

      ChatterManager = new(
        this,
        PluginConfig.ChannelName.Value,
        PluginConfig.BlocklistedChatters,
        PluginConfig.QueueSize.Value
      );
      ChatterManager.Connect();

      CachedNPCManager = new CachedNPCManager(Log);

      harmony = new(Constants.ToHarmonyID());
      harmony.PatchAll(typeof(CreateUIPatch));
      harmony.PatchAll(typeof(ExitToMenuPatch));
      harmony.PatchAll(typeof(ExpireOfferPatch));
      harmony.PatchAll(typeof(NotifyPlayerOfContractPatch));
      harmony.PatchAll(typeof(ProcessHandoverPatch));
    }

    private void RegisterConversationNameCoroutineRunner()
    {
      ConversationNameCoroutineRunner coroutineRunner =
        AddComponent<ConversationNameCoroutineRunner>();

      if (coroutineRunner != null)
      {
        coroutineRunner.gameObject.name = $"{MyPluginInfo.PLUGIN_NAME}.CoroutineRunner";
        Log.LogInfo("CoroutineRunner component added successfully!");
        return;
      }
      Log.LogError("Failed to add CoroutineRunner component!");
    }
  }
}
