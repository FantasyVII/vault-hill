using ChatLib;
using ChatLib.Packets;
using ChatLib.Packets.Mutual;
using System;
using System.Net;
using System.Net.Sockets;

namespace udp_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);
            socket.Connect(ipEndPoint);
            socket.Blocking = false;
            Player player = new Player(Guid.NewGuid().ToString("N"), "Vivi", ipEndPoint);

            socket.Send(new ConnectPacket().PrepareRequest(player).Serialize());

            while (true)
            {
                if (socket.Available > 0)
                {
                    try
                    {
                        byte[] receivedBuffer = new byte[socket.Available];
                        socket.Receive(receivedBuffer);

                        BasePacket bp = new BasePacket().Deserialize(receivedBuffer);
                        
                        switch (bp.NetworkEvents)
                        {
                            case PacketEvents.AlivePing:
                                Console.WriteLine("Recieved Ping");
                                socket.Send(new PingPacket().SuccessResponse(player).Serialize());
                                break;
                            case PacketEvents.ConnectToServer:
                                break;
                            case PacketEvents.DisconnectFromServer:
                                break;
                            default:
                                break;
                        }

                        switch (bp.Response)
                        {
                            case PacketResponse.ConnectedToServerSuccessfully:
                                {
                                    Console.WriteLine("Connected to server!");
                                    break;
                                }
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
                    catch
                    {

                    }
                }
            }
        }
    }
}