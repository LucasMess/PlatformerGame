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
        public static List<CSteamID> OtherPlayers
        {
            get
            {
                return (from player in Lobby.PlayerList
                        where player != AdamGame.SteamID
                        select player).ToList();
            }
        }

        public const int BB_LevelData = 0;
        public const int BB_Ready = 1;
        public const int BB_StartTime = 2;
        public const int BB_TileIdChange = 3;
        public const int BB_AllEntities = 4;
        public const int BB_EntityUpdate = 5;

        public static bool IsWaitingForOtherPlayers { get; set; }

        private static int _playersReadyCount = 0;
        private static bool _hasStartTime = false;
        private static DateTime _startTime;

        /// <summary>
        /// Returns true if the current client is hosting the multiplayer server. The user is always hosting their single player games.
        /// </summary>
        public static bool IsHost { get; set; } = true;

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
                IsWaitingForOtherPlayers = true;
                IsActive = true;

                //if (Clients.Count == 0)
                //{
                //    AdamGame.MessageBox.Show("There is no one in this lobby :(");
                //    return;
                //}

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

        private static void ReceivePackets(int channel = -1)
        {
            int i = 0;
            int max = 10;
            if (channel != -1)
            {
                i = channel;
                max = channel + 1;
            }
            for (i = 0; i < max; i++)
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
                            case BB_Ready:
                                _playersReadyCount++;
                                break;
                            case BB_StartTime:
                                _startTime = new DateTime((long)CalcHelper.ConvertToObject(pubDest));
                                break;
                            case BB_TileIdChange:
                                Packet.TileIdChange packet = (Packet.TileIdChange)CalcHelper.ConvertToObject(pubDest);
                                LevelEditor.UpdateTileFromP2P(packet);
                                break;
                            case BB_AllEntities:
                                UpdateEntities(pubDest);
                                break;
                            case BB_EntityUpdate:
                                UpdateEntities(pubDest);
                                break;
                        }

                        // Relay message to others.
                        if (IsHost)
                        {
                            var allButSender = (from client in OtherPlayers
                                                where client != steamIdRemote
                                                select client).ToList();
                            Send(pubDest, EP2PSend.k_EP2PSendReliable, i, allButSender);
                        }
                    }


                }
            }
        }

        public static void SendEntityUpdates()
        {
            List<EntityPacket> packets = new List<EntityPacket>();
            foreach (var entity in GameWorld.GetAllEntities())
            {
                EntityPacket packet = new EntityPacket(entity.Id, entity);
                packets.Add(packet);
            }
            byte[] data = CalcHelper.ToByteArray(packets.ToArray());
            Send(data, EP2PSend.k_EP2PSendReliable, BB_EntityUpdate);
        }

        private static void UpdateEntities(byte[] data)
        {
            LoadingScreen.LoadingText = "Receiving entity data";

            EntityPacket[] packets = (EntityPacket[])CalcHelper.ConvertToObject(data);
            foreach (var packet in packets)
            {
                packet.UpdateEntityInGameWorld();
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
                clients = OtherPlayers;
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

        public static void WaitForPlayers()
        {
            Send(BitConverter.GetBytes(true), EP2PSend.k_EP2PSendReliable, BB_Ready);

            while (_playersReadyCount < OtherPlayers.Count)
            {
                LoadingScreen.LoadingText = "Waiting for other players...";
                ReceivePackets(BB_Ready);
            }

            if (IsHost)
            {
                long time = DateTime.UtcNow.Ticks + 5 * 1000000;
                _hasStartTime = true;
                _startTime = new DateTime(time);
                Send(CalcHelper.ToByteArray(time), EP2PSend.k_EP2PSendReliable, BB_StartTime);
            }
            else
            {
                while (!_hasStartTime)
                {
                    ReceivePackets(BB_StartTime);
                }
            }

            while (DateTime.UtcNow.Ticks < _startTime.Ticks)
            {
                LoadingScreen.LoadingText = "Starting game...";
            }
        }

    }
}
