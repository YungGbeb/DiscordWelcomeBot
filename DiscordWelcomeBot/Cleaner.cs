using Discord.WebSocket;
using System;
using config;
using WelcomeBot;

namespace cleaner
{
    public class MessageCleaner
    {
        public static void cleanerThread(ConcurrentPriorityQueue<Message<ulong, DateTime>> pairs, DiscordSocketClient mainclient)
        {
            while (true)
            {
                if (pairs.Count() > 0)
                {
                    var mainChannel = mainclient.GetChannel(Config.Get().channelId) as SocketTextChannel;
                    var firstPair = pairs.First();
                    DateTime firstDate = firstPair.value;
                    if (firstDate < DateTime.Now)
                    {
                        mainChannel.DeleteMessageAsync(firstPair.key);
                        pairs.Dequeue();
                    }
                }
            }
        }
    }
}