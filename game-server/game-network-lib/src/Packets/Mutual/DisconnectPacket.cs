namespace GameNetworkLib.Packets.Mutual
{
    public class DisconnectPacket : BasePacket
    {
        public DisconnectPacket PrepareRequest(Player player)
        {
            NetworkMethod = PacketMethod.Request;
            NetworkEvent = PacketEvent.DisconnectFromServer;
            Player = player;
            return this;
        }

        public DisconnectPacket SuccessResponse(
            BasePacket basePacket,
            Player player,
            string responseMessage = "Disconnected from server successfully"
            )
        {
            base.SuccessResponse(basePacket);
            ResponseMessage = responseMessage;
            Player = player;
            return this;
        }

        public DisconnectPacket FailResponse(
            BasePacket basePacket,
            Player player,
            string responseMessage = "Failed to disconnect from server"
            )
        {
            base.SuccessResponse(basePacket);
            ResponseMessage = responseMessage;
            Player = player;
            return this;
        }
    }
}