using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using GameNetworkLib;
using GameNetworkLib.Packets;
using GameNetworkLib.Packets.Mutual;

public class GameNetwork : MonoBehaviour
{
    static GameNetwork instance;

    Socket socket;
    Player player;

    bool runNetworkLoop;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        runNetworkLoop = false;
        ConnectToServer("Vivi");
    }

    public void ConnectToServer(string username)
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);
        socket.Connect(ipEndPoint);
        socket.Blocking = false;
        player = new Player(Guid.NewGuid().ToString("N"), "Vivi", ipEndPoint);

        socket.Send(new ConnectPacket().PrepareRequest(player).Serialize());
        runNetworkLoop = true;
    }

    void Update()
    {
        if (!runNetworkLoop)
            return;


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
                        print("Recieved Ping");
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
                            print("Connected to server!");
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
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode != SocketError.WouldBlock)
                    Debug.LogError(ex);
            }
        }
    }
}