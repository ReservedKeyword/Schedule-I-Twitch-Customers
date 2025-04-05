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
    string messageCommand,
    double subscriberWeight,
    int queueSize
  )
  {
    private enum TargetGroup
    {
      Subscriber,
      NonSubscriber,
      None,
    }

    private const double NON_SUBSCRIBER_WEIGHT = 1.0;

    private readonly string channelName = channelName;
    private readonly List<string> blocklistedChatters = blocklistedChatters;
    private readonly string messageCommand = messageCommand;
    private readonly double subscriberWeight = subscriberWeight;
    private readonly int queueSize = queueSize;

    private TwitchClient client;
    private readonly ManualLogSource log = plugin.Log;

    private readonly object chattersLock = new();
    private readonly HashSet<string> subscriberParticipants = [];
    private readonly HashSet<string> nonSubscriberParticipants = [];
    private readonly Random random = new();

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
      bool isSubscriber = e.ChatMessage.IsSubscriber;

      if (blocklistedChatters.Contains(displayName, StringComparer.CurrentCultureIgnoreCase))
      {
        log.LogInfo($"Detected blocklisted chatter {displayName}, will not add to queue.");
        return;
      }

      if (!e.ChatMessage.Message.Contains(messageCommand))
        return;

      lock (chattersLock)
      {
        if (GetTotalParticipants() >= queueSize)
          return;

        if (isSubscriber)
        {
          subscriberParticipants.Add(displayName);
          log.LogInfo($"Twitch chatter {displayName} (subscriber) added as participant.");
        }
        else
        {
          nonSubscriberParticipants.Add(displayName);
          log.LogInfo($"Twitch chatter {displayName} (non-subscriber) added as participant.");
        }
      }
    }

    private (double totalWeight, double subscriberWeightTotal) CalculateWeights()
    {
      double subWeight = subscriberParticipants.Count * subscriberWeight;
      double nonSubWeight = nonSubscriberParticipants.Count * NON_SUBSCRIBER_WEIGHT;
      return (subWeight + nonSubWeight, subWeight);
    }

    private TargetGroup DetermineTargetGroup(double randomPick, double subscriberWeightTotal)
    {
      bool tryPickingSubscriber = randomPick < subscriberWeightTotal;
      int subscriberCount = subscriberParticipants.Count;
      int nonSubscriberCount = nonSubscriberParticipants.Count;

      // Pick subscriber if roll is in range & subscriber exists
      if (tryPickingSubscriber && subscriberCount > 0)
      {
        return TargetGroup.Subscriber;
      }

      // Pick a non-subscriber if roll is outside range (or a subscriber didn't exist) AND non-subscriber exists
      if (nonSubscriberCount > 0)
      {
        return TargetGroup.NonSubscriber;
      }

      // If non-subscribers didn't exist either, but subscribers did, pick a subscriber anyway.
      if (subscriberCount > 0)
      {
        return TargetGroup.Subscriber;
      }
      return TargetGroup.None;
    }

    public string GetRandomChatter()
    {
      lock (chattersLock)
      {
        if (GetTotalParticipants() == 0)
        {
          log.LogWarning("No chatters found, nothing to return.");
          return null;
        }

        (double totalWeight, double subscriberWeightTotal) = CalculateWeights();

        if (totalWeight <= 0)
        {
          log.LogError("Total weight is zero or negative. Cannot perform weighted pick.");
          return null;
        }

        double randomPick = random.NextDouble() * totalWeight;
        TargetGroup target = DetermineTargetGroup(randomPick, subscriberWeightTotal);

        string winner = null;
        bool winnerSelected = false;

        switch (target)
        {
          case TargetGroup.Subscriber:
            int subIndex = random.Next(subscriberParticipants.Count);
            winner = subscriberParticipants.ElementAt(subIndex);
            subscriberParticipants.Remove(winner);
            log.LogInfo($"Selected winner (subscriber): {winner}");
            winnerSelected = true;
            break;

          case TargetGroup.NonSubscriber:
            int nonSubIndex = random.Next(nonSubscriberParticipants.Count);
            winner = nonSubscriberParticipants.ElementAt(nonSubIndex);
            nonSubscriberParticipants.Remove(winner);
            log.LogInfo($"Selected winner (non-subscriber): {winner}");
            winnerSelected = true;
            break;

          case TargetGroup.None:
          default:
            log.LogWarning("No target group determined for selection.");
            winnerSelected = false;
            break;
        }

        if (!winnerSelected && (GetTotalParticipants() > 0))
        {
          log.LogError(
            $"Failed to select winner event thought participants exist (Target Group: {target})."
          );
          return null;
        }

        log.LogInfo($"Returning winner: {winner}. Remaining participants: {GetTotalParticipants()}.");
        return winner;
      }
    }

    private int GetTotalParticipants() =>
      subscriberParticipants.Count + nonSubscriberParticipants.Count;
  }
}
