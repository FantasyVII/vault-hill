using System;
using System.Net;

namespace GameNetworkLib
{
    public class Player
    {
        public string ID { get; internal set; }
        public string Name { get; internal set; }

        public IPEndPoint ipEndpoint;

        string lastRecievedPacketDateTime;
        public string LastRecievedPacketDateTime 
        { 
            get
            {
                return lastRecievedPacketDateTime;
            }

            set
            {
                totalPingsWithoutResponse = 0;
                lastSentPingPacketDateTime = value;
                lastRecievedPacketDateTime = value;
            }
        }

        string lastSentPingPacketDateTime;
        int totalPingsWithoutResponse;

        public bool IsConnected { get; private set; }

        public Player()
        {
            ID = "";
            Name = "";
            lastSentPingPacketDateTime = "";
            totalPingsWithoutResponse = 0;
            IsConnected = true;
        }

        public Player(string id, string name, IPEndPoint ipEndpoint)
        {
            ID = id;
            Name = name;
            this.ipEndpoint = ipEndpoint;
            lastSentPingPacketDateTime = "";
            totalPingsWithoutResponse = 0;
            IsConnected = true;
        }

        public bool RequirePing()
        {
            if (totalPingsWithoutResponse >= 5)
            {
                IsConnected = false;
                return true;
            }

            if (totalPingsWithoutResponse == 0)
            {
                DateTime otherPacketDateTime = Convert.ToDateTime(LastRecievedPacketDateTime);
                DateTime currentDateTime = DateTime.UtcNow;
                TimeSpan diff = currentDateTime.Subtract(otherPacketDateTime);

                if (diff.TotalSeconds >= 10)
                {
                    lastSentPingPacketDateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    totalPingsWithoutResponse++;
                    return true;
                }
            }
            else if (totalPingsWithoutResponse > 0)
            {
                DateTime otherPacketDateTime = Convert.ToDateTime(lastSentPingPacketDateTime);
                DateTime currentDateTime = DateTime.UtcNow;
                TimeSpan diff = currentDateTime.Subtract(otherPacketDateTime);

                if (diff.TotalSeconds >= 10)
                {
                    lastSentPingPacketDateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    totalPingsWithoutResponse++;
                    return true;
                }
            }

            return false;
        }
    }
}