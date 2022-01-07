namespace ChatLib.Packets.Mutual
{
    public class DisconnectPacket : BasePacket
    {
        public DisconnectPacket PrepareRequest(Player player)
        {
            Request = PacketRequest.DisconnectFromServer;
            Player = player;
            return this;
        }

        public DisconnectPacket FailResponse(
            Player player,
            string responseMessage = "Failed to disconnect from server"
            )
        {
            Response = PacketResponse.FailedToDisconnectFromServer;
            ResponseMessage = responseMessage;
            Player = player;
            return this;
        }

        public DisconnectPacket SuccessResponse(
            Player player,
            string responseMessage = "Disconnected from server successfully"
            )
        {
            Response = PacketResponse.DisconnectedFromServerSuccessfully;
            ResponseMessage = responseMessage;
            Player = player;
            return this;
        }
    }
}