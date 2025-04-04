using System;
using BepInEx.Logging;

namespace TwitchCustomers.HarmonyPatches.LoadManager
{
  public class LoadManagerPatchModule : PatchModule<LoadManagerPatchLogic>
  {
    public override Type StaticWrapperType => typeof(LoadManagerPatchWrapper);

    public override LoadManagerPatchLogic CreateLogicInstance(Plugin plugin)
    {
      plugin.Log.LogInfo("LoadManagerPatchModule created & bound LoadManagerPatchLogic instance.");
      return new LoadManagerPatchLogic(plugin);
    }

    public override void InitializeStaticWrapper(ManualLogSource log)
    {
      if (LogicInstance is LoadManagerPatchLogic logic)
      {
        LoadManagerPatchWrapper.Initialize(logic);
        return;
      }

      log.LogError(
        $"LogicInstance is not type LoadManagerPatchLogic in LoadManagerPatchModule. Type is ${LogicInstance?.GetType().Name ?? "unknown"}."
      );

      throw new InvalidOperationException(
        $"LogicInstance is not type LoadManagerPatchLogic for {GetType().Name}."
      );
    }
  }
}
