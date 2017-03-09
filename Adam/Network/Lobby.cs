using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adam.Network
{
    public static class Lobby
    {
        public static ulong LobbyId { get; set; }
        public static bool IsInLobby { get; set; }

        static Callback<LobbyCreated_t> _lobbyCreated;
        static Callback<LobbyEnter_t> _lobbyEntered;
        static Callback<GameLobbyJoinRequested_t> _lobbyJoinRequested;

        public static void Initialize()
        {
            _lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            _lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            _lobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
        }

        private static void OnLobbyJoinRequested(GameLobbyJoinRequested_t result)
        {
            JoinLobby(result.m_steamIDLobby.m_SteamID);
        }

        private static void OnLobbyCreated(LobbyCreated_t result)
        {
            LobbyId = result.m_ulSteamIDLobby;
            IsInLobby = true;
            Console.WriteLine("Created lobby with id: " + LobbyId);
        }

        private static void OnLobbyEntered(LobbyEnter_t result)
        {
            LobbyId = result.m_ulSteamIDLobby;
            IsInLobby = true;
            Console.WriteLine("Entered lobby with id: " + LobbyId);
        }

        public static void CreateLobby()
        {
            SteamAPICall_t result = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
            Console.WriteLine("Requesting new lobby...");
        }

        public static void JoinLobby(ulong id)
        {
            SteamMatchmaking.JoinLobby(new CSteamID(id));
            Console.WriteLine("Attempting to join lobby...");
        }

        public static void InviteFriendToLobby()
        {
            if (IsInLobby)
            {
                SteamFriends.ActivateGameOverlayInviteDialog(new CSteamID(LobbyId));
            }
            else
            {
                AdamGame.MessageBox.Show("You cannot invite friends when you are not in a lobby.");
            }
        }
    }
}
