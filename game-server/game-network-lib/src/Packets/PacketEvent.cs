namespace GameNetworkLib.Packets
{
    public enum PacketEvent
    {
        Unknown = -1,
        AlivePing,

        ConnectToServer,
        DisconnectFromServer,
        Instantiate,
        TrackPosition
    }
}