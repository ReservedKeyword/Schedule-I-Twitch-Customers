using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace TwitchCustomers.TwitchIntegration
{
  public class ChatterManager(
    Plugin plugin,
    string channelName,
    List<string> blocklistedChatters,
    int queueSize
  )
  {
    private readonly ManualLogSource log = plugin.Log;
    private readonly string messageCommand = plugin.PluginConfig.MessageCommand.Value;
    private readonly string channelName = channelName;
    private readonly List<string> blocklistedChatters = blocklistedChatters;
    private readonly int queueSize = queueSize;

    private TwitchClient client;
    private readonly object chattersLock = new();
    private readonly HashSet<string> chatters = [];

    public void Connect()
    {
      ConnectionCredentials credentials = new("justinfan1234567", "");
      client = new TwitchClient();

      client.Initialize(credentials, channelName);
      client.OnConnected += Client_OnConnected;
      client.OnConnectionError += Client_OnConnectionError;
      client.OnJoinedChannel += Client_OnJoinedChannel;
      client.OnMessageReceived += Client_OnMessageReceived;
      client.Connect();

      log.LogInfo("Attempting to connect to Twitch IRC client...");
    }

    private void Client_OnConnected(object sender, OnConnectedArgs e)
    {
      log.LogInfo("Connected to Twitch IRC client.");
      client.JoinChannel(channelName);
      log.LogInfo($"Attempting to join channel {channelName} as anonymous user...");
    }

    private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
    {
      log.LogError("Failed to connect to Twitch IRC client!");
      log.LogError(e.Error.Message);
    }

    private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
      log.LogInfo($"Joined {channelName}'s Twitch channel as anonymous user.");
    }

    private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
      string displayName = e.ChatMessage.DisplayName;

      if (blocklistedChatters.Contains(displayName, StringComparer.CurrentCultureIgnoreCase))
        return;

      if (!e.ChatMessage.Message.Contains(messageCommand))
        return;

      lock (chattersLock)
      {
        if (chatters.Count >= queueSize)
          return;

        if (chatters.Add(displayName))
          log.LogInfo($"Added Twitch chatter {displayName} to Set.");
      }
    }

    public string GetRandomChatter()
    {
      lock (chattersLock)
      {
        if (chatters.Count <= 0)
        {
          log.LogWarning("No chatters found, nothing to return.");
          return null;
        }

        Random random = new();
        int randIndex = random.Next(chatters.Count);
        string randElement = chatters.ElementAt(randIndex);
        chatters.Remove(randElement);
        return randElement;
      }
    }
  }
}
