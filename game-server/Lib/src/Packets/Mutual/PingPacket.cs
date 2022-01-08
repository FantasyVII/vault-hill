namespace ChatLib.Packets.Mutual
{
    public class PingPacket : BasePacket
    {
        public PingPacket PrepareRequest(Player player)
        {
            NetworkEvents = PacketEvents.AlivePing;
            Player = player;
            return this;
        }

        public PingPacket SuccessResponse(
            Player player,
            string responseMessage = "Alive"
            )
        {
            Response = PacketResponse.Alive;
            ResponseMessage = responseMessage;
            Player = player;
            return this;
        }
    }
}