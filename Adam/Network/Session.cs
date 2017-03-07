﻿using Adam.GameData;
using Adam.Network.Packets;
using Adam.UI;
using Steamworks;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Adam.Network
{
    public static class Session
    {
        static Server _server;
        static Connection _connection;
        static IPEndPoint _serverIp;

        static string _playerName;

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

        static Callback<P2PSessionRequest_t> _sessionRequest;
        static Callback<P2PSessionConnectFail_t> _sessionConnectFail;

        const int Channel_LevelData = 0;
        const int Channel_Entities = 1;

        public static void Initialize()
        {
            _sessionRequest = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequested);

        }

        private static void OnP2PSessionRequested(P2PSessionRequest_t callback)
        {
            Console.WriteLine("Session requested.");
            SteamNetworking.AcceptP2PSessionWithUser(callback.m_steamIDRemote);
        }

        public static void CreateNew()
        {

            SteamNetworking.CreateP2PConnectionSocket(AdamGame.SteamID, 42577, 1000, true);
            Console.WriteLine("Creating P2P connection...");
            remoteUser = AdamGame.SteamID;
            IsHost = true;

            //IsActive = true;
            //Console.WriteLine("New session started. Is host? {0}, Player Name: {1}", isHost, playerName);
            //_playerName = playerName;
            //IsHost = isHost;

            //if (isHost)
            //{
            //    //Server automatically starts listening for players.
            //    _server = new Server();

            //    //Automatically connects to own server.
            //    //ConnectTo("192.168.1.1", 42555);
            //}
            //else
            //{
            //    ConnectTo("127.0.0.1", 42555);
            //}

        }

        public static void Join()
        {
            SteamNetworking.CreateP2PConnectionSocket(AdamGame.SteamID, 42577, 1000, true);
            Console.WriteLine("Creating P2P connection...");
            remoteUser = AdamGame.SteamID;
            IsHost = false;
        }

        public static void ConnectTo(string address, int port)
        {
            //Connection automatically sets up a connection with server.
            _connection = new Connection(address, port, _playerName);
        }

        public static void Start()
        {
            if (IsHost)
            {
              //  _server.IsWaitingForPlayers = false;
                SendLevel();
            }
            //new Thread(new ThreadStart(SendPackets)).Start();
            //new Thread(new ThreadStart(ReceivePackets)).Start();
        }

        static CSteamID remoteUser;

        public static void Update()
        {
            ReceivePackets();
        }

        private static void SendPackets()
        {

        }

        private static void ReceivePackets()
        {
            uint messageSize;
            CSteamID steamIdRemote;
            while (SteamNetworking.IsP2PPacketAvailable(out messageSize))
            {
                Console.WriteLine("Packet available of size: " + messageSize);
                byte[] pubDest = new byte[messageSize];
                uint bytesRead = 0;
                if (!IsHost)
                {
                    if (SteamNetworking.ReadP2PPacket(pubDest, messageSize, out bytesRead, out steamIdRemote, Channel_LevelData))
                    {
                        WorldConfigFile config = (WorldConfigFile)CalcHelper.ConvertToObject(pubDest);
                        config.LoadIntoEditor();
                    }
                }
            }


            //while (IsActive)
            //{
            //    if (AdamGame.IsLoadingContent)
            //        continue;
            //    Console.WriteLine("Updating...");
            //    if (IsHost)
            //    {
            //        _server.SendEntityPacket();
            //        Console.WriteLine("Entity packet sent.");
            //        //PlayerPacket = connection.ReceivePlayerPacket();
            //    }
            //    else
            //    {
            //        EntityPacket = _connection.ReceiveEntityPacket();
            //        Console.WriteLine("Entity packet received.");
            //    }
            //}
        }

        public static void SendTestMessage()
        {
            _server.SendMessage("Hello World!");
        }

        public static void SendLevel()
        {
            DataFolder.DeleteLevel(DataFolder.LevelDirectory + "/temp.lvl");
            string filePath = DataFolder.CreateNewLevel("temp", 256, 256);

            WorldConfigFile config = DataFolder.GetWorldConfigFile(filePath);

            byte[] levelData = CalcHelper.ToByteArray(config);
            SteamNetworking.SendP2PPacket(remoteUser, levelData, (uint)levelData.Length, EP2PSend.k_EP2PSendReliable);

            config.LoadIntoEditor();


            //_server.SendLevelPacket(level);


            //level.LoadIntoPlay();
            // server.SendLevelPacket(new WorldConfigFile("Test",256, 256));
        }
    }
}
