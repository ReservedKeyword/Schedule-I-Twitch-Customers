namespace TwitchCustomers
{
  public class Constants
  {
    public const string ENTRY_TRANSFORM_FIND_NAME = "Name";
    public const string ENTRY_TRANSFORM_FIND_CATEGORY = "Category";
    public const float ENTRY_TRANSFORM_CATEGORY_MARGIN_RIGHT = 225.0f;

    public static string ToHarmonyID() =>
      $"{MyPluginInfo.PLUGIN_GUID}.{MyPluginInfo.PLUGIN_NAME}.{MyPluginInfo.PLUGIN_VERSION.Replace(".", "-")}";
  }
}
