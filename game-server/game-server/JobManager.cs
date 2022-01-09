using System;
using System.Collections.Generic;
using System.Net.Sockets;
using GameNetworkLib.Packets;

namespace GameServer
{
    class JobManager
    {
        List<NetworkJob> jobs;

        GameRoom gameRoom;

        public JobManager(GameRoom gameRoom)
        {
            jobs = new List<NetworkJob>();

            this.gameRoom = gameRoom;
        }

        public void AddJob(BasePacket requestPacket)
        {
            bool jobAlreadyExist = false;

            for (int i = 0; i < jobs.Count; i++)
            {
                if (jobs[i].BasePacket.ID == requestPacket.ID)
                {
                    jobAlreadyExist = true;
                    break;
                }
            }

            if (!jobAlreadyExist)
                jobs.Add(new NetworkJob(requestPacket, gameRoom));
        }

        public void SubmitResponsePacket(BasePacket responsePacket)
        {
            for (int i = 0; i < jobs.Count; i++)
            {
                if (jobs[i].BasePacket.ID == responsePacket.ID)
                {
                    jobs[i].SubmitResponsePacket(responsePacket);
                    break;
                }
            }
        }

        public void Update(Socket socket)
        {
            for (int i = jobs.Count - 1; i >= 0; i--)
            {
                jobs[i].Update(socket);

                if(jobs[i].Abort)
                {
                    jobs.RemoveAt(i);
                    Console.WriteLine("Aborting... Removing job");
                }
            }
        }
    }
}