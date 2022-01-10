using System;
using System.Net;
using System.Net.Sockets;
using GameNetworkLib;
using GameNetworkLib.Packets;
using GameNetworkLib.Packets.Mutual;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 3000));
            socket.Blocking = false;

            Console.WriteLine("Game Server Listening!");

            GameRoom gameRoom = new GameRoom();
            JobManager jobManager = new JobManager(gameRoom);

            while (true)
            {
                try
                {
                    if (socket.Available > 0)
                    {
                        byte[] receivedbuffer = new byte[socket.Available];
                        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                        EndPoint senderRemote = sender;

                        socket.ReceiveFrom(receivedbuffer, ref senderRemote);
                        BasePacket bp = new BasePacket().Deserialize(receivedbuffer);
                        Player receivedPlayer = null;

                        for (int i = 0; i < gameRoom.PlayersCount; i++)
                        {
                            if (gameRoom.GetPlayer(i).ID == bp.Player.ID)
                            {
                                receivedPlayer = gameRoom.GetPlayer(i);
                                receivedPlayer.LastRecievedPacketDateTime = bp.CreationTime;
                                break;
                            }
                        }

                        if (bp.NetworkMethod == PacketMethod.Request)
                        {
                            switch (bp.NetworkEvent)
                            {
                                case PacketEvent.ConnectToServer:
                                    {
                                        receivedPlayer = new Player(bp.Player.ID, bp.Player.Name, (IPEndPoint)senderRemote);
                                        receivedPlayer.LastRecievedPacketDateTime = bp.CreationTime;
                                        gameRoom.AddPlayer(receivedPlayer);

                                        socket.SendTo(new ConnectPacket().SuccessResponse(bp, receivedPlayer).Serialize(), receivedPlayer.ipEndpoint);
                                        Console.WriteLine($"Player {bp.Player.Name} connected!");
                                        break;
                                    }

                                case PacketEvent.DisconnectFromServer:
                                    {
                                        break;
                                    }

                                case PacketEvent.Instantiate:
                                    {
                                        for (int i = 0; i < gameRoom.PlayersCount; i++)
                                        {
                                            Player player = gameRoom.GetPlayer(i);

                                            if (bp.Player.ID != player.ID)
                                            {
                                                jobManager.AddJob(bp);
                                                socket.SendTo(receivedbuffer, player.ipEndpoint);
                                            }
                                        }
                                        break;
                                    }

                                case PacketEvent.TrackPosition:
                                    {
                                        for (int i = 0; i < gameRoom.PlayersCount; i++)
                                        {
                                            Player player = gameRoom.GetPlayer(i);

                                            if (bp.Player.ID != player.ID)
                                            {
                                                socket.SendTo(receivedbuffer, player.ipEndpoint);
                                            }
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                        else if (bp.NetworkMethod == PacketMethod.Response)
                        {
                            if (bp.NetworkEvent != PacketEvent.Unknown)
                            {
                                if (bp.NetworkResponse != PacketResponse.Unknown)
                                {
                                    jobManager.SubmitResponsePacket(bp);
                                }
                            }
                        }
                    }
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode != SocketError.WouldBlock)
                        Console.WriteLine(ex);
                }

                jobManager.Update(socket);

                try
                {
                    for (int i = gameRoom.PlayersCount - 1; i >= 0; i--)
                    {
                        Player player = gameRoom.GetPlayer(i);

                        if (player.RequirePing())
                        {
                            if (!player.IsConnected)
                            {
                                gameRoom.RemovePlayerAt(i);
                                Console.WriteLine($"{player.Name} got disconnected!");
                                continue;
                            }
                            Console.WriteLine($"Sending Ping Packet to player {player.Name}");
                            socket.SendTo(new PingPacket().PrepareRequest(player).Serialize(), player.ipEndpoint);
                        }
                    }
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode != SocketError.WouldBlock)
                        Console.WriteLine(ex);
                }
            }
        }
    }
}