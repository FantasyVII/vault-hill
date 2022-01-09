using System;
using System.Collections.Generic;
using System.Net.Sockets;
using GameNetworkLib;
using GameNetworkLib.Packets;

namespace GameServer
{
    class NetworkJob
    {
        public BasePacket BasePacket { get; private set; }

        List<Player> playersDidNotRespondOrRespondedWithFailure;

        string lastTimePacketResent;
        int totalPacketResent;

        GameRoom gameRoom;
        public bool Abort { get; private set; }

        public NetworkJob(BasePacket basePacket, GameRoom gameRoom)
        {
            playersDidNotRespondOrRespondedWithFailure = new List<Player>();

            for (int i = 0; i < gameRoom.PlayersCount; i++)
            {
                Player player = gameRoom.GetPlayer(i);

                if (player.ID != basePacket.Player.ID)
                    playersDidNotRespondOrRespondedWithFailure.Add(player);
            }

            this.gameRoom = gameRoom;
            BasePacket = basePacket;
            lastTimePacketResent = basePacket.CreationTime;
            totalPacketResent = 0;
            Abort = false;
        }

        public void SubmitResponsePacket(BasePacket responsePacket)
        {
            for (int i = playersDidNotRespondOrRespondedWithFailure.Count - 1; i >= 0; i--)
            {
                if (playersDidNotRespondOrRespondedWithFailure[i].ID == responsePacket.Player.ID)
                {
                    if (responsePacket.NetworkResponse == PacketResponse.Success)
                    {
                        playersDidNotRespondOrRespondedWithFailure.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void Update(Socket socket)
        {
            DateTime otherPacketDateTime = Convert.ToDateTime(lastTimePacketResent);
            DateTime currentDateTime = DateTime.UtcNow;
            TimeSpan diff = currentDateTime.Subtract(otherPacketDateTime);

            if (diff.TotalSeconds >= 2)
            {
                for (int i = 0; i < playersDidNotRespondOrRespondedWithFailure.Count; i++)
                {
                    socket.SendTo(BasePacket.Serialize(), playersDidNotRespondOrRespondedWithFailure[i].ipEndpoint);
                    Console.WriteLine("Senttttt");
                }

                totalPacketResent++;
                lastTimePacketResent = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }

            if (totalPacketResent >= 5)
                Abort = true;
        }
    }
}