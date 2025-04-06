using System;
using MelonLoader;

namespace TwitchCustomers.HarmonyPatches.LoadManager
{
  public class LoadManagerPatchModule : PatchModule<LoadManagerPatchLogic>
  {
    public override Type StaticWrapperType => typeof(LoadManagerPatchWrapper);

    public override LoadManagerPatchLogic CreateLogicInstance(Mod mod)
    {
      mod.LoggerInstance.Msg(
        "LoadManagerPatchModule created & bound LoadManagerPatchLogic instance."
      );
      return new LoadManagerPatchLogic(mod);
    }

    public override void InitializeStaticWrapper(MelonLogger.Instance log)
    {
      if (LogicInstance is LoadManagerPatchLogic logic)
      {
        LoadManagerPatchWrapper.Initialize(logic);
        return;
      }

      log.Error(
        $"LogicInstance is not type LoadManagerPatchLogic in LoadManagerPatchModule. Type is ${LogicInstance?.GetType().Name ?? "unknown"}."
      );

      throw new InvalidOperationException(
        $"LogicInstance is not type LoadManagerPatchLogic for {GetType().Name}."
      );
    }
  }
}
