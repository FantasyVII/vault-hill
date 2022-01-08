namespace ChatLib.Packets
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