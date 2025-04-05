using System.Collections.Generic;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using TwitchCustomers.Coroutines;
using TwitchCustomers.HarmonyPatches;
using TwitchCustomers.HarmonyPatches.Customer;
using TwitchCustomers.HarmonyPatches.LoadManager;
using TwitchCustomers.HarmonyPatches.MSGConversation;
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

      // Setup Twitch chatter integration listener
      ChatterManager = new(
        this,
        PluginConfig.ChannelName.Value,
        PluginConfig.BlocklistedChatters,
        PluginConfig.MessageCommand.Value,
        PluginConfig.SubscriberWeight.Value,
        PluginConfig.QueueSize.Value
      );

      ChatterManager.Connect();

      // Setup cached NPC management
      CachedNPCManager = new CachedNPCManager(Log);

      // Setup Harmony patches
      harmony = new(Constants.ToHarmonyID());

      List<IPatchModule> patchModules =
      [
        new CustomerPatchModule(),
        new LoadManagerPatchModule(),
        new MSGConversationPatchModule(),
      ];

      foreach (IPatchModule patchModule in patchModules)
      {
        patchModule.Setup(this);
      }

      harmony.PatchAll();
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
