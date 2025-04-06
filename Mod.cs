using System.Collections.Generic;
using MelonLoader;
using TwitchCustomers.HarmonyPatches;
using TwitchCustomers.HarmonyPatches.Customer;
using TwitchCustomers.HarmonyPatches.LoadManager;
using TwitchCustomers.HarmonyPatches.MSGConversation;
using TwitchCustomers.NPC;
using TwitchCustomers.TwitchIntegration;

[assembly: HarmonyDontPatchAll]
[assembly: MelonInfo(typeof(TwitchCustomers.Mod), "TwitchCustomers", "0.3.1", "ReservedKeyword")]
[assembly: MelonGame("TVGS", "Schedule I")]
[assembly: MelonColor(1, 160, 32, 240)]

namespace TwitchCustomers
{
  public class Mod : MelonMod
  {
    public static Mod Instance { get; private set; }

    public ModConfig ModConfig { get; private set; }
    public ChatterManager ChatterManager { get; private set; }
    public CachedNPCManager CachedNPCManger { get; private set; }

    private HarmonyLib.Harmony harmony;

    public override void OnInitializeMelon()
    {
      Instance = this;
      ModConfig = new();

      if (!ModConfig.IsEnabled)
      {
        LoggerInstance.Msg("Plugin disabled in configuration file, won't proceed...");
        return;
      }

      // Setup Twitch chatter integration
      ChatterManager = new(this, ModConfig);
      ChatterManager.Connect();

      // Setup cached NPC management
      CachedNPCManger = new(LoggerInstance);

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
      LoggerInstance.Msg("Mod has finished initialization process!");
    }
  }
}
