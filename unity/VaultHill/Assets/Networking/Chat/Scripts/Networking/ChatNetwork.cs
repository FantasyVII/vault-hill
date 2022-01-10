using System;
using System.Net;
using System.Net.Sockets;
using ChatLib;
using ChatLib.Packets;
using ChatLib.Packets.Mutual;
using UnityEngine;

public class ChatNetwork : MonoBehaviour
{
    static ChatNetwork instance;

    public delegate void ConnectedToServerEvent();
    public ConnectedToServerEvent ConnectedToServer;

    public delegate void FailedToConnectToServerEvent();
    public FailedToConnectToServerEvent FailedToConnectToServer;


    public delegate void RecievedChatMessageEvent(string name, string message);
    public RecievedChatMessageEvent RecievedChatMessage;

    Socket socket;
    public Player player;

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
    }

    public void ConnectToServer(string username)
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(new IPEndPoint(IPAddress.Parse("34.79.237.128"), 4000));
        socket.Blocking = false;
        runNetworkLoop = true;

        player = new Player(Guid.NewGuid().ToString("N"), username, socket);
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
        if (!runNetworkLoop)
            return;

        try
        {
            if (socket.Available > 0)
            {
                byte[] recieveBuffer = new byte[socket.Available];
                socket.Receive(recieveBuffer);

                BasePacket bp = new BasePacket();
                bp.Deserialize(recieveBuffer);

                switch (bp.Response)
                {
                    case PacketResponse.ConnectedToServerSuccessfully:
                        ConnectedToServer();
                        break;

                    case PacketResponse.FailedToConnectToServer:
                        FailedToConnectToServer();
                        break;

                    default:
                        break;
                }

                //---------------------------------------------------------------------------------------------------------------------
                //---------------------------------------------------------------------------------------------------------------------

                switch (bp.Request)
                {
                    case PacketRequest.GlobalChatMessage:
                        GlobalMessagePacket gmp = new GlobalMessagePacket().Deserialize(recieveBuffer);
                        socket.Send(new GlobalMessagePacket().SuccessResponse(player).Serialize());
                        RecievedChatMessage(gmp.Player.Name, gmp.Message);
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