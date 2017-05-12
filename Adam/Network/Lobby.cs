using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThereMustBeAnotherWay.Network
{
    public static class Lobby
    {
        public static ulong LobbyId { get; set; }
        public static bool IsInLobby { get; set; }

        public static int UserCount => PlayerList.Count;

        static Callback<LobbyCreated_t> _lobbyCreated;
        static Callback<LobbyEnter_t> _lobbyEntered;
        static Callback<GameLobbyJoinRequested_t> _lobbyJoinRequested;
        static Callback<LobbyChatUpdate_t> _lobbyUpdated;

        public static List<CSteamID> PlayerList = new List<CSteamID>();
        private static List<string> _playerNames = new List<string>();

        static List<TextButton> _buttons = new List<TextButton>();

        public static void Initialize()
        {
            _lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            _lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            _lobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
            _lobbyUpdated = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);

            TextButton inviteFriend = new TextButton(new Vector2(0, 0), "Invite Friend", false);
            TextButton startGame = new TextButton(new Vector2(0, 0), "Start Game", false);

            TextButton backButton = new TextButton(new Vector2(0, 0), "Exit Lobby", false);

            inviteFriend.MouseClicked += InviteFriend_MouseClicked;
            startGame.MouseClicked += StartGame_MouseClicked;
            backButton.MouseClicked += BackButton_MouseClicked;

            _buttons.Add(inviteFriend);
            _buttons.Add(startGame);
            _buttons.Add(backButton);

            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].SetPosition(new Vector2(10, 10 + i * TextButton.Height * 2));
                _buttons[i].ChangeDimensions(new Vector2(TextButton.Width * 2, TextButton.Height * 2));
                _buttons[i].Color = new Color(196, 69, 69);
            }

        }

        private static void BackButton_MouseClicked(UI.Button button)
        {
            MainMenu.CurrentMenuState = MainMenu.MenuState.Main;
        }

        private static void OnLobbyChatUpdate(LobbyChatUpdate_t result)
        {
            Console.WriteLine("Lobby chat update");
            int lobbyMemberCount = SteamMatchmaking.GetNumLobbyMembers(new CSteamID(LobbyId));
            PlayerList.Clear();
            _playerNames.Clear();
            for (int i = 0; i < lobbyMemberCount; i++)
            {
                PlayerList.Add(SteamMatchmaking.GetLobbyMemberByIndex(new CSteamID(LobbyId), i));
                _playerNames.Add(SteamFriends.GetFriendPersonaName(PlayerList[i]));
            }
        }

        private static void StartGame_MouseClicked(UI.Button button)
        {
            Session.Start();
        }

        private static void InviteFriend_MouseClicked(UI.Button button)
        {
            InviteFriendToLobby();
        }

        private static void OnLobbyJoinRequested(GameLobbyJoinRequested_t result)
        {
            JoinLobby(result.m_steamIDLobby.m_SteamID);
        }

        private static void OnLobbyCreated(LobbyCreated_t result)
        {
            Session.IsHost = true;
            LobbyId = result.m_ulSteamIDLobby;
            IsInLobby = true;
            Session.HostGame();
            Console.WriteLine("Created lobby with id: " + LobbyId);
        }

        private static void OnLobbyEntered(LobbyEnter_t result)
        {
            LobbyId = result.m_ulSteamIDLobby;
            IsInLobby = true;
            Session.Join(SteamMatchmaking.GetLobbyOwner(new CSteamID(LobbyId)));
            if (TMBAW_Game.CurrentGameState != GameState.MainMenu)
            {
                TMBAW_Game.ChangeState(GameState.MainMenu, GameMode.None, true);
            }
            MainMenu.CurrentMenuState = MainMenu.MenuState.MultiplayerLobby;
            Console.WriteLine("Entered lobby with id: " + LobbyId);
            OnLobbyChatUpdate(new LobbyChatUpdate_t());
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
                TMBAW_Game.MessageBox.Show("You cannot invite friends when you are not in a lobby.");
            }
        }

        public static void Update()
        {
            foreach (var button in _buttons)
            {
                button.Update();
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var button in _buttons)
            {
                button.Draw(spriteBatch);
            }

            string text = "Number of players in lobby: " + UserCount;
            FontHelper.DrawWithOutline(spriteBatch, FontHelper.Fonts[1], text, new Vector2(TMBAW_Game.DefaultUiWidth/2 - FontHelper.Fonts[1].MeasureString(text).X/2, 0), 1, Color.White, Color.Black);

            for (int i = 0; i < _playerNames.Count; i++)
            {
                FontHelper.DrawWithOutline(spriteBatch, FontHelper.Fonts[1], _playerNames[i], new Vector2(20, 300 + i * 20), 1, Color.White, Color.Black);
            }
        }
    }
}
