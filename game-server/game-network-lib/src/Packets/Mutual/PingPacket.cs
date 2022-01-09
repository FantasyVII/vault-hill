namespace GameNetworkLib.Packets.Mutual
{
    public class PingPacket : BasePacket
    {
        public PingPacket PrepareRequest(Player player)
        {
            NetworkMethod = PacketMethod.Request;
            NetworkEvent = PacketEvent.AlivePing;
            Player = player;
            return this;
        }

        public PingPacket SuccessResponse(
            BasePacket basePacket,
            Player player,
            string responseMessage = "Alive"
            )
        {
            base.SuccessResponse(basePacket);
            ResponseMessage = responseMessage;
            Player = player;
            return this;
        }
    }
}