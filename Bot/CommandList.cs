using DiscordBot.Game;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Bot
{
    public class CommandList : ApplicationCommandModule
    {
        private Player _firstPlayer = null!;
        private Player _secondPlayer = null!;
        private long _gameCounter = 0;

        private static bool PlayerInData(Player player)
        {
            foreach(var playersPair in MainProgram.PlayersGames)
            {
                if (playersPair.Value.Id == player.User.Id) return true;
                else if (playersPair.Key.Id == player.User.Id) return true;
            }
            return false;
        }
        private void DisposePlayers()
        {
            _firstPlayer = null!;
            _secondPlayer = null!;
        }

        [SlashCommand("DoWords", "Вызвать на дуэль человека")]
        public async Task DoWords(InteractionContext c, [Option("Противник", "Кого вы хотите вызвать на дуэль?")] DiscordUser user)
        {
            await c.DeferAsync();
            _firstPlayer = new(c.User);
            _secondPlayer = new(user);
            if (PlayerInData(_secondPlayer))
            {
                await c.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Этот игрок уже находится в одной из игровых сессий, или ожидает ответа."));
                DisposePlayers();
                return;
            }
            MainProgram.PlayersGames.Add(_firstPlayer, _secondPlayer);
            await c.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed: Embeds.DuelEmbed(_firstPlayer, _secondPlayer)));
        }
        [SlashCommand("RefuseWords", "Отказаться от вызова (отказаться могут обе стороны)")]
        public async Task RefuseWords(InteractionContext c)
        {
            foreach(var players in MainProgram.PlayersGames)
            {
                if (players.Value.Id == c.User.Id || players.Key.Id == c.User.Id)
                {
                    MainProgram.PlayersGames.Remove(players.Key);
                    await c.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed: Embeds.DuelStop(c.User)));
                    return;
                }
            }
            await c.EditResponseAsync(new DiscordWebhookBuilder().WithContent("У вас нету на данных момент никаких вызовов."));
        }
        [SlashCommand("AcceptWords", "Принать вызов человека")]
        public async Task AcceptDuel(InteractionContext c)
        {
            await c.DeferAsync();
            foreach (var playersPair in MainProgram.PlayersGames) 
            {
                if (playersPair.Value.User.Id == c.User.Id)
                {
                    GameManager newGame = new (playersPair.Key, playersPair.Value, c.Channel, _gameCounter, MainProgram.Client);
                    MainProgram.Games.Add(_gameCounter++, newGame);
                    await c.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed: Embeds.AcceptDuelEmbed(playersPair)));
                    MainProgram.PlayersGames.Remove(playersPair.Key);
                    return;
                }
            }
            await c.EditResponseAsync(new DiscordWebhookBuilder().WithContent("У вас нету действительных вызовов."));
        }
        [SlashCommand("Games", "Список текущих игр")] 
        public async Task Games(InteractionContext c)
        {
            await c.DeferAsync();
            
            await c.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed: Embeds.GamesInfoEmbed(c.Guild.Id)));
        }

        [SlashCommand("GameStop", "Закончить игровую сессию по причине, при её наличии.")]
        public async Task GameStop(InteractionContext c, [Option("GameID", "ID игровой сессии, её вы можете найти по команду /games")] long id, [Option("Причина", "Эта причина будет выведена ботом при окончании игры.")] string stopMessage)
        {
            await c.DeferAsync();
            foreach (GameManager i in MainProgram.Games.Values)
            {
                if(i.UID == id && i.PlayChannel.GuildId == c.Guild.Id && (i.FirstPlayer.User.Id == c.User.Id || i.SecondPlayer.User.Id == c.User.Id || Player.IsAdmin(_firstPlayer.User.Id) || Player.IsAdmin(_secondPlayer.User.Id)))
                {
                    i.StopGame(c, stopMessage);
                    return;
                }
            }
            await c.EditResponseAsync(new DiscordWebhookBuilder().WithContent("У вас нету действительных игр."));
        }
        [SlashCommand("GameInfo", "Информация о вашей нынешней, игровой сессии, при её наличии.")]
        public async Task GameInfo(InteractionContext c, [Option("GameID", "ID игровой сессии, её вы можете найти по команду /games")] long id)
        {
            await c.DeferAsync();
            foreach (GameManager i in MainProgram.Games.Values)
            {
                if (i.UID == id)
                {
                    await c.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed: Embeds.GameInfoEmbed(i)));
                }
            }
        }
    }
}
