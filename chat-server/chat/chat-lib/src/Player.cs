using System.Net.Sockets;

namespace ChatLib
{
    public class Player
    {
        public string ID { get; internal set; }
        public string Name { get; internal set; }

        public Socket socket;

        public Player()
        {
        }

        public Player(string id, string name, Socket socket)
        {
            ID = id;
            Name = name;
            this.socket = socket;
        }
    }
}