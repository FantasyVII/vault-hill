using System;

namespace GameNetworkLib.Packets
{
    public class BasePacket
    {
        public string ID { get; protected set; }
        public PacketMethod NetworkMethod { get; protected set; }
        public PacketEvent NetworkEvent { get; protected set; }
        public PacketResponse NetworkResponse { get; protected set; }
        public string ResponseMessage { get; protected set; }

        public string CreationTime { get; protected set; }

        public Player Player { get; protected set; }

        public BasePacket()
        {
            ID = Guid.NewGuid().ToString("N");
            NetworkMethod = PacketMethod.Unknown;
            NetworkEvent = PacketEvent.Unknown;
            NetworkResponse = PacketResponse.Unknown;
            ResponseMessage = "";
            CreationTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Player = new Player();
        }

        protected void SuccessResponse(BasePacket basePacket)
        {
            ID = basePacket.ID;
            NetworkMethod = PacketMethod.Response;
            NetworkEvent = basePacket.NetworkEvent;
            NetworkResponse = PacketResponse.Success;
            CreationTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        protected void FailResponse(BasePacket basePacket)
        {
            ID = basePacket.ID;
            NetworkMethod = PacketMethod.Response;
            NetworkEvent = basePacket.NetworkEvent;
            NetworkResponse = PacketResponse.Failure;
            CreationTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        protected void BeginWrite()
        {
            NetworkStream.BeginWrite();
            NetworkStream.Write(ID);
            NetworkStream.Write((int)NetworkMethod);
            NetworkStream.Write((int)NetworkEvent);
            NetworkStream.Write((int)NetworkResponse);
            NetworkStream.Write(ResponseMessage);
            NetworkStream.Write(CreationTime);

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

            ID = NetworkStream.ReadString();
            NetworkMethod = (PacketMethod)NetworkStream.ReadInt32();
            NetworkEvent = (PacketEvent)NetworkStream.ReadInt32();
            NetworkResponse = (PacketResponse)NetworkStream.ReadInt32();
            ResponseMessage = NetworkStream.ReadString();
            CreationTime = NetworkStream.ReadString();

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