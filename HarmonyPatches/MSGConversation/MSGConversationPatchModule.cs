using System;
using MelonLoader;

namespace TwitchCustomers.HarmonyPatches.MSGConversation
{
  public class MSGConversationPatchModule : PatchModule<MSGConversationPatchLogic>
  {
    public override Type StaticWrapperType => typeof(MSGConversationPatchWrapper);

    public override MSGConversationPatchLogic CreateLogicInstance(Mod mod)
    {
      mod.LoggerInstance.Msg(
        "LoadManagerPatchModule created & bound LoadManagerPatchLogic instance."
      );
      return new MSGConversationPatchLogic(mod);
    }

    public override void InitializeStaticWrapper(MelonLogger.Instance log)
    {
      if (LogicInstance is MSGConversationPatchLogic logic)
      {
        MSGConversationPatchWrapper.Initialize(logic);
        return;
      }

      log.Error(
        $"LogicInstance is not type MSGConversationPatchLogic in MSGConversationPatchModule. Type is ${LogicInstance?.GetType().Name ?? "unknown"}."
      );

      throw new InvalidOperationException(
        $"LogicInstance is not type MSGConversationPatchLogic for {GetType().Name}."
      );
    }
  }
}
