using cleaner;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using config;
using WelcomeBot;
using System.Reflection;
using Discord.Interactions;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DiscordWelcomeBot
{
    class main
    {
        //public vars definition
        DiscordSocketClient client;
        List<ulong> greetingsMsg;
        ConcurrentPriorityQueue<Message<ulong, DateTime>> msgdate;
        List<KeyValuePair<ulong, ulong>> pingCount; 
        SocketTextChannel channel;
        //start Main method
        static void Main(string[] args)
            => new main().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            msgdate = new ConcurrentPriorityQueue<Message<ulong, DateTime>>();
            greetingsMsg = new List<ulong>();

            pingCount = new List<KeyValuePair<ulong, ulong>>();

            //give permission for bot
            var socketConfig = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers | GatewayIntents.GuildBans | GatewayIntents.GuildMessages | GatewayIntents.MessageContent,
                MessageCacheSize = 100
            };

            client = new DiscordSocketClient(socketConfig);
            //Handlers
            client.MessageReceived += MessageReceivedHandler;
            client.UserJoined += JoinedUserHandler;
            client.ButtonExecuted += WelcomeButtonHandler;
            client.MessageDeleted += MessageDeleteHandler;
            
            channel = client.GetChannel(Config.Get().channelId) as SocketTextChannel;

            client.Log += Log;
            //start bot
            await client.LoginAsync(TokenType.Bot, Config.Get().botToken);
            await client.StartAsync();
            //another thread for timer for deleting expired welcome messages
            Thread timerThread = new Thread(() => MessageCleaner.cleanerThread(msgdate, client));
            timerThread.Start();
            Console.ReadLine();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public async Task WelcomeButtonHandler(SocketMessageComponent component)
        {
            var triggeredUser = component.User;
            var channel = client.GetChannel(Config.Get().channelId) as SocketTextChannel;
            var mentionedUser = channel.GetUser(ulong.Parse(component.Data.CustomId)) as SocketGuildUser;

            if (mentionedUser == null)
            {
                await component.DeferAsync();
                return;
            }

            var userPair = KeyValuePair.Create(triggeredUser.Id, mentionedUser.Id);

            if (!pingCount.Contains(userPair) && triggeredUser.Mention != mentionedUser.Mention)
            {
                pingCount.Add(userPair);

                Random random = new Random();
                int randgif = random.Next(0, Config.Get().gifs.Count);

                FileAttachment fileAttachment = new FileAttachment(Config.Get().gifs[randgif]);
                await component.RespondWithFileAsync(fileAttachment, mentionedUser.Mention + ", Тебе привет от " + triggeredUser.Mention);
            }
            else 
                await component.DeferAsync();
        }

        private async Task JoinedUserHandler(SocketGuildUser user)
        {
            Random random = new Random();
            int randgreet = random.Next(0, Config.Get().greetings.Count);

            var builder = new ComponentBuilder().WithButton("Привет!", user.Id.ToString());

            var channel = client.GetChannel(Config.Get().channelId) as SocketTextChannel;
            var lastMsgId = channel.SendMessageAsync(Config.Get().greetings[randgreet] + user.Mention, components: builder.Build()).Result.Id;

            var time = DateTime.Now;
            
            var pair = new Message<ulong, DateTime>(lastMsgId, time.AddSeconds(Config.Get().withButtonDeleteAfter), WelcomeBot.Type.BUTTON);

            msgdate.Enqueue(pair);

            greetingsMsg.Add(lastMsgId);
            
            
        }



        private async Task MessageReceivedHandler(SocketMessage message)
        {
            if (message.Channel.Id == Config.Get().channelId)
            {
                var channel = client.GetChannel(Config.Get().channelId) as SocketTextChannel;
                if (message.Author.Id == Config.Get().botId && !greetingsMsg.Contains(message.Id))
                {
                    var time = DateTime.Now;
                    var pair = new Message<ulong, DateTime>(message.Id, time.AddSeconds(Config.Get().deleteAfter));

                    msgdate.Enqueue(pair);
                }
            }

            if (message.Channel.Id == Config.Get().engChannelID)
            {
                var messageString = message.CleanContent.ToString();

                if (Regex.IsMatch(messageString, @"\p{IsCyrillic}"))
                {
                    await message.Channel.DeleteMessageAsync(message.Id);
                }
            }
        }

        private async Task MessageDeleteHandler(Cacheable<IMessage, ulong> msg, Cacheable<IMessageChannel, ulong> channel)
        { 
            if (msg.Value.Author.Id == Config.Get().botId)
            {
                var mentionedUsers = msg.Value.MentionedUserIds;
                if (mentionedUsers.Count == 1)
                {
                    pingCount.RemoveAll((pair) => pair.Value == mentionedUsers.Last());
                }
            }
        }
    }
}
