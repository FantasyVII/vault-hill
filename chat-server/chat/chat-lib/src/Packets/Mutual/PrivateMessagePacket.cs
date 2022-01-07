namespace ChatLib.Packets.Mutual
{
    public class PrivateMessagePacket : BasePacket
    {
        public string ReceiverID { get; internal set; }
        public string Message { get; internal set; }

        public PrivateMessagePacket PrepareRequest(Player player, string receiverID, string message)
        {
            Request = PacketRequest.SendPrivateChatMessage;
            Player = player;
            ReceiverID = receiverID;
            Message = message;
            return this;
        }

        public PrivateMessagePacket FailResponse(
            Player player,
            string responseMessage = "Failed to receive private chat message"
            )
        {
            Response = PacketResponse.FailedToReceivedPrivateChatMessage;
            ResponseMessage = responseMessage;
            Player = player;
            return this;
        }

        public PrivateMessagePacket SuccessResponse(
            Player player,
            string responseMessage = "Private chat message received successfully"
            )
        {
            Response = PacketResponse.PrivateChatMessageReceivedSuccessfully;
            ResponseMessage = responseMessage;
            Player = player;
            return this;
        }

        public override byte[] Serialize()
        {
            base.BeginWrite();

            NetworkStream.Write(ReceiverID);
            NetworkStream.Write(Message);

            return base.EndWrite();
        }

        public new PrivateMessagePacket Deserialize(byte[] buffer)
        {
            base.BeginRead(buffer);

            ReceiverID = NetworkStream.ReadString();
            Message = NetworkStream.ReadString();

            base.EndRead();

            return this;
        }
    }
}