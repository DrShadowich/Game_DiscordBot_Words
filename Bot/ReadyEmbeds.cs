using DiscordBot.DataPart;
using DiscordBot.DataPart.Entities;
using DiscordBot.Game;
using DiscordBot.Game.PlayerItems;
using DSharpPlus.Entities;

namespace DiscordBot.Bot
{
    public static class Embeds
    {
        private static DiscordEmbedBuilder _message = new() { Color = DiscordColor.Red };
        public static DiscordWebhookBuilder WithWebHook(string content) => new DiscordWebhookBuilder().WithContent(content);
        public static DiscordWebhookBuilder WithWebHook(DiscordEmbed embed) => new DiscordWebhookBuilder().AddEmbed(embed);
        public static DiscordWebhookBuilder WithWebHook() => new();
        public static DiscordEmbed DuelEmbed(PlayerEntity firstPlayer, PlayerEntity secondPlayer)
        {
            _message.Title = $"{PlayerEntity.AdminName(firstPlayer.User)}\nвызвал на дуэль в слова\n{(secondPlayer.Admin ? "Моего повелителя" : $"{secondPlayer.User?.Username}")}";
            _message.Description = $"{secondPlayer.User?.Mention}, вы можете принять игру, написав команду /acceptwords";
            return _message.Build();
        }
        public static DiscordEmbed AcceptDuelEmbed(KeyValuePair<PlayerEntity, PlayerEntity> players)
        {
            _message.Title = $"{(players.Value.Admin ? "Мой повелитель" : $"{players.Value.User?.Username}")} принял вызов против {(players.Key.Admin ? "Моего повелителя" : $"{players.Key.User?.Username}")}";
            _message.Description = $"Игра начинается с {players.Key.User?.Mention}." +
                    $"\nЧтобы начать играть просто напишите слово, а Я проверю или исправлю синтаксис вашего сообщения." +
                    $"\nЧтобы закончить игру досрочно, напишите команду /gamestop и причину окончания игры." +
                    $"\nИгра может сама закончиться, если вы не будете писать сообщение втечение 5 минут." +
                    $"\nЧтобы посмотреть информацию о нынешней сессии, просто напишите /gameinfo." +
                    $"\nЧтобы написать какое-то сообщение вне контекста сессии, то в начале сообщения напишите \'*\'." +
                    $"\nУдачной игры!";
            return _message.Build();
        }
        public static DiscordEmbed GamesInfoEmbed(ulong GuildId)
        {
            string description = string.Empty;
            foreach (var i in MainProgram.Games)
            {
                if (i.Value.PlayChannel.GuildId == GuildId)
                {
                    description += $"Игра {i.Value.FirstPlayer.User?.Mention} vs {i.Value.SecondPlayer.User?.Mention}. ID = {i.Key}.\n";
                }
            }
            _message.Title = "Текущие сесси";
            _message.Description = description;
            return _message.Build();
        }
        public static DiscordEmbed GameInfoEmbed(GameManager thisGame)
        {
            int totalMinutes = (DateTime.Now.Hour * 60 + DateTime.Now.Minute) - (thisGame.GameStarted.Hour * 60 + thisGame.GameStarted.Minute);
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;
            _message.Title = $"Информация об игре {thisGame.SecondPlayer.User?.Username} vs {thisGame.SecondPlayer.User?.Username}";
            _message.Description = $"Игра {thisGame.FirstPlayer.User?.Mention} против {thisGame.SecondPlayer.User?.Mention}." +
                $"\nИгровая сессия уже длится {hours} часов {minutes} минут" +
                $"\nВсего слов использовано за сессию: {thisGame.TotalWordsCount}" +
                $"\nНа данный момент ожидается буква: {thisGame.LastWord}" +
                $"\nПоследнее использованное слово: {thisGame.LastString}" +
                $"\nНа данный момент ожидается ход от {(thisGame.TurnOfFirst ? $"{thisGame.FirstPlayer.User?.Mention}" : $"{thisGame.SecondPlayer.User?.Mention}")}";
            return _message.Build();
        }
        public static DiscordEmbed GameStopEmbed(GameManager thisGame)
        {
            _message.Title = "Игра окончена!";
            _message.Description = $"Игра была завершена по причине: {thisGame.StopReason}";
            return _message.Build();
        }
        public static DiscordEmbed ErrorEmbed(DiscordUser sender)
        {
            _message.Title = "Ошибка";
            _message.Description = $"{PlayerEntity.AdminName(sender)}, это слово уже было использовано в данной сессии. Подберите что-то другое.";
            return _message.Build();
        }
        public static DiscordEmbed DuelStop(DiscordUser sender)
        {
            _message.Title = "Вызов отменен";
            _message.Description = $"{sender.Mention} отказался от вызова";
            return _message.Build();
        }
        public static DiscordEmbed Respect(DiscordUser sender, DiscordUser user)
        {
            _message.Title = $"{sender.Username} кинул респект {user.Username}'у";
            _message.Description = "\0";
            return _message.Build();
        }
        public static DiscordEmbed ShowStat(DiscordUser c, DataContext context)
        {
            _message.Title = $"Статистика {c.Username}";
            PlayerEntity? p = context.Players.Find(c.Id);
            foreach (var i in p?.Stats?.GetAllStats() is not null ? p.Stats.GetAllStats() : [])
            {
                _message.Description += $"{i}\n";
            }
            return _message.Build();
        }
    }
}