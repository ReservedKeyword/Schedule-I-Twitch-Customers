using System.Collections.Generic;
using BepInEx.Configuration;

namespace TwitchCustomers
{
  public class PluginConfig
  {
    public bool IsEnabled;
    public List<string> BlocklistedChatters;
    public string ChannelName;
    public string MessageCommand;
    public bool PreserveOriginalNPCName;
    public int QueueSize;

    public PluginConfig(Plugin plugin)
    {
      ConfigFile configFile = plugin.Config;

      IsEnabled = configFile.Bind(
        "General",
        "Enable Plugin",
        true,
        "Set to true to enable the plugin."
      ).Value;

      PreserveOriginalNPCName = configFile.Bind(
        "General",
        "Preserve Original NPC Name",
        true,
        "Set to true to reset NPCs back to their original names once a deal has finished."
      ).Value;

      ChannelName = configFile.Bind(
        "Twitch Integration",
        "Channel Name",
        "reservedkeyword",
        "Set the Twitch channel to listen to messages in."
      ).Value;

      string blocklistedChatters = configFile.Bind(
        "Twitch Integration",
        "Blocklisted Chatters",
        "",
        "Comma-separated list of chatter usernames to not process messages of."
      ).Value;

      BlocklistedChatters = [.. blocklistedChatters.Split(",")];

      MessageCommand = configFile.Bind(
        "Twitch Integration",
        "Message Command",
        "!iwannabeacustomer",
        "Unique command that registers a Twitch chatter as someone who wishes to be in game."
      ).Value;

      QueueSize = configFile.Bind(
        "Twitch Integration",
        "Queue Size",
        100,
        "Limit of **unique** chatters to keep in queue. Won't allow above limit."
      ).Value;
    }
  }
}
