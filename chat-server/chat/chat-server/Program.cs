using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using ChatLib;
using ChatLib.Packets;
using ChatLib.Packets.Mutual;

namespace ChatServer
{
    namespace LobbyServer
    {
        class Program
        {
            static void Main(string[] args)
            {
                Lobby lobby = new Lobby();

                Socket acceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                acceptSocket.Blocking = false;
                acceptSocket.Bind(new IPEndPoint(IPAddress.Any, 42069));
                acceptSocket.Listen(10);

                Console.WriteLine("Server listening...");
                List<Socket> sockets = new List<Socket>();

                while (true)
                {
                    try
                    {
                        sockets.Add(acceptSocket.Accept());
                        Console.WriteLine("Client connected!");
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode != SocketError.WouldBlock)
                            Console.WriteLine(ex);
                    }

                    for (int i = sockets.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            if (sockets[i].Available > 0)
                            {
                                Console.WriteLine("Player identification data received from player!");

                                byte[] buffer = new byte[sockets[i].Available];
                                sockets[i].Receive(buffer);

                                BasePacket bp = new BasePacket();
                                bp.Deserialize(buffer);

                                switch (bp.Request)
                                {
                                    case PacketRequest.ConnectToServer:
                                        {
                                            Player player = lobby.AddPlayer(new Player(bp.Player.ID, bp.Player.Name, sockets[i]));
                                            Console.WriteLine($"Player ID {bp.Player.ID} and name {bp.Player.Name}. Players count in lobby {lobby.PlayersCount}");

                                            sockets[i].Send(new ConnectPacket().SuccessResponse(player).Serialize());
                                            sockets.RemoveAt(i);
                                            break;
                                        }
                                }
                            }
                        }
                        catch (SocketException ex)
                        {
                            if (ex.SocketErrorCode != SocketError.WouldBlock)
                                Console.WriteLine(ex);
                        }
                    }

                    for (int i = lobby.PlayersCount - 1; i >= 0; i--)
                    {
                        try
                        {
                            Player player = lobby.GetPlayer(i);

                            if (player.socket.Available > 0)
                            {
                                Console.WriteLine("Player identification data received from player!");

                                byte[] buffer = new byte[player.socket.Available];
                                player.socket.Receive(buffer);

                                BasePacket bp = new BasePacket();
                                bp.Deserialize(buffer);

                                switch (bp.Request)
                                {
                                    case PacketRequest.SendGlobalChatMessage:
                                        {
                                            for (int j = 0; j < lobby.PlayersCount; j++)
                                            {
                                                Player other = lobby.GetPlayer(j);
                                                if (other.ID != bp.Player.ID)
                                                {
                                                    Console.WriteLine("Message sent");
                                                    other.socket.Send(buffer);
                                                }
                                            }
                                            break;
                                        }

                                    case PacketRequest.SendPrivateChatMessage:
                                        {
                                            string receiverId = new PrivateMessagePacket().Deserialize(buffer).ReceiverID;

                                            for (int j = 0; j < lobby.PlayersCount; j++)
                                            {
                                                if (lobby.GetPlayer(i).ID == receiverId)
                                                {
                                                    lobby.GetPlayer(i).socket.Send(buffer);
                                                    break;
                                                }
                                            }
                                            break;
                                        }

                                    default:
                                        break;
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
    }
}