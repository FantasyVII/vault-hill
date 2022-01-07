namespace ChatLib.Packets
{
    public enum PacketResponse
    {
        Unknown = -1,
        None,

        ConnectedToServerSuccessfully,
        FailedToConnectToServer,

        DisconnectedFromServerSuccessfully,
        FailedToDisconnectFromServer,

        GlobalChatMessageReceivedSuccessfully,
        FailedToReceivedGlobalChatMessage,

        PrivateChatMessageReceivedSuccessfully,
        FailedToReceivedPrivateChatMessage,
    }
}