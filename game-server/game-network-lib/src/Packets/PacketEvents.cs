namespace GameNetworkLib.Packets
{
    public enum PacketEvents
    {
        Unknown = -1,
        None,
        AlivePing,

        ConnectToServer,
        DisconnectFromServer,
    }
}