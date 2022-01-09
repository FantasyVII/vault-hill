using System;

namespace GameNetworkLib.Packets
{
    public class BasePacket
    {
        public PacketEvents NetworkEvents { get; protected set; }
        public PacketResponse Response { get; protected set; }
        public string ResponseMessage { get; protected set; }

        public Player Player { get; protected set; }
        public string CreationTime { get; protected set; }

        public BasePacket()
        {
            CreationTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            NetworkEvents = PacketEvents.Unknown;
            ResponseMessage = "";
            Player = new Player();
        }

        protected void BeginWrite()
        {
            NetworkStream.BeginWrite();
            NetworkStream.Write(CreationTime);
            NetworkStream.Write((int)NetworkEvents);
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

            CreationTime = NetworkStream.ReadString();
            NetworkEvents = (PacketEvents)NetworkStream.ReadInt32();
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
            Player.LastRecievedPacketDateTime = CreationTime;
            return this;
        }

        public TimeSpan GetTimeDifferance(BasePacket otherPacket)
        {
            DateTime otherPacketDateTime = Convert.ToDateTime(otherPacket.CreationTime);
            DateTime currentDateTime = DateTime.UtcNow;
            TimeSpan diff = currentDateTime.Subtract(otherPacketDateTime);
            return diff;
        }

        public TimeSpan GetTimeDifferance(BasePacket firstPacket, BasePacket secondPacket)
        {
            DateTime firstPacketDateTime = Convert.ToDateTime(firstPacket.CreationTime);
            DateTime secondPacketDateTime = Convert.ToDateTime(secondPacket.CreationTime);
            TimeSpan diff = firstPacketDateTime.Subtract(secondPacketDateTime);
            return diff;
        }
    }
}