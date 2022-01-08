using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class GameNetwork : MonoBehaviour
{
    static GameNetwork instance;

    Socket socket;

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
        socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000));
        socket.Blocking = false;
        //byte[] recieveBuffer = new byte[1024];
        socket.Send(Encoding.ASCII.GetBytes("Hello"));
        runNetworkLoop = true;
    }

    void Update()
    {
        if (!runNetworkLoop)
            return;

        try
        {
            if (socket.Available > 0)
            {
                /*byte[] recieveBuffer = new byte[socket.Available];
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
            }*/
            }
        }
        catch (SocketException ex)
        {
            if (ex.SocketErrorCode != SocketError.WouldBlock)
                Debug.LogError(ex);
        }
    }
}