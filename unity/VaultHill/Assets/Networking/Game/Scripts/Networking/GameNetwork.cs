using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using GameNetworkLib;
using GameNetworkLib.Packets;
using GameNetworkLib.Packets.Mutual;
using System.Collections.Generic;

public class GameNetwork : MonoBehaviour
{
    static GameNetwork instance;

    public delegate void ConnectedToServerEvent();
    public ConnectedToServerEvent ConnectedToServer;

    Socket socket;
    public Player player { get; private set; }

    bool runNetworkLoop;

    public delegate void NetworkUpdateHandler(GameNetwork gameNetwork);
    public NetworkUpdateHandler NetworkUpdate;

    float timer;
    const float triggerTime = 1.0f / 30.0f;

    public List<NetworkComponent> registeredNetworkComponents;

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
        registeredNetworkComponents = new List<NetworkComponent>();
        runNetworkLoop = false;
    }

    public void ConnectToServer(string username)
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("34.79.237.128"), 3000);
        socket.Connect(ipEndPoint);
        socket.Blocking = false;
        player = new Player(Guid.NewGuid().ToString("N"), username, ipEndPoint);

        socket.Send(new ConnectPacket().PrepareRequest(player).Serialize());
        runNetworkLoop = true;
    }

    public void SendPosition(Vector3 position)
    {
        socket.Send(new PositionPacket().PrepareRequest(player, position).Serialize());
    }

    public void InstantiatePlayer()
    {
        socket.Send(new InstantiatePacket().PrepareRequest(
            player, "Player", Vector3.zero, Quaternion.identity).Serialize());

        GameObject Go = InstantiateFromResources("Player", Vector3.zero, Quaternion.identity);
        Go.GetComponent<MyPlayer>().SetUsername(player.Name);
        Go.GetComponent<NetworkComponent>().OwnerID = player.ID;
    }

    private GameObject InstantiateFromResources(string gameObjectName, Vector3 position, Quaternion rotation)
    {
        GameObject player = Resources.Load<GameObject>("Player");
        return Instantiate(player, position, rotation);
    }

    void Update()
    {
        if (!runNetworkLoop)
            return;

        timer += Time.deltaTime;

        if (timer >= triggerTime)
        {
            if (NetworkUpdate != null)
                NetworkUpdate(this);
        }

        if (socket.Available > 0)
        {
            try
            {
                byte[] receivedBuffer = new byte[socket.Available];
                socket.Receive(receivedBuffer);

                BasePacket bp = new BasePacket().Deserialize(receivedBuffer);

                if (bp.NetworkMethod == PacketMethod.Request)
                {
                    switch (bp.NetworkEvent)
                    {
                        case PacketEvent.AlivePing:
                            socket.Send(new PingPacket().SuccessResponse(bp, player).Serialize());
                            break;
                        case PacketEvent.ConnectToServer:
                            break;
                        case PacketEvent.DisconnectFromServer:
                            break;
                        case PacketEvent.Instantiate:
                            {
                                InstantiatePacket ip = new InstantiatePacket().Deserialize(receivedBuffer);
                                GameObject Go = InstantiateFromResources("Player", ip.Position, ip.Rotation);
                                Go.GetComponent<MyPlayer>().SetUsername(ip.Player.Name);
                                Go.GetComponent<NetworkComponent>().OwnerID = ip.Player.ID;
                                socket.Send(new InstantiatePacket().SuccessResponse(bp, player).Serialize());
                                break;
                            }
                        case PacketEvent.TrackPosition:
                            {
                                PositionPacket pp = new PositionPacket().Deserialize(receivedBuffer);
                                for (int i = 0; i < registeredNetworkComponents.Count; i++)
                                {
                                    if (registeredNetworkComponents[i].OwnerID == pp.Player.ID)
                                    {
                                        registeredNetworkComponents[i].transform.position = pp.Position;
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
                    switch (bp.NetworkEvent)
                    {
                        case PacketEvent.ConnectToServer:
                            {
                                if (bp.NetworkResponse == PacketResponse.Success)
                                {
                                    ConnectedToServer();
                                }
                                break;
                            }
                        case PacketEvent.DisconnectFromServer:
                            break;
                        case PacketEvent.Instantiate:
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
}