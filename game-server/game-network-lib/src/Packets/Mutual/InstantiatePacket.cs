using UnityEngine;

namespace GameNetworkLib.Packets.Mutual
{
    public class InstantiatePacket : BasePacket
    {
        public string GameObjectName { get; private set; }
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }

        public InstantiatePacket()
        {
            GameObjectName = "";
            Position = Vector3.zero;
            Rotation = Quaternion.identity;
        }

        public InstantiatePacket PrepareRequest(
            Player player,
            string gameObjectName,
            Vector3 position,
            Quaternion rotation
            )
        {
            NetworkMethod = PacketMethod.Request;
            NetworkEvent = PacketEvent.Instantiate;
            Player = player;
            GameObjectName = gameObjectName;
            Position = position;
            Rotation = rotation;
            return this;
        }

        public InstantiatePacket SuccessResponse(
            BasePacket basePacket,
            Player player,
            string responseMessage = "Instantiated an object successfully"
            )
        {
            base.SuccessResponse(basePacket);
            ResponseMessage = responseMessage;
            Player = player;
            return this;
        }

        public InstantiatePacket FailResponse(
            BasePacket basePacket,
            Player player,
            string responseMessage = "Failed to instantiate an object"
            )
        {
            base.FailResponse(basePacket);
            ResponseMessage = responseMessage;
            Player = player;
            return this;
        }

        public override byte[] Serialize()
        {
            base.BeginWrite();

            NetworkStream.Write(GameObjectName);

            NetworkStream.Write(Position.x);
            NetworkStream.Write(Position.y);
            NetworkStream.Write(Position.z);

            NetworkStream.Write(Rotation.x);
            NetworkStream.Write(Rotation.y);
            NetworkStream.Write(Rotation.z);
            NetworkStream.Write(Rotation.w);

            return base.EndWrite();
        }

        public new InstantiatePacket Deserialize(byte[] buffer)
        {
            base.BeginRead(buffer);

            GameObjectName = NetworkStream.ReadString();

            Position = new Vector3(
                NetworkStream.ReadSingle(),
                NetworkStream.ReadSingle(),
                NetworkStream.ReadSingle()
                );

            Rotation = new Quaternion(
                NetworkStream.ReadSingle(),
                NetworkStream.ReadSingle(),
                NetworkStream.ReadSingle(),
                NetworkStream.ReadSingle()
                );

            base.EndRead();

            return this;
        }
    }
}