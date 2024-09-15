using DiscordBot.DataPart.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Game.GameItems
{
    public class PlayedPlayers
    {
        private Dictionary<PlayerEntity, PlayerEntity> _players = [];
        public Dictionary<PlayerEntity, PlayerEntity> PlayingPlayers { get { return _players; } }

        public void AddPairPlayers(PlayerEntity firstPlayer, PlayerEntity secondPlayer)
        {
            if(_players.TryAdd(firstPlayer, secondPlayer)) Console.WriteLine($"> PlayedPlayers: Новая пара игроков успешно добавилась в игровую сессию.");
        }
        public bool FindPlayers(PlayerEntity firstPlayer, out KeyValuePair<PlayerEntity, PlayerEntity> Players)
        {
            foreach (PlayerEntity i in _players.Keys) 
            {
                if (i == firstPlayer) 
                {
                    _players.TryGetValue(i, out PlayerEntity? foundSecondPlayer);
                    if (foundSecondPlayer == null) throw new Exception("> PlayedPlayers: Первый игрок существует, но второго игрока почему-то нету. 0_0");
                    Players = new KeyValuePair<PlayerEntity, PlayerEntity>(i, foundSecondPlayer);
                    return true;
                }
            }
            Players = new();
            return false;
        }
        public bool IsPairExsist(PlayerEntity firstPlayer)
        {
            foreach (PlayerEntity i in _players.Keys)
            {
                if (i == firstPlayer && _players.TryGetValue(i, out PlayerEntity? p)) return true;
            }
            return false;
        }
        public bool FindPlayers(ulong byUserId, out KeyValuePair<PlayerEntity, PlayerEntity> Players)
        {
            foreach (PlayerEntity i in _players.Keys)
            {
                if (i.User.Id == byUserId)
                {
                    FindPlayers(i, out KeyValuePair<PlayerEntity, PlayerEntity> foundPlayers);
                    Players = foundPlayers;
                    return true;
                }
            }
            Players = new();
            return false;
        }
        public void RemovePair(ulong byUserId)
        {
            if(FindPlayers(byUserId, out KeyValuePair<PlayerEntity, PlayerEntity> playersPair))
            {
                _players.Remove(playersPair.Key);
            }
        }
        public void RemovePair(PlayerEntity byFirstPlayer)
        {
            if (FindPlayers(byFirstPlayer, out KeyValuePair<PlayerEntity, PlayerEntity> playersPair))
            {
                _players.Remove(playersPair.Key);
            }
        }
    }
}
