namespace GameNetworkLib.Packets.Mutual
{
    public class ConnectPacket : BasePacket
    {
        public ConnectPacket PrepareRequest(Player player)
        {
            NetworkMethod = PacketMethod.Request;
            NetworkEvent = PacketEvent.ConnectToServer;
            Player = player;
            return this;
        }

        public ConnectPacket SuccessResponse(
            BasePacket basePacket,
            Player player,
            string responseMessage = "Connected to server successfully"
            )
        {
            base.SuccessResponse(basePacket);
            ResponseMessage = responseMessage;
            Player = player;
            return this;
        }

        public ConnectPacket FailResponse(
            BasePacket basePacket,
            Player player,
            string responseMessage = "Failed to connect to server"
            )
        {
            base.FailResponse(basePacket);
            ResponseMessage = responseMessage;
            Player = player;
            return this;
        }
    }
}