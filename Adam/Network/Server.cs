using Adam.GameData;
using Adam.Network.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Adam.Network
{
    public class Server
    {
        UdpClient _udpServer;
        TcpListener _tcpListener;
        IPEndPoint _serverIp;


        public List<IPEndPoint> ClientIPs = new List<IPEndPoint>();
        public List<Client> Clients = new List<Client>();
        public bool IsWaitingForPlayers { get; set; }

        BinaryFormatter _formatter;

        public X509Certificate2 Cert = new X509Certificate2();

        public Server()
        {
            Console.WriteLine("Starting server...");

            //This is where the game will be hosted.
            IPEndPoint serverIp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 42555);
            Console.WriteLine("Server IP: {0}", serverIp);


            //The listener is to get incoming connections and the udp is for sending game data.
            _udpServer = new UdpClient(new IPEndPoint(serverIp.Address, 42557));

            Console.WriteLine("UDP server set up.");
            _tcpListener = new TcpListener(serverIp);
            _tcpListener.Start();
            Console.WriteLine("TCP listener set up.");


            //Starts a new thread that will continually look for clients while the game has not started.
            IsWaitingForPlayers = true;
            new Thread(new ThreadStart(ListenForClients)).Start();
            Console.WriteLine("Waiting for players...");
        }

        private void ListenForClients()
        {
            while (IsWaitingForPlayers)
            {
                Console.WriteLine("Waiting...");
                TcpClient newTcpClient = _tcpListener.AcceptTcpClient();
                Console.WriteLine("A player is attempting to connect to the server...");
                Client newClient = new Client(this, newTcpClient);
            }
        }

        /// <summary>
        /// Sends an entity packet to all clients.
        /// </summary>
        public void SendEntityPacket()
        {
            EntityPacket en = new EntityPacket();
            byte[] packet = CalcHelper.ToByteArray(en);
            SendToClients(packet);
        }

        /// <summary>
        /// Send a level packet of the specified world config file.
        /// </summary>
        /// <param name="config"></param>
        public void SendLevelPacket(WorldConfigFile config)
        {
            byte[] data = CalcHelper.ToByteArray(new LevelPacket(config));
            foreach (Client c in Clients)
            {
                c.SendLevelOverTcp(data);
            }
        }

        /// <summary>
        /// Used to send a byte packet to all clients via UDP.
        /// </summary>
        /// <param name="packet"></param>
        private void SendToClients(byte[] packet)
        {
            foreach (var ip in ClientIPs)
            {
                IPEndPoint actualIp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 42559);
                Console.WriteLine("Packet sent to {0}, from: {1}", actualIp, _udpServer.Client.LocalEndPoint);
                _udpServer.Send(packet, packet.Length, actualIp);
            }
        }

        public void SendMessage(string message)
        {
            foreach (Client c in Clients)
            {
                c.SendMessageOverTcp(message);
            }
        }

    }
}
