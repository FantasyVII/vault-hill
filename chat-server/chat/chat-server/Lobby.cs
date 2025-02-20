﻿using System.Collections.Generic;
using ChatLib;

namespace ChatServer
{
    class Lobby
    {
        List<Player> players = new List<Player>();

        public int PlayersCount
        {
            get
            {
                return players.Count;
            }
        }

        public Player AddPlayer(Player player)
        {
            players.Add(player);
            return players[players.Count - 1];
        }

        public void RemovePlayer(Player player)
        {
            players.Remove(player);
        }

        public Player GetPlayer(string playerId)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].ID == playerId)
                    return players[i];
            }

            return null;
        }

        public Player GetPlayer(int index)
        {
            return players[index];
        }
    }
}