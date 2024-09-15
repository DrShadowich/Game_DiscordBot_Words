using DiscordBot.DataPart;
using DiscordBot.DataPart.Entities;
using DiscordBot.Game.PlayerItems;
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
            if (MainProgram.PlayersGames.IsPairExsist(player)) return true;
            return false;
        }

        private async Task InitPlayers(DiscordUser firstPlayer, DiscordUser secondPlayer)
        {
            _firstPlayer = new(firstPlayer);
            _secondPlayer = new(secondPlayer);
            if (_firstPlayer != null && _secondPlayer != null)
            {
                if (_firstPlayer != null && await MainProgram.Context.Players.FindAsync(_firstPlayer.PlayerId) == null)
                {
                    MainProgram.Context.PlayersStats.Add(new Stats() { PlayerId = _firstPlayer.User.Id });
                    MainProgram.Context.Players.Add(_firstPlayer);
                    Console.WriteLine($"> Добавили нового пользователя: {_firstPlayer.User?.Username}");
                }
                if (_firstPlayer?.User?.Id != _secondPlayer.User.Id)
                {
                    if (await MainProgram.Context.Players.FindAsync(_secondPlayer.PlayerId) == null)
                    {
                        MainProgram.Context.PlayersStats.Add(new Stats() { PlayerId = _secondPlayer.User.Id });
                        MainProgram.Context.Players.Add(_secondPlayer);
                        Console.WriteLine($"> Добавили нового пользователя: {_secondPlayer.User?.Username}");
                    }
                }
            }
            await MainProgram.Context.SaveChangesAsync();
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
        public static async Task WordStats(InteractionContext c)
        {
            await c.DeferAsync();
            await c.EditResponseAsync(Embeds.WithWebHook(Embeds.ShowStat(c.User)));
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
            MainProgram.PlayersGames.AddPairPlayers(_firstPlayer, _secondPlayer);
            await c.EditResponseAsync(Embeds.WithWebHook(embed: Embeds.DuelEmbed(_firstPlayer, _secondPlayer)));
        }


        [SlashCommand("RefuseWords", "Отказаться от вызова (отказаться могут обе стороны)")]
        public static async Task RefuseWords(InteractionContext c)
        {
            await c.DeferAsync();
            if (MainProgram.PlayersGames.FindPlayers(c.User.Id, out KeyValuePair<PlayerEntity, PlayerEntity> playersPair))
            {
                MainProgram.PlayersGames.RemovePair(playersPair.Key);
                await c.EditResponseAsync(Embeds.WithWebHook(embed: Embeds.DuelStop(c.User)));
                return;
            }
            await c.EditResponseAsync(Embeds.WithWebHook("У вас нету на данных момент никаких вызовов."));
        }


        [SlashCommand("AcceptWords", "Принять вызов человека")]
        public async Task AcceptDuel(InteractionContext c)
        {
            await c.DeferAsync();
            if (MainProgram.PlayersGames.FindPlayers(c.User.Id, out KeyValuePair<PlayerEntity, PlayerEntity> playersPair))
            {
                GameManager newGame = new(playersPair.Key, playersPair.Value, c.Channel, _gameCounter, MainProgram.Client);
                MainProgram.Games.AddGame(_gameCounter++, newGame);
                MainProgram.PlayersGames.RemovePair(playersPair.Key);
                await c.EditResponseAsync(Embeds.WithWebHook(embed: Embeds.AcceptDuelEmbed(playersPair)));
                return;
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
            if (MainProgram.Games.FindGame(id, out GameManager game) && game.PlayChannel.GuildId == c.Guild.Id && (game.IsPlayerExsist(c.User.Id)
                || game.IsGameRuledByAdmin()))
            {
                await c.EditResponseAsync(Embeds.WithWebHook(game.StopGame(stopMessage)));
                return;
            }
            await c.EditResponseAsync(Embeds.WithWebHook("У вас нету действительных игр."));
        }


        [SlashCommand("GameInfo", "Информация о вашей нынешней, игровой сессии, при её наличии.")]
        public static async Task GameInfo(InteractionContext c, [Option("GameID", "ID игровой сессии, её вы можете найти по команду /games")] long id)
        {
            await c.DeferAsync();
            if (MainProgram.Games.FindGame(id, out GameManager game)) await c.EditResponseAsync(Embeds.WithWebHook(Embeds.GameInfoEmbed(game)));
        }
    }
}
