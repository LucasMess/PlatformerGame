using CodenameAdam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Adam.Network
{
    class Server
    {
        UdpClient udpServer;
        TcpListener tcpListener;
        IPEndPoint serverIP;
        public List<IPEndPoint> clientIPs = new List<IPEndPoint>();
        public List<Client> clients = new List<Client>();
        public bool IsWaitingForPlayers { get; set; }
        BinaryFormatter formatter;

        public X509Certificate2 cert = new X509Certificate2("server.pfx", "instant");

        public Server()
        {
            //This is where the game will be hosted.
            IPEndPoint serverIP = new IPEndPoint(IPAddress.Any,42555);

            //The listener is to get incoming connections and the udp is for sending game data.
            udpServer = new UdpClient(serverIP);
            tcpListener = new TcpListener(serverIP);

            //Starts a new thread that will continually look for clients while the game has not started.
            IsWaitingForPlayers = true;
            new Thread(new ThreadStart(ListenForClients));
        }

        private void ListenForClients()
        {
            while (IsWaitingForPlayers)
            {
                TcpClient newTcpClient = tcpListener.AcceptTcpClient();
                Client newClient = new Client(this, newTcpClient);
            }
        }

        public void SendMapDataPacket(Map map)
        {
            MapDataPacket m = new MapDataPacket(map);
            byte[] mapBytes = CalcHelper.ToByteArray(m);

            foreach (var ip in clientIPs)
            {
                udpServer.Send(mapBytes, mapBytes.Length, ip);               
            }
        }

        public void SendCurrentLevel(Level CurrentLevel)
        {
            foreach (var cl in clients)
            {
                cl.SendCurrentLevel(CurrentLevel);
            }
        }

    }
}
