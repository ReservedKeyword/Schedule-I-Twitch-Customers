using System;
using MelonLoader;

namespace TwitchCustomers.HarmonyPatches
{
  public interface IPatchModule
  {
    Type StaticWrapperType { get; }

    void Setup(Mod mod);
  }

  public abstract class PatchModule<TLogic> : IPatchModule
    where TLogic : class
  {
    public abstract Type StaticWrapperType { get; }

    protected TLogic LogicInstance { get; private set; }

    public abstract TLogic CreateLogicInstance(Mod mod);

    public abstract void InitializeStaticWrapper(MelonLogger.Instance log);

    public virtual void Setup(Mod mod)
    {
      MelonLogger.Instance log = mod.LoggerInstance;

      if (LogicInstance != null)
      {
        log.Warning($"PatchModule for {StaticWrapperType.Name} already initialized. Skipping...");
        return;
      }

      LogicInstance = CreateLogicInstance(mod);

      if (LogicInstance == null)
      {
        throw new InvalidOperationException(
          $"CreateLogicInstance for {GetType().Name} did not set the LogicInstance property."
        );
      }

      InitializeStaticWrapper(mod.LoggerInstance);
    }
  }
}
