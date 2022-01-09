namespace GameNetworkLib.Packets.Mutual
{
    public class ConnectPacket : BasePacket
    {
        public ConnectPacket PrepareRequest(Player player)
        {
            NetworkEvents = PacketEvents.ConnectToServer;
            Player = player;
            return this;
        }

        public ConnectPacket FailResponse(
            Player player, 
            string responseMessage = "Failed to connect to server"
            )
        {
            Response = PacketResponse.FailedToConnectToServer;
            ResponseMessage = responseMessage;
            Player = player;
            return this;
        }

        public ConnectPacket SuccessResponse(
            Player player,
            string responseMessage = "Connected to server successfully"
            )
        {
            Response = PacketResponse.ConnectedToServerSuccessfully;
            ResponseMessage = responseMessage;
            Player = player;
            return this;
        }
    }
}