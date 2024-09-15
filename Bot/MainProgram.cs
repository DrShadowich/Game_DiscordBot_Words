using DiscordBot.Config;
using DiscordBot.DataPart;
using DiscordBot.DataPart.Entities;
using DiscordBot.Game.GameItems;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Bot
{
    internal class MainProgram
    {
        public static object Locker = new();
        public static DataContext Context = new();
        public static DiscordClient Client = null!;
        public static PlayedPlayers PlayersGames = new();
        public static GamesContainer Games = new();
        static async Task Main()
        {
            ConfigReader jsonReader = new();
            await jsonReader.ReadJSONAsync();

            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            Client = new DiscordClient(discordConfig);
            Client.Ready += ClientReady;
            Client.MessageCreated += OnSend;

            SlashCommandsExtension slashService = Client.UseSlashCommands();
            slashService.RegisterCommands<CommandList>();

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private static Task ClientReady(DiscordClient s, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }


        private static async Task OnSend(DiscordClient s, MessageCreateEventArgs e)
        {
            if (e.Author.IsBot) return;
            Random random = new();
            int randint = random.Next() % 100;
            if (random.Next() % 100 == 5) await e.Message.RespondAsync("Факт");
            if (random.Next() % 100 == 25) await e.Message.RespondAsync("Прочитал, удаляй.");
            else Console.WriteLine($"> Монетка была брошена и выбила: {randint}");
        }
    }
}