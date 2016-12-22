using Adam.Network.Packets;
using Adam.PlayerCharacter;
using System;
using System.IO;
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
        TcpClient _tcpClient;
        UdpClient _udpClient;
        IPEndPoint _serverIp;
        NetworkStream _netStream;
        SslStream _ssl;
        BinaryWriter _bw;
        BinaryReader _br;

        GameMode _currentLevel;

        public const int DkdHello = 1996;
        public const byte DkdOk = 0;
        public const byte DkdConnect = 1;
        public const byte DkdPlayerData = 2;
        public const byte DkdMapData = 3;
        public const byte DkdRegister = 4;
        public const byte DkdLevel = 5;
        public const byte DkdTest = 100;

        public string PlayerName { get; set; }
        public bool IsConnected { get; set; }

        IPEndPoint _udpIp;
        IPEndPoint _server;

        /// <summary>
        /// Sets up a connection with the specified server.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public Connection(string ipAddress, int port, string playerName)
        {
            Console.WriteLine("Trying to connect to server...");
            _serverIp = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            Console.WriteLine("Server IP: {0}, Player name: {1}", _serverIp, playerName);
            PlayerName = playerName;

            _udpIp = new IPEndPoint(_serverIp.Address, 42559);
            _server = new IPEndPoint(_serverIp.Address, 42557);

            _udpClient = new UdpClient(_udpIp);
            Console.WriteLine("UDP client set up.");

            new Thread(new ThreadStart(SetupConnection)).Start();
            //SetupConnection();
        }

        private void SetupConnection()
        {
            Console.WriteLine("Setting up TCP client...");
            _tcpClient = new TcpClient();
            try {
                _tcpClient.Connect(_serverIp);
            }
            catch(SocketException e)
            {
                Console.WriteLine("Could not connect to server. Error code: {0}",e.ErrorCode);
                return;
            }
            Console.WriteLine("TCP client set up.");
            _netStream = _tcpClient.GetStream();
            Console.WriteLine("Network stream found.");
            _ssl = new SslStream(_netStream, false);
            Console.WriteLine("SSL stream created.");
            //ssl.AuthenticateAsClient("AdamMultiplayer");
            Console.WriteLine("Authenticated as client.");

            _br = new BinaryReader(_netStream, Encoding.UTF8);
            _bw = new BinaryWriter(_netStream, Encoding.UTF8);

            int hello = _br.ReadInt32();
            if (hello == DkdHello)
            {
                Console.WriteLine("Sending player info to server...");

                _bw.Write(DkdHello);
                _bw.Flush();

                _bw.Write(DkdRegister);
                _bw.Write(PlayerName);
                _bw.Flush();

                byte ans = _br.ReadByte();
                if (ans == DkdOk)
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
                byte request = _br.ReadByte();
                if (request == DkdLevel)
                {
                    // Gets the amount of bytes the level was split into.
                    int size = _br.ReadInt32();
                    byte[] data = _br.ReadBytes(size);
                    LevelPacket packet = (LevelPacket)CalcHelper.ConvertToObject(data);
                    //bw.Write(DKD_OK);
                    //bw.Flush();

                    packet.ExtractConfigFile().LoadIntoPlay();
                    AdamGame.Session.Start();
                }
                if (request == DkdTest)
                {
                    AdamGame.MessageBox.Show(_br.ReadString());
                    _bw.Write(DkdOk);
                    _bw.Flush();
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
            _udpClient.Send(packet, packet.Length, _serverIp);
        }

        public EntityPacket ReceiveEntityPacket()
        {
            Console.WriteLine("Listening at: {0}, for server: {1}", _udpIp, _server);
            byte[] packet = _udpClient.Receive(ref _server);
            Console.WriteLine("Received entity packet:" + packet);
            EntityPacket en = (EntityPacket)CalcHelper.ConvertToObject(packet);
            return en;
        }

        public PlayerPacket ReceivePlayerPacket(IPEndPoint clientIpEndPoint)
        {
            byte[] packet = _udpClient.Receive(ref clientIpEndPoint);
            PlayerPacket en = (PlayerPacket)CalcHelper.ConvertToObject(packet);
            return en;
        }

    }
}
