namespace TwitchCustomers
{
  public class Constants
  {
    public const string ENTRY_TRANSFORM_FIND_NAME = "Name";
    public const string ENTRY_TRANSFORM_FIND_CATEGORY = "Category";
    public const float ENTRY_TRANSFORM_CATEGORY_MARGIN_RIGHT = 225.0f;

    public const string PLUGIN_GUID = "me.reservedkeyword";
    public const string PLUGIN_NAME = "TwitchCustomers";
    public const string PLUGIN_VERSION = "0.0.1";

    public static string ToHarmonyID() =>
      $"{PLUGIN_GUID}.{PLUGIN_NAME}.{PLUGIN_VERSION.Replace(".", "-")}";
  }
}
