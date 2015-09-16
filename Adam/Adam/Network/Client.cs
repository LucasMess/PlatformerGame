using Adam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Authentication;
using System.Text;
using System.Threading;

namespace Adam.Network
{
    public class Client
    {
        Server server;
        TcpClient tcpClient;
        NetworkStream netStream;
        SslStream ssl;
        BinaryReader br;
        BinaryWriter bw;
        BinaryFormatter formatter;

        UdpClient udpClient;

        public const int DKD_Hello = 1996;
        public const byte DKD_OK = 0;
        public const byte DKD_Connect = 1;
        public const byte DKD_PlayerData = 2;
        public const byte DKD_EntityData = 3;
        public const byte DKD_Register = 4;
        public const byte DKD_Level = 5;
        public const byte DKD_Test = 100;


        public Client(Server s, TcpClient c)
        {
            tcpClient = c;
            server = s;
            (new Thread(new ThreadStart(SetupConnection))).Start();

            udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 42555));
        }

        private void SetupConnection()
        {
            Console.WriteLine("Setting up connection to client...");
            netStream = tcpClient.GetStream();
            ssl = new SslStream(netStream, false);
            //ssl.AuthenticateAsServer(server.cert, false, SslProtocols.Tls, true);

            br = new BinaryReader(netStream, Encoding.UTF8);
            bw = new BinaryWriter(netStream, Encoding.UTF8);

            //Hello greeting.
            bw.Write(DKD_Hello);
            bw.Flush();

            int hello = br.ReadInt32();
            if (hello == DKD_Hello)
            {
                byte message = br.ReadByte();

                if (message == DKD_Register)
                {
                    
                    server.clientIPs.Add((IPEndPoint)tcpClient.Client.RemoteEndPoint);
                    server.clients.Add(this);

                    //TODO: Add player to map.
                    string playerName = br.ReadString();

                    Console.WriteLine("New client registered with IP address: {0}, and Player name: {1}", (IPEndPoint)tcpClient.Client.RemoteEndPoint,playerName);

                    bw.Write(DKD_OK);
                    bw.Flush();
                    Receiver();
                }
            }
        }

        private void Receiver()
        {
            while (tcpClient.Connected)
            {
                byte request = br.ReadByte();
            }
        }

        public void SendLevelOverTCP(byte[] packet)
        {
            // Tells client server is about to send level data.
            bw.Write(DKD_Level);
            // Tells client length of data.
            bw.Write(packet.Length);

            // Sends all data.
            foreach (byte b in packet)
            {
                bw.Write(b);
            }

            bw.Flush();

            // Checks if the client received all information.
            byte ans = br.ReadByte();
            if (ans == DKD_OK)
            {
                //over
            }
        }

        public void SendMessageOverTCP(string message)
        {
            bw.Write(DKD_Test);
            bw.Write(message);
            bw.Flush();

            byte ans = br.ReadByte();
            if (ans == DKD_OK)
            {
                //over
            }

        }

    }
}
