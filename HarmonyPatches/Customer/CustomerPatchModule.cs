using System;
using BepInEx.Logging;

namespace TwitchCustomers.HarmonyPatches.Customer
{
  public class CustomerPatchModule : PatchModule<CustomerPatchLogic>
  {
    public override Type StaticWrapperType => typeof(CustomerPatchWrapper);

    public override CustomerPatchLogic CreateLogicInstance(Plugin plugin)
    {
      plugin.Log.LogInfo("CustomerPatchModule created & bound CustomerPatchLogic instance.");
      return new CustomerPatchLogic(plugin);
    }

    public override void InitializeStaticWrapper(ManualLogSource log)
    {
      if (LogicInstance is CustomerPatchLogic logic)
      {
        CustomerPatchWrapper.Initialize(logic);
        return;
      }

      log.LogError(
        $"LogicInstance is not type CustomerPatchLogic in CustomerPatchModule. Type is ${LogicInstance?.GetType().Name ?? "unknown"}."
      );

      throw new InvalidOperationException(
        $"LogicInstance is not type CustomerPatchLogic for {GetType().Name}."
      );
    }
  }
}
