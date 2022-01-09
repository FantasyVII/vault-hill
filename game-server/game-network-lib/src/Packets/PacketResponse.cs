namespace GameNetworkLib.Packets
{
    public enum PacketResponse
    {
        Unknown = -1,
        None,
        Alive,

        ConnectedToServerSuccessfully,
        FailedToConnectToServer,

        DisconnectedFromServerSuccessfully,
        FailedToDisconnectFromServer,
    }
}