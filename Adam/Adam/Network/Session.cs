using Adam;
using Adam.GameData;
using Adam.Network.Packets;
using Adam.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Adam.Network
{
    public class Session
    {
        Server server;
        Connection connection;
        IPEndPoint serverIP;

        GameWorld gameWorld;

        string playerName;

        /// <summary>
        /// Returns true if the current client is host.
        /// </summary>
        public static bool IsHost
        {
            get; set;
        }

        /// <summary>
        /// The most recent entity packet received.
        /// </summary>
        public static EntityPacket EntityPacket
        {
            get; set;
        }

        /// <summary>
        /// The most recent level packet received.
        /// </summary>
        public static LevelPacket LevelPacket
        {
            get; set;
        }

        public static PlayerPacket PlayerPacket
        {
            get; set;
        }

        /// <summary>
        /// Returns true if the session is still happening.
        /// </summary>
        public static bool IsActive
        {
            get; set;
        }

        public Session(bool isHost, string playerName)
        {
            IsActive = true;
            Console.WriteLine("New session started. Is host? {0}, Player Name: {1}", isHost, playerName);
            this.playerName = playerName;
            IsHost = isHost;

            if (isHost)
            {
                //Server automatically starts listening for players.
                server = new Server();

                //Automatically connects to own server.
                //ConnectTo("192.168.1.1", 42555);
            }
            else
            {
                ConnectTo("127.0.0.1", 42555);
            }

        }

        public void ConnectTo(string address, int port)
        {
            //Connection automatically sets up a connection with server.
            connection = new Connection(address, port, playerName);
        }

        public void Start()
        {
            if (IsHost)
            {
                server.IsWaitingForPlayers = false;
                SendLevel();
            }
            new Thread(new ThreadStart(Update)).Start();
        }

        private void Update()
        {
            while (IsActive)
            {
                if (Main.IsLoadingContent)
                    continue;
                Console.WriteLine("Updating...");
                if (IsHost)
                {
                    server.SendEntityPacket(GameWorld.Instance);
                    Console.WriteLine("Entity packet sent.");
                    //PlayerPacket = connection.ReceivePlayerPacket();
                }
                else
                {
                    EntityPacket = connection.ReceiveEntityPacket();
                    Console.WriteLine("Entity packet received.");
                }
            }
        }

        public void SendTestMessage()
        {
            server.SendMessage("Hello World!");
        }

        public void SendLevel()
        {
            WorldConfigFile level = DataFolder.GetWorldConfigFile(Path.Combine(DataFolder.LevelDirectory, "Tutorial.lvl"));
            server.SendLevelPacket(level);
            level.LoadIntoPlay();
           // server.SendLevelPacket(new WorldConfigFile("Test",256, 256));
        }
    }
}
