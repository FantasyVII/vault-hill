using System;
using System.Net;
using System.Net.Sockets;
using GameNetworkLib;
using GameNetworkLib.Packets;
using GameNetworkLib.Packets.Mutual;

namespace server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 3000));
            socket.Blocking = false;

            GameRoom gameRoom = new GameRoom();

            while (true)
            {
                try
                {
                    if (socket.Available > 0)
                    {
                        byte[] buffer = new byte[socket.Available];
                        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                        EndPoint senderRemote = sender;

                        socket.ReceiveFrom(buffer, ref senderRemote);
                        BasePacket bp = new BasePacket().Deserialize(buffer);
                        Player player = null;

                        for (int i = 0; i < gameRoom.PlayersCount; i++)
                        {
                            if (gameRoom.GetPlayer(i).ID == bp.Player.ID)
                            {
                                player = gameRoom.GetPlayer(i);
                                player.LastRecievedPacketDateTime = bp.CreationTime;
                                break;
                            }
                        }

                        switch (bp.NetworkEvents)
                        {
                            case PacketEvents.ConnectToServer:
                                {
                                    player = new Player(bp.Player.ID, bp.Player.Name, (IPEndPoint)senderRemote);
                                    player.LastRecievedPacketDateTime = bp.CreationTime;
                                    gameRoom.AddPlayer(player);

                                    Console.WriteLine(socket.SendTo(new ConnectPacket().SuccessResponse(player).Serialize(), player.ipEndpoint));
                                    Console.WriteLine($"Player {bp.Player.Name} connected!");
                                    break;
                                }
                            case PacketEvents.DisconnectFromServer:
                                {
                                    break;
                                }

                            default:
                                break;
                        }

                        switch (bp.Response)
                        {
                            case PacketResponse.Alive:
                                {
                                    Console.WriteLine("Reponsed to ping");
                                    break;
                                }

                            case PacketResponse.ConnectedToServerSuccessfully:
                                break;
                            case PacketResponse.FailedToConnectToServer:
                                break;
                            case PacketResponse.DisconnectedFromServerSuccessfully:
                                break;
                            case PacketResponse.FailedToDisconnectFromServer:
                                break;

                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {

                }


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
                catch
                {

                }
            }
        }
    }
}