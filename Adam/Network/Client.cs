using Adam.Network.Packets;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace Adam.Network
{
    public class Client
    {
        Server _server;
        TcpClient _tcpClient;
        NetworkStream _netStream;
        SslStream _ssl;
        BinaryReader _br;
        BinaryWriter _bw;
        BinaryFormatter _formatter;

        UdpClient _udpClient;

        public PlayerPacket PlayerPacket { get; set; }

        public const int DkdHello = 1996;
        public const byte DkdOk = 0;
        public const byte DkdConnect = 1;
        public const byte DkdPlayerData = 2;
        public const byte DkdEntityData = 3;
        public const byte DkdRegister = 4;
        public const byte DkdLevel = 5;
        public const byte DkdTest = 100;


        public Client(Server s, TcpClient c)
        {
            _tcpClient = c;
            _server = s;
            (new Thread(new ThreadStart(SetupConnection))).Start();
           // (new Thread(new ThreadStart(UpdateReceiver))).Start();

            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 42556));
        }

        private void SetupConnection()
        {
            Console.WriteLine("Setting up connection to client...");
            _netStream = _tcpClient.GetStream();
            _ssl = new SslStream(_netStream, false);
            //ssl.AuthenticateAsServer(server.cert, false, SslProtocols.Tls, true);

            _br = new BinaryReader(_netStream, Encoding.UTF8);
            _bw = new BinaryWriter(_netStream, Encoding.UTF8);

            //Hello greeting.
            _bw.Write(DkdHello);
            _bw.Flush();

            int hello = _br.ReadInt32();
            if (hello == DkdHello)
            {
                byte message = _br.ReadByte();

                if (message == DkdRegister)
                {
                    
                    _server.ClientIPs.Add((IPEndPoint)_tcpClient.Client.RemoteEndPoint);
                    _server.Clients.Add(this);

                    //TODO: Add player to map.
                    string playerName = _br.ReadString();

                    Console.WriteLine("New client registered with IP address: {0}, and Player name: {1}", (IPEndPoint)_tcpClient.Client.RemoteEndPoint,playerName);

                    _bw.Write(DkdOk);
                    _bw.Flush();
                    Receiver();
                }
            }
        }

        private void Receiver()
        {
            while (_tcpClient.Connected)
            {
                byte request = _br.ReadByte();
            }
        }

        //private void UpdateReceiver()
        //{
        //    Console.WriteLine("Listening for player packets...");
        //    while (Session.IsActive)
        //    {
        //        IPEndPoint ip = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
        //        byte[] packet = udpClient.Receive(ref ip);
        //        Console.WriteLine("Player packet received.");
        //        PlayerPacket = (PlayerPacket)CalcHelper.ConvertToObject(packet); 
        //    }
        //}

        public void SendLevelOverTcp(byte[] packet)
        {
            // Tells client server is about to send level data.
            _bw.Write(DkdLevel);
            // Tells client length of data.
            _bw.Write(packet.Length);
            Console.WriteLine("Sending level over TCP to client...");
            // Sends all data.
            _bw.Write(packet);

            _bw.Flush();

            // Checks if the client received all information.
            //byte ans = br.ReadByte();
            //if (ans == DKD_OK)
            //{
            //    Console.WriteLine("Level sent sucessfully.");
            //    //over
            //}
            //else
            //{
            //    Console.WriteLine("Error occurred.");
            //}
        }

        public void SendMessageOverTcp(string message)
        {
            _bw.Write(DkdTest);
            _bw.Write(message);
            _bw.Flush();

            byte ans = _br.ReadByte();
            if (ans == DkdOk)
            {
                //over
            }

        }

    }
}
