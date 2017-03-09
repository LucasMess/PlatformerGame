using Adam.GameData;
using Adam.Levels;
using Adam.Network.Packets;
using Adam.UI;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            Lobby.Initialize();
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
            IsHost = true;
        }

        public static void Join(CSteamID hostId)
        {
            if (IsHost) return;
            SteamNetworking.CreateP2PConnectionSocket(hostId, 42577, 1000, true);
            Console.WriteLine("Creating P2P connection...");
            HostUser = hostId;
            IsHost = false;
        }

        public static void Start()
        {
            if (IsHost)
            {
                IsActive = true;
                Clients = (from client in Lobby.PlayerList
                           where client != AdamGame.SteamID
                           select client).ToList();

                if (Clients.Count == 0)
                {
                    AdamGame.MessageBox.Show("There is no one in this lobby :(");
                    return;
                }

                SendLevel();
            }
            else
            {
                AdamGame.MessageBox.Show("You cannot start a game you do not own, silly boy.");
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
                                IsActive = true;
                                WorldConfigFile config = (WorldConfigFile)CalcHelper.ConvertToObject(pubDest);
                                config.LoadIntoEditor();
                                break;
                            case BB_TileIdChange:
                                Packet.TileIdChange packet = (Packet.TileIdChange)CalcHelper.ConvertToObject(pubDest);
                                LevelEditor.UpdateTileFromP2P(packet);
                                break;
                        }

                        // Relay message to others.
                        if (IsHost)
                        {
                            var allButSender = (from client in Clients
                                                where client != steamIdRemote
                                                select client).ToList();
                            Send(pubDest, EP2PSend.k_EP2PSendReliable, i, allButSender);
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

            Send(levelData, EP2PSend.k_EP2PSendReliable, BB_LevelData, null);

            config.LoadIntoEditor();
        }


        public static void Send(byte[] data, EP2PSend type, int channel, List<CSteamID> clients = null)
        {
            if (clients == null)
                clients = Clients;
            if (IsHost)
            {
                foreach (var client in clients)
                {
                    SteamNetworking.SendP2PPacket(client, data, (uint)data.Length, type, channel);
                }
            }
            else
            {
                SteamNetworking.SendP2PPacket(HostUser, data, (uint)data.Length, type, channel);
            }
        }

    }
}
