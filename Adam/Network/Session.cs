using Adam.GameData;
using Adam.Levels;
using Adam.Network.Packets;
using Adam.UI;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace Adam.Network
{
    public static class Session
    {
        static Callback<P2PSessionRequest_t> _sessionRequest;
        static Callback<P2PSessionConnectFail_t> _sessionConnectFail;

        public static CSteamID HostUser;
        public static List<CSteamID> Clients;

        public const int BB_LevelData = 0;
        public const int BB_TileIdChange = 1;

        /// <summary>
        /// Returns true if the current client is hosting the multiplayer server.
        /// </summary>
        public static bool IsHost
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

        public static void Initialize()
        {
            _sessionRequest = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequested);
            _sessionConnectFail = Callback<P2PSessionConnectFail_t>.Create(OnP2PSessionConnectFail);
        }

        private static void OnP2PSessionConnectFail(P2PSessionConnectFail_t callback)
        {
            AdamGame.MessageBox.Show("There was an error with the connection.");
            AdamGame.ChangeState(GameState.MainMenu, GameMode.None, true);
        }

        private static void OnP2PSessionRequested(P2PSessionRequest_t callback)
        {
            Console.WriteLine("Session requested.");
            SteamNetworking.AcceptP2PSessionWithUser(callback.m_steamIDRemote);
            HostUser = callback.m_steamIDRemote;
        }

        public static void HostGame()
        {
            SteamNetworking.CreateP2PConnectionSocket(AdamGame.SteamID, 42577, 1000, true);
            Console.WriteLine("Creating P2P connection...");
            HostUser = AdamGame.SteamID;
            IsHost = true;
        }

        public static void Join()
        {
            SteamNetworking.CreateP2PConnectionSocket(AdamGame.SteamID, 42577, 1000, true);
            Console.WriteLine("Creating P2P connection...");
            HostUser = AdamGame.SteamID;
            IsHost = false;
        }

        public static void Start()
        {
            if (IsHost)
            {
                SendLevel();
            }
        }
        public static void Update()
        {
            ReceivePackets();
        }

        private static void ReceivePackets()
        {
            for (int i = 0; i < 10; i++)
            {
                uint messageSize;
                while (SteamNetworking.IsP2PPacketAvailable(out messageSize, i))
                {
                    CSteamID steamIdRemote;
                    Console.WriteLine("Packet available of size: " + messageSize);
                    byte[] pubDest = new byte[messageSize];
                    uint bytesRead = 0;
                    if (SteamNetworking.ReadP2PPacket(pubDest, messageSize, out bytesRead, out steamIdRemote, i))
                    {
                        switch (i)
                        {
                            case BB_LevelData:
                                if (!IsHost)
                                {
                                    WorldConfigFile config = (WorldConfigFile)CalcHelper.ConvertToObject(pubDest);
                                    config.LoadIntoEditor();
                                }
                                break;
                            case BB_TileIdChange:
                                Packet.TileIdChange packet = (Packet.TileIdChange)CalcHelper.ConvertToObject(pubDest);
                                LevelEditor.UpdateTileFromP2P(packet);
                                break;
                        }
                    }


                }
            }
        }

        public static void SendLevel()
        {
            DataFolder.DeleteLevel(DataFolder.LevelDirectory + "/temp.lvl");
            string filePath = DataFolder.CreateNewLevel("temp", 256, 256);

            WorldConfigFile config = DataFolder.GetWorldConfigFile(filePath);

            byte[] levelData = CalcHelper.ToByteArray(config);
            SteamNetworking.SendP2PPacket(HostUser, levelData, (uint)levelData.Length, EP2PSend.k_EP2PSendReliable, BB_LevelData);

            config.LoadIntoEditor();
        }
    }
}
