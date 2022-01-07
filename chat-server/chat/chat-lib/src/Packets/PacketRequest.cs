namespace ChatLib.Packets
{
    public enum PacketRequest
    {
        Unknown = -1,
        None,

        ConnectToServer,
        DisconnectFromServer,
        SendGlobalChatMessage,
        SendPrivateChatMessage,
    }
}