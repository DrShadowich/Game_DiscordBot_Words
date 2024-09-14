using DiscordBot.DataPart;
using DiscordBot.DataPart.Entities;
using DiscordBot.Game;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
namespace DiscordBot.Bot
{
    public class CommandList : ApplicationCommandModule
    {
        private PlayerEntity _firstPlayer = null!;
        private PlayerEntity _secondPlayer = null!;
        private long _gameCounter = 0;
        private static bool PlayerInData(PlayerEntity player)
        {
            foreach(var playersPair in MainProgram.PlayersGames)
            {
                if (playersPair.Value.User?.Id == player.User?.Id) return true;
                else if (playersPair.Key.User?.Id == player.User?.Id) return true;
            }
            return false;
        }
        private async Task InitPlayers(DiscordUser firstPlayer, DiscordUser secondPlayer)
        {
            _firstPlayer = new(firstPlayer);
            _secondPlayer = new(secondPlayer);
            if (await MainProgram.Context.Players.FindAsync(_firstPlayer.PlayerId) == null)
            {
                MainProgram.Context.Players.Add(_firstPlayer);
                Console.WriteLine($"Добавили нового пользователя: {_firstPlayer.User?.Username}");
            }
            if (_firstPlayer.User?.Id != _secondPlayer.User?.Id)
            {
                if (await MainProgram.Context.Players.FindAsync(_secondPlayer.PlayerId) == null)
                {
                    MainProgram.Context.Players.Add(_secondPlayer);
                    Console.WriteLine($"Добавили нового пользователя: {_secondPlayer.User?.Username}");
                }
            }
        }
        private void DisposePlayers()
        {
            _firstPlayer = null!;
            _secondPlayer = null!;
        }

        [SlashCommand("PayRespect", "Кинуть респект человеку")]
        public static async Task PayRespect(InteractionContext c, [Option("Лютый", "Кого похвалить?")] DiscordUser user)
        {
            await c.DeferAsync();
            await c.EditResponseAsync(Embeds.WithWebHook(Embeds.Respect(c.User, user)));
        }

        [SlashCommand("WordStats", "Ваша статистика по игре в слова")]
        public async Task WordStats(InteractionContext c)
        {
            await c.DeferAsync();
            await c.EditResponseAsync(Embeds.WithWebHook(Embeds.ShowStat(c.User, new DataContext())));
        }

        [SlashCommand("DoWords", "Вызвать на дуэль человека")]
        public async Task DoWords(InteractionContext c, [Option("Противник", "Кого вы хотите вызвать на дуэль?")] DiscordUser user)
        {
            await c.DeferAsync();
            await InitPlayers(c.User, user);
            if (PlayerInData(_secondPlayer))
            {
                await c.EditResponseAsync(Embeds.WithWebHook("Этот игрок уже находится в одной из игровых сессий, или ожидает ответа."));
                DisposePlayers();
                return;
            }
            MainProgram.PlayersGames.Add(_firstPlayer, _secondPlayer);
            await c.EditResponseAsync(Embeds.WithWebHook(embed: Embeds.DuelEmbed(_firstPlayer, _secondPlayer)));
        }
        [SlashCommand("RefuseWords", "Отказаться от вызова (отказаться могут обе стороны)")]
        public static async Task RefuseWords(InteractionContext c)
        {
            foreach(var players in MainProgram.PlayersGames)
            {
                if (players.Value.User?.Id == c.User.Id || players.Key.User?.Id == c.User.Id)
                {
                    MainProgram.PlayersGames.Remove(players.Key);
                    await c.EditResponseAsync(Embeds.WithWebHook(embed: Embeds.DuelStop(c.User)));
                    return;
                }
            }
            await c.EditResponseAsync(Embeds.WithWebHook("У вас нету на данных момент никаких вызовов."));
        }
        [SlashCommand("AcceptWords", "Принать вызов человека")]
        public async Task AcceptDuel(InteractionContext c)
        {
            await c.DeferAsync();
            foreach (var playersPair in MainProgram.PlayersGames) 
            {
                if (playersPair.Value.User?.Id == c.User.Id)
                {
                    GameManager newGame = new (new DataContext(), playersPair.Key, playersPair.Value, c.Channel, _gameCounter, MainProgram.Client);
                    MainProgram.Games.Add(_gameCounter++, newGame);
                    await c.EditResponseAsync(Embeds.WithWebHook(embed: Embeds.AcceptDuelEmbed(playersPair)));
                    MainProgram.PlayersGames.Remove(playersPair.Key);
                    return;
                }
            }
            await c.EditResponseAsync(Embeds.WithWebHook("У вас нету действительных вызовов."));
        }
        [SlashCommand("Games", "Список текущих игр")] 
        public static async Task Games(InteractionContext c)
        {
            await c.DeferAsync();
            
            await c.EditResponseAsync(Embeds.WithWebHook(Embeds.GamesInfoEmbed(c.Guild.Id)));
        }

        [SlashCommand("GameStop", "Закончить игровую сессию по причине, при её наличии.")]
        public async Task GameStop(InteractionContext c, [Option("GameID", "ID игровой сессии, её вы можете найти по команду /games")] long id, [Option("Причина", "Эта причина будет выведена ботом при окончании игры.")] string stopMessage)
        {
            await c.DeferAsync();
            foreach (GameManager i in MainProgram.Games.Values)
            {
                if(i.UID == id && i.PlayChannel.GuildId == c.Guild.Id && (i.FirstPlayer.User?.Id == c.User.Id || i.SecondPlayer.User?.Id == c.User.Id 
                    || PlayerEntity.IsAdmin(_firstPlayer.User is not null ? _firstPlayer.User.Id : 0) 
                    || PlayerEntity.IsAdmin(_secondPlayer.User is not null ? _secondPlayer.User.Id : 0)))
                {
                    await c.EditResponseAsync(Embeds.WithWebHook(i.StopGame(stopMessage))); 
                    return;
                }
            }
            await c.EditResponseAsync(Embeds.WithWebHook("У вас нету действительных игр."));
        }
        [SlashCommand("GameInfo", "Информация о вашей нынешней, игровой сессии, при её наличии.")]
        public static async Task GameInfo(InteractionContext c, [Option("GameID", "ID игровой сессии, её вы можете найти по команду /games")] long id)
        {
            await c.DeferAsync();
            foreach (GameManager i in MainProgram.Games.Values)
            {
                if (i.UID == id) await c.EditResponseAsync(Embeds.WithWebHook(Embeds.GameInfoEmbed(i)));
            }
        }
    }
}
