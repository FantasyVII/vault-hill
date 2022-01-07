using System.Net;
using System.Net.Sockets;
using ChatLib;
using ChatLib.Packets;
using ChatLib.Packets.Mutual;
using UnityEngine;

public class ChatNetwork : MonoBehaviour
{
    public delegate void RecievedMessageEvent(string name, string message);
    public RecievedMessageEvent OnRecievedMessage;

    Socket socket;
    Player player;

    void Start()
    {
        socket = new Socket(
              AddressFamily.InterNetwork,
              SocketType.Stream,
              ProtocolType.Tcp
              );

        socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 42069));
        socket.Blocking = false;

        player = new Player(Random.Range(0, 100).ToString(), "Vivi", socket);
        print(player.ID);
        socket.Send(new ConnectPacket().PrepareRequest(player).Serialize());
    }

    public void SendChatMessage(string message)
    {
        try
        {
            socket.Send(new GlobalMessagePacket().PrepareRequest(player, message).Serialize());
        }
        catch (SocketException ex)
        {
            if (ex.SocketErrorCode != SocketError.WouldBlock)
                Debug.LogError(ex);
        }
    }

    void Update()
    {
        try
        {
            if (socket.Available > 0)
            {
                byte[] recieveBuffer = new byte[socket.Available];
                socket.Receive(recieveBuffer);

                BasePacket bp = new BasePacket();
                bp.Deserialize(recieveBuffer);

                switch (bp.Request)
                {
                    case PacketRequest.SendGlobalChatMessage:
                        print(new GlobalMessagePacket().Deserialize(recieveBuffer).Message);
                        break;

                    default:
                        break;
                }
            }
        }
        catch (SocketException ex)
        {
            if (ex.SocketErrorCode != SocketError.WouldBlock)
                Debug.LogError(ex);
        }
    }
}