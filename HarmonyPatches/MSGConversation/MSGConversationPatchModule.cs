using System;
using BepInEx.Logging;

namespace TwitchCustomers.HarmonyPatches.MSGConversation
{
  public class MSGConversationPatchModule : PatchModule<MSGConversationPatchLogic>
  {
    public override Type StaticWrapperType => typeof(MSGConversationPatchWrapper);

    public override MSGConversationPatchLogic CreateLogicInstance(Plugin plugin)
    {
      plugin.Log.LogInfo("LoadManagerPatchModule created & bound LoadManagerPatchLogic instance.");
      return new MSGConversationPatchLogic(plugin);
    }

    public override void InitializeStaticWrapper(ManualLogSource log)
    {
      if (LogicInstance is MSGConversationPatchLogic logic)
      {
        MSGConversationPatchWrapper.Initialize(logic);
        return;
      }

      log.LogError(
        $"LogicInstance is not type MSGConversationPatchLogic in MSGConversationPatchModule. Type is ${LogicInstance?.GetType().Name ?? "unknown"}."
      );

      throw new InvalidOperationException(
        $"LogicInstance is not type MSGConversationPatchLogic for {GetType().Name}."
      );
    }
  }
}
