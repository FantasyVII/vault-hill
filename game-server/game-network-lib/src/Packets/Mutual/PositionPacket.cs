using UnityEngine;

namespace GameNetworkLib.Packets.Mutual
{
    public class PositionPacket : BasePacket
    {
        public Vector3 Position { get; private set; }

        public PositionPacket()
        {
            Position = Vector3.zero;
        }

        public PositionPacket PrepareRequest(
            Player player,
            Vector3 position
            )
        {
            NetworkMethod = PacketMethod.Request;
            NetworkEvent = PacketEvent.TrackPosition;
            Player = player;
            Position = position;
            return this;
        }

        public override byte[] Serialize()
        {
            base.BeginWrite();

            NetworkStream.Write(Position.x);
            NetworkStream.Write(Position.y);
            NetworkStream.Write(Position.z);

            return base.EndWrite();
        }

        public new PositionPacket Deserialize(byte[] buffer)
        {
            base.BeginRead(buffer);

            Position = new Vector3(
                NetworkStream.ReadSingle(),
                NetworkStream.ReadSingle(),
                NetworkStream.ReadSingle()
                );

            base.EndRead();

            return this;
        }
    }
}