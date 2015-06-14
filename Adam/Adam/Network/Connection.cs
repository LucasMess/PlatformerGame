using Adam;
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
    class Connection
    {
        TcpClient tcpClient;
        UdpClient udpClient;
        IPEndPoint serverIP;
        NetworkStream netStream;
        SslStream ssl;
        BinaryWriter bw;
        BinaryReader br;

        Level CurrentLevel;

        public const int DKD_Hello = 1996;
        public const byte DKD_OK = 0;
        public const byte DKD_Connect = 1;
        public const byte DKD_PlayerData = 2;
        public const byte DKD_MapData = 3;
        public const byte DKD_Register = 4;
        public const byte DKD_Level = 5;

        public string PlayerName { get; set; }
        public bool IsConnected { get; set; }

        /// <summary>
        /// Sets up a connection with the specified server.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public Connection(string ipAddress, int port, string playerName)
        {
            serverIP = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            PlayerName = playerName;
            new Thread(new ThreadStart(SetupConnection));

            udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 42555));
        }

        private void SetupConnection()
        {
            try
            {
                tcpClient = new TcpClient(serverIP);
                netStream = tcpClient.GetStream();
                ssl = new SslStream(netStream, false, new RemoteCertificateValidationCallback(ValidateCert));
                ssl.AuthenticateAsClient("AdamMultiplayer");

                int hello = br.ReadInt32();
                if (hello == DKD_Hello)
                {
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

            catch
            {
                Console.WriteLine("Could not find server.");
            }
        }

        private void Receiver()
        {
            while (IsConnected)
            {
                byte request = br.ReadByte();
                if (request == DKD_Level)
                {
                    CurrentLevel = (Level)br.ReadByte();
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

        public void SendPlayerDataPacket(PlayerDataPacket p)
        {
            byte[] playerData = CalcHelper.ToByteArray(p);
            udpClient.Send(playerData, playerData.Length, serverIP);
        }

        public MapDataPacket ReceiveMapDataPacket()
        {
            byte[] mapData = udpClient.Receive(ref serverIP);
            MapDataPacket m = (MapDataPacket)CalcHelper.ConvertToObject(mapData);
            return m;
        }

    }
}
