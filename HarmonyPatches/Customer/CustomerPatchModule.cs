using System;
using MelonLoader;

namespace TwitchCustomers.HarmonyPatches.Customer
{
  public class CustomerPatchModule : PatchModule<CustomerPatchLogic>
  {
    public override Type StaticWrapperType => typeof(CustomerPatchWrapper);

    public override CustomerPatchLogic CreateLogicInstance(Mod mod)
    {
      mod.LoggerInstance.Msg("CustomerPatchModule created & bound CustomerPatchLogic instance.");
      return new CustomerPatchLogic(mod);
    }

    public override void InitializeStaticWrapper(MelonLogger.Instance log)
    {
      if (LogicInstance is CustomerPatchLogic logic)
      {
        CustomerPatchWrapper.Initialize(logic);
        return;
      }

      log.Error(
        $"LogicInstance is not type CustomerPatchLogic in CustomerPatchModule. Type is ${LogicInstance?.GetType().Name ?? "unknown"}."
      );

      throw new InvalidOperationException(
        $"LogicInstance is not type CustomerPatchLogic for {GetType().Name}."
      );
    }
  }
}
