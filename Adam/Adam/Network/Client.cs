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
    class Client
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
        public const byte DKD_MapData = 3;
        public const byte DKD_Register = 4;
        public const byte DKD_Level = 5;


        public Client(Server s, TcpClient c)
        {
            tcpClient = c;
            server = s;
            (new Thread(new ThreadStart(SetupConnection))).Start();

            udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 42555));
        }

        private void SetupConnection()
        {
            netStream = tcpClient.GetStream();
            ssl = new SslStream(netStream, false);
            ssl.AuthenticateAsServer(server.cert, false, SslProtocols.Tls, true);

            br = new BinaryReader(ssl, Encoding.UTF8);
            bw = new BinaryWriter(ssl, Encoding.UTF8);

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

                    bw.Write(DKD_OK);
                    bw.Flush();
                    Receiver();
                }
            }
        }

        private void Receiver()
        {
            try
            {
                while (tcpClient.Connected)
                {
                    byte request = br.ReadByte();
                    
                }
            }
            catch { }
        }

        public void SendCurrentLevel(Level CurrentLevel)
        {
            bw.Write(DKD_Level);
            bw.Write((byte)CurrentLevel);
            bw.Flush();

            byte ans = br.ReadByte();
            if (ans == DKD_OK)
            {
                //over
            }
        }

    }
}
