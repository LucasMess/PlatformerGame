using Adam;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Adam.Network
{
    class Session
    {
        Server server;
        Connection connection;
        IPEndPoint serverIP;
        Level CurrentLevel = Level.Level0;

        Map map;

        string playerName;
        bool isHost;
        bool inSession;

        public Session(bool isHost, string playerName)
        {
            this.playerName = playerName;
            this.isHost = isHost;

            if (isHost)
            {
                //Server automatically starts listening for players.
                server = new Server();

                //Automatically connects to own server.
                ConnectTo("localhost", 42555);
            }
        }

        public void ConnectTo(string address, int port)
        {
            //Connection automatically sets up a connection with server.
            connection = new Connection(address, port, playerName);
        }

        public void Start(Level Currentlevel)
        {
            this.CurrentLevel = Currentlevel;
            inSession = true;
            server.IsWaitingForPlayers = false;
            server.SendCurrentLevel(Currentlevel);
            new Thread(new ThreadStart(SendUpdatePackets));
        }

        public void Update(GameTime gameTime)
        {
            if (isHost)
            {
               // map.Update(gameTime, CurrentLevel, camera);
            }
            else
            {
                MapDataPacket m = connection.ReceiveMapDataPacket();
                map.UpdateFromDataPacket(m);
            }
        }

        private void SendUpdatePackets()
        {
            while (inSession)
            {
                server.SendMapDataPacket(map);
            }
        }
    }
}
