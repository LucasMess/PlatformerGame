using Adam;
using Adam.Network.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Adam.Network
{
    public class Connection
    {
        TcpClient tcpClient;
        UdpClient udpClient;
        IPEndPoint serverIP;
        NetworkStream netStream;
        SslStream ssl;
        BinaryWriter bw;
        BinaryReader br;

        GameMode CurrentLevel;

        public const int DKD_Hello = 1996;
        public const byte DKD_OK = 0;
        public const byte DKD_Connect = 1;
        public const byte DKD_PlayerData = 2;
        public const byte DKD_MapData = 3;
        public const byte DKD_Register = 4;
        public const byte DKD_Level = 5;
        public const byte DKD_Test = 100;

        public string PlayerName { get; set; }
        public bool IsConnected { get; set; }

        /// <summary>
        /// Sets up a connection with the specified server.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public Connection(string ipAddress, int port, string playerName)
        {
            Console.WriteLine("Trying to connect to server...");
            serverIP = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            Console.WriteLine("Server IP: {0}, Player name: {1}", serverIP, playerName);
            PlayerName = playerName;

            udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 42556));
            Console.WriteLine("UDP client set up.");

            new Thread(new ThreadStart(SetupConnection)).Start();
            //SetupConnection();
        }

        private void SetupConnection()
        {
            Console.WriteLine("Setting up TCP client...");
            tcpClient = new TcpClient();
            try {
                tcpClient.Connect(serverIP);
            }
            catch(SocketException e)
            {
                Console.WriteLine("Could not connect to server. Error code: {0}",e.ErrorCode);
                return;
            }
            Console.WriteLine("TCP client set up.");
            netStream = tcpClient.GetStream();
            Console.WriteLine("Network stream found.");
            ssl = new SslStream(netStream, false);
            Console.WriteLine("SSL stream created.");
            //ssl.AuthenticateAsClient("AdamMultiplayer");
            Console.WriteLine("Authenticated as client.");

            br = new BinaryReader(netStream, Encoding.UTF8);
            bw = new BinaryWriter(netStream, Encoding.UTF8);

            int hello = br.ReadInt32();
            if (hello == DKD_Hello)
            {
                Console.WriteLine("Sending player info to server...");

                bw.Write(DKD_Hello);
                bw.Flush();

                bw.Write(DKD_Register);
                bw.Write(PlayerName);
                bw.Flush();

                byte ans = br.ReadByte();
                if (ans == DKD_OK)
                {
                    IsConnected = true;
                    Console.WriteLine("Connected with the server.");
                    Receiver();
                }
                else
                {
                    //TODO: Throw error message. Connection rejected.
                }
            }
        }

        private void Receiver()
        {
            while (Session.IsActive)
            {
                byte request = br.ReadByte();
                if (request == DKD_Level)
                {
                    CurrentLevel = (GameMode)br.ReadByte();
                    bw.Write(DKD_OK);
                    bw.Flush();
                }
                if (request == DKD_Test)
                {
                    Main.MessageBox.Show(br.ReadString());
                    bw.Write(DKD_OK);
                    bw.Flush();
                }
            }
        }

        public static bool ValidateCert(object sender, X509Certificate certificate,
           X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true; //Allow untrusted certificates.
        }

        public void SendPlayerDataPacket(Player player)
        {
            PlayerPacket pl = new PlayerPacket(player);
            byte[] packet = CalcHelper.ToByteArray(pl);
            udpClient.Send(packet, packet.Length, serverIP);
        }

        public EntityPacket ReceiveEntityPacket()
        {
            byte[] packet = udpClient.Receive(ref serverIP);
            EntityPacket en = (EntityPacket)CalcHelper.ConvertToObject(packet);
            return en;
        }

    }
}
