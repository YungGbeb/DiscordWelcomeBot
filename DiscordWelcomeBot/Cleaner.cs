using Discord.WebSocket;
using System;
using config;
using WelcomeBot;

namespace cleaner
{
    public class MessageCleaner
    {
        public static void cleanerThread(ConcurrentPriorityQueue<Pair<ulong, DateTime>> pairs, DiscordSocketClient mainclient)
        {
            while (true)
            {
                if (pairs.Count() > 0)
                {
                    var mainChannel = mainclient.GetChannel(Config.Get().channelId) as SocketTextChannel;
                    var firstPair = pairs.First();
                    DateTime firstDate = firstPair.Value;
                    if (firstDate < DateTime.Now)
                    {
                        mainChannel.DeleteMessageAsync(firstPair.Key);
                        pairs.Dequeue();
                    }
                }

            }
        }
    }
}