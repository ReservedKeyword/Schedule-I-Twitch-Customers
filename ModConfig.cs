using System.Collections.Generic;
using MelonLoader;

namespace TwitchCustomers
{
  public class ModConfig
  {
    // General Settings
    public bool IsEnabled { get; private set; }
    public bool PreserveOriginalNPCName { get; private set; }

    // Twitch Integration Settings
    public string ChannelName { get; private set; }
    public List<string> BlocklistedChatters { get; private set; }
    public string MessageCommand { get; private set; }
    public double SubscriberWeight { get; private set; }
    public int QueueSize { get; private set; }

    public ModConfig()
    {
      // General Category
      var generalCategory = MelonPreferences.CreateCategory(
        "TwitchCustomers_General",
        "Twitch Customers - General"
      );

      IsEnabled = generalCategory
        .CreateEntry(
          identifier: "Enable Mod",
          default_value: true,
          description: "If true, plugin will initialize when loaded."
        )
        .Value;

      PreserveOriginalNPCName = generalCategory
        .CreateEntry(
          identifier: "Preserve Original NPC Name",
          default_value: true,
          description: "If true, NPC names are reset once a deal has finished."
        )
        .Value;

      // Twitch Integration
      var twitchIntegrationCategory = MelonPreferences.CreateCategory(
        "TwitchCustomers_Integration",
        "Twitch Customers - Twitch Integration"
      );

      ChannelName = twitchIntegrationCategory
        .CreateEntry(
          identifier: "Channel Name",
          default_value: "reservedkeyword",
          description: "Twitch channel to listen to messages in (no authentication needed)."
        )
        .Value;

      BlocklistedChatters =
      [
        .. twitchIntegrationCategory
          .CreateEntry(
            identifier: "Blocklisted Chatters",
            default_value: "",
            description: "Comma-separated list of chatter usernames to not process messages from."
          )
          .Value.Split(","),
      ];

      MessageCommand = twitchIntegrationCategory
        .CreateEntry(
          identifier: "Message Command",
          default_value: "!iwannabeacustomer",
          description: "Unique command that registers a Twitch chatter's intent to be a non-playable, in-game character."
        )
        .Value;

      SubscriberWeight = twitchIntegrationCategory
        .CreateEntry(
          identifier: "Subscriber Weight",
          default_value: 1.2d,
          description: "Weight value that makes subscribers more likely to get picked. (For example, 1.2 means subscribers are 20% luckier.)"
        )
        .Value;

      QueueSize = twitchIntegrationCategory
        .CreateEntry(
          identifier: "Queue Size",
          default_value: 100,
          description: "Limit of **unique** chatters, subscribers and non-subscribers combined, to keep in queue. Mod will refuse above limit."
        )
        .Value;
    }
  }
}
