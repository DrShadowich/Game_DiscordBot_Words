﻿using DiscordBot.Bot;
using DiscordBot.Config;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using System.Timers;

namespace DiscordBot.Game
{
    public sealed class GameManager : IDisposable
    {
        #region VARS
        private long _gameId;
        private readonly DiscordChannel _channel;
        private readonly Player _firstPlayer;
        private readonly Player _secondPlayer;
        private readonly List<string> _baseOfStrings = [];
        private readonly DateTime _now = DateTime.Now;
        private readonly DiscordClient _botClient;
        private static System.Timers.Timer _fiveMinuteTime = null!;
        private const float _maxMinuteWait = 1.5f;
        private string _lastString = string.Empty;
        private char _lastWord = '\0';
        private bool _firstTurn = true;
        private string _stopReason = string.Empty;
        
        public long UID { get { return _gameId; } }
        public string LastString { get { return _lastString; } }
        public int TotalWordsCount { get { return _baseOfStrings.Count; } }
        public char LastWord { get { return _lastWord; } }
        public bool TurnOfFirst { get { return _firstTurn; } }
        public DiscordClient Bot { get { return _botClient; } }
        public Player FirstPlayer { get { return _firstPlayer; } }
        public Player SecondPlayer { get { return _secondPlayer; } }
        public DiscordChannel PlayChannel { get { return _channel; } }
        public DateTime GameStarted { get { return _now; } }
        public string StopReason { get { return _stopReason; } }
        #endregion
        public GameManager(Player firstPlayer, Player secondPlayer, DiscordChannel channel, long ID, DiscordClient bot)
        {
            _gameId = ID;
            _channel = channel;
            _firstPlayer = firstPlayer;
            _secondPlayer = secondPlayer;
            _botClient = bot;
            _botClient.MessageCreated += OnMessageSend;
            _ = ResetTimer();
        }
        #region GAMEMETHODS
        private async Task OnMessageSend(DiscordClient s, MessageCreateEventArgs e)
        {
            if (e.Author.IsBot) return;
            if (e.Channel.Id == _channel.Id)
            {
                if (e.Message.Content[0] == '*' && (_firstPlayer.User.Id == e.Author.Id || _secondPlayer.User.Id == e.Author.Id))
                {
                    return;
                }
                else if (_firstTurn && _firstPlayer.User.Id == e.Author.Id)
                {
                    await CompareMessage(e, _firstPlayer);
                }
                else if (_firstTurn == false && _secondPlayer.User.Id == e.Author.Id)
                {
                    await CompareMessage(e, _secondPlayer);
                }
            }
        }

        public void StopGame(InteractionContext c, string reason)
        {
            _stopReason = reason;
            Dispose();
        }

        public void StopGame(string reason)
        {
            _stopReason = reason;            
            Dispose();
        }
        #endregion
        #region TECHMETHODS
        private void OnTimer(object? s, ElapsedEventArgs e) => StopGame($"Никто не писал слово втечении {_maxMinuteWait} минут");
        public async void Dispose() 
        {
            _fiveMinuteTime.Elapsed -= OnTimer;
            _botClient.MessageCreated -= OnMessageSend;
            Console.WriteLine($"> Игровая сессия {UID} {_firstPlayer.User.Username} vs {_secondPlayer.User.Username} закончилась.");
            MainProgram.Games.Remove(_gameId);
            await _channel.SendMessageAsync(embed: Embeds.GameStopEmbed(this));
            GC.SuppressFinalize(this);
        }

        private async Task ResetTimer()
        {
            if(_fiveMinuteTime != null)
            {
                _fiveMinuteTime.Stop();
                _fiveMinuteTime.Dispose();
            }
            _fiveMinuteTime = new(_maxMinuteWait * 60 * 1000) { AutoReset = true, Enabled = true };
            _fiveMinuteTime.Elapsed += OnTimer;

            await Task.Run(() => { });
            Console.WriteLine("> Создали новый таймер");
        }

        private void SwitchTurn(MessageCreateEventArgs e)
        {
            _lastString = e.Message.Content;
            _lastWord = DebugString.GetLastWord(DebugString.GetRightString(e.Message.Content));
            if (_firstTurn) _firstTurn = false;
            else _firstTurn = true;
            _baseOfStrings.Add(DebugString.GetRightString(e.Message.Content));
        }

        private async Task CompareMessage(MessageCreateEventArgs e, Player whichPlayer)
        {
            await ResetTimer();
            if (_baseOfStrings.Contains(DebugString.GetRightString(e.Message.Content)))
            {
                await _channel.SendMessageAsync(embed: Embeds.ErrorEmbed(e.Author));
            }
            else if (_lastWord == '\0' && Convert.ToBoolean(await WordChecker.IsWordAsync(e.Message.Content)))
            {
                SwitchTurn(e);
                TestWord(e);
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName(_botClient, ":white_check_mark:"));
            }
            else if (_lastWord == DebugString.GetFirstWord(DebugString.GetRightString(e.Message.Content)) && Convert.ToBoolean(await WordChecker.IsWordAsync(e.Message.Content)))
            {
                SwitchTurn(e);
                TestWord(e);
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName(_botClient, ":white_check_mark:"));
            }
            else
            {
                TestWord(e);
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName(_botClient, ":x:"));
            }
        }

        private void TestWord(MessageCreateEventArgs e)
        {
            Console.WriteLine($"> Слово: {e.Message.Content}");
            Console.WriteLine($"> Отдебаженное слово: {DebugString.GetRightString(e.Message.Content)}");
            Console.WriteLine($"> Первая буква слова: {DebugString.GetFirstWord(DebugString.GetRightString(e.Message.Content))}");
            Console.WriteLine($"> Последняя буква слова: {_lastWord}\n\n");
        }
        #endregion
    }
}