using System;
using BepInEx.Logging;

namespace TwitchCustomers.HarmonyPatches
{
  public interface IPatchModule
  {
    Type StaticWrapperType { get; }

    void Setup(Plugin plugin);
  }

  public abstract class PatchModule<TLogic> : IPatchModule
    where TLogic : class
  {
    public abstract Type StaticWrapperType { get; }

    protected TLogic LogicInstance { get; private set; }

    public abstract TLogic CreateLogicInstance(Plugin plugin);

    public abstract void InitializeStaticWrapper(ManualLogSource log);

    public virtual void Setup(Plugin plugin)
    {
      ManualLogSource log = plugin.Log;

      if (LogicInstance != null)
      {
        log.LogWarning(
          $"PatchModule for {StaticWrapperType.Name} already initialized. Skipping..."
        );
        return;
      }

      LogicInstance = CreateLogicInstance(plugin);

      if (LogicInstance == null)
      {
        throw new InvalidOperationException(
          $"CreateLogicInstance for {GetType().Name} did not set the LogicInstance property."
        );
      }

      InitializeStaticWrapper(plugin.Log);
    }
  }
}
