using System.IO;

namespace ChatLib.Packets
{
    public class BasePacket
    {
        public PacketRequest Request { get; protected set; }
        public PacketResponse Response { get; protected set; }
        public string ResponseMessage { get; protected set; }

        public Player Player { get; protected set; }

        public BasePacket()
        {
            Request = PacketRequest.Unknown;
            ResponseMessage = "";
            Player = new Player();
        }

        protected void BeginWrite()
        {
            NetworkStream.BeginWrite();
            NetworkStream.Write((int)Request);
            NetworkStream.Write((int)Response);
            NetworkStream.Write(ResponseMessage);

            NetworkStream.Write(Player.ID);
            NetworkStream.Write(Player.Name);
        }

        protected byte[] EndWrite()
        {
            return NetworkStream.EndWrite();
        }

        protected void BeginRead(byte[] buffer)
        {
            NetworkStream.BeginRead(buffer);

            Request = (PacketRequest)NetworkStream.ReadInt32();
            Response = (PacketResponse)NetworkStream.ReadInt32();
            ResponseMessage = NetworkStream.ReadString();

            Player.ID = NetworkStream.ReadString();
            Player.Name = NetworkStream.ReadString();
        }

        protected void EndRead()
        {
            NetworkStream.EndRead();
        }

        public virtual byte[] Serialize()
        {
            BeginWrite();
            return EndWrite();
        }

        public virtual BasePacket Deserialize(byte[] buffer)
        {
            BeginRead(buffer);
            EndRead();

            return this;
        }
    }
}