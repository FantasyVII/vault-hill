namespace ChatLib.Packets.Mutual
{
    public class GlobalMessagePacket : BasePacket
    {
        public string Message { get; internal set; }

        public GlobalMessagePacket()
        {
            Message = "";
        }

        public GlobalMessagePacket PrepareRequest(Player player, string message)
        {
            Request = PacketRequest.GlobalChatMessage;
            Player = player;
            Message = message;
            return this;
        }

        public GlobalMessagePacket FailResponse(
            Player player, 
            string responseMessage = "Failed to receive global chat message"
            )
        {
            Response = PacketResponse.FailedToReceivedGlobalChatMessage;
            ResponseMessage = responseMessage;
            Player = player;
            return this;
        }

        public GlobalMessagePacket SuccessResponse(
            Player player,
            string responseMessage = "Global chat message received successfully"
            )
        {
            Response = PacketResponse.GlobalChatMessageReceivedSuccessfully;
            ResponseMessage = responseMessage;
            Player = player;
            return this;
        }

        public override byte[] Serialize()
        {
            base.BeginWrite();

            NetworkStream.Write(Message);

            return base.EndWrite();
        }

        public new GlobalMessagePacket Deserialize(byte[] buffer)
        {
            base.BeginRead(buffer);

            Message = NetworkStream.ReadString();

            base.EndRead();

            return this;
        }
    }
}