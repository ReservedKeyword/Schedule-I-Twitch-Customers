using System.Collections.Generic;
using BepInEx.Configuration;

namespace TwitchCustomers
{
  public class PluginConfig
  {
    public ConfigEntry<bool> IsEnabled;
    public List<string> BlocklistedChatters;
    public ConfigEntry<string> ChannelName;
    public ConfigEntry<string> MessageCommand;
    public ConfigEntry<double> SubscriberWeight;
    public ConfigEntry<bool> PreserveOriginalNPCName;
    public ConfigEntry<int> QueueSize;

    public PluginConfig(Plugin plugin)
    {
      ConfigFile configFile = plugin.Config;

      IsEnabled = configFile.Bind(
        "General",
        "Enable Plugin",
        true,
        "Set to true to enable the plugin."
      );

      PreserveOriginalNPCName = configFile.Bind(
        "General",
        "Preserve Original NPC Name",
        true,
        "Set to true to reset NPCs back to their original names once a deal has finished."
      );

      ChannelName = configFile.Bind(
        "Twitch Integration",
        "Channel Name",
        "reservedkeyword",
        "Set the Twitch channel to listen to messages in."
      );

      ConfigEntry<string> blocklistedChatters = configFile.Bind(
        "Twitch Integration",
        "Blocklisted Chatters",
        "",
        "Comma-separated list of chatter usernames to not process messages of."
      );

      BlocklistedChatters = [.. blocklistedChatters.Value.Split(",")];

      MessageCommand = configFile.Bind(
        "Twitch Integration",
        "Message Command",
        "!iwannabeacustomer",
        "Unique command that registers a Twitch chatter as someone who wishes to be in game."
      );

      SubscriberWeight = configFile.Bind(
        "Twitch Integration",
        "Subscriber Weight",
        1.2d,
        "Weight making subscribers more likely to get picked (e.g., 1.2 means subscribers are 20% more likely)."
      );

      QueueSize = configFile.Bind(
        "Twitch Integration",
        "Queue Size",
        100,
        "Limit of **unique** chatters to keep in queue. Won't allow above limit."
      );
    }
  }
}
