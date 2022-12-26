/*
    GhostAdvancers (Useful mods for Ghost Watchers)
    Copyright (C) 2022  niansa/Tuxifan

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MelonLoader;
using HarmonyLib;
using UnityEngine;
using Donteco;
using GhostAdvancers;
using Donteco.Menu;

namespace GhostAdvancers
{
    class Environment
    {
        static public double inGameTipDuration = 7.5;

        static private QuickPlay? quickPlay;

        public string levelName;
        static public LobbyManager? lobbyMan;
        static public Lobby? lobby;
        static public MainMenuStateMachine? menuStateMachine;
        static public bool inGameMenuVisible = false;
        static public bool debugUI = false;
        static private Stopwatch inGameMenuVisibleCooldownTimer;
        public Stopwatch roundDuration;
        public List<Tool> tools;
        public List<GameObject> players;

        static public Environment? instance; // Some of the data is only available through this instance which exists only in-game

        static Environment()
        {
            inGameMenuVisibleCooldownTimer = new Stopwatch();
            inGameMenuVisibleCooldownTimer.Start();
        }
        public Environment(string _levelName)
        {
            levelName = _levelName;
            tools = new List<Donteco.Tool>();
            players = new List<GameObject>();
            roundDuration = new Stopwatch();
            instance = this;
            Popup.Show("Ghost Advancers Tip", "Press <b>F3</b> to show in-game UI and press <b>Tab</b> or <b>ESC</b> to use your mouse", inGameTipDuration);
            Popup.SetSizePresetLongLine();
        }
        
        public static void RunGUI()
        {
            if (instance != null)
            {
                // In-game UI toggle
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    if (inGameMenuVisibleCooldownTimer.Elapsed.TotalMilliseconds > 250)
                    {
                        inGameMenuVisible = !inGameMenuVisible;
                        inGameMenuVisibleCooldownTimer.Restart();
                    }
                }
                // In-Game UI
                if (inGameMenuVisible)
                {
                    GUI.Window(0, new Rect(20, 20, 400, 220), InGameWindow, "Ghost Advancers");
                }
            }
            else if (menuStateMachine != null)
            {
                if (menuStateMachine.GetState<MainState>() == menuStateMachine.Current)
                {
                    // Main menu UI
                    GUI.Window(0, new Rect(150, 20, 200, 100), MainMenuGameWindow, "Ghost Advancers");
                }
                else quickPlay = null;
            }
        }
        private static void InGameWindow(int windowID)
        {
            debugUI = GUILayout.Toggle(debugUI, "Debug UI");
            GUILayout.Label($"Current map: {instance!.levelName}");
            if (debugUI)
            {
                GUILayout.Label($"Player ID: {GameData.Id}");
                if (lobby != null)
                {
                    GUILayout.Label($"Lobby ID: {lobby.Id}");
                    if (lobby.Room != null)
                    {
                        GUILayout.Label($"Room ID: {lobby.Room.GameRoomId}");
                        GUILayout.Label($"Server: {lobby.Room.ServerIp}:{lobby.Room.ServerPort}");
                        string playerList = new string("");
                        foreach (var player in lobby.Players)
                        {
                            playerList += $"{player.Value.Id}: {player.Value.Nickname}\n";
                        }
                        GUILayout.Box(playerList);
                    }
                }
            }
        }
        private static void MainMenuGameWindow(int windowID)
        {
            if (lobbyMan != null && menuStateMachine != null)
            {
                if (quickPlay == null)
                {
                    if (GUILayout.Button("Quick Play"))
                    {
                        QuickPlay.OnLobbyFound += QuickPlayFoundLobby;
                        quickPlay = new QuickPlay();
                    }
                }
                else
                {
                    GUILayout.Label("Quick Play is searching...");
                    quickPlay.RunUpdate(); // Probably shouldn't do this inside of UI but whatever
                }
            }
        }
        private static void QuickPlayFoundLobby(Lobby lobby)
        {
            // Stop searching
            quickPlay = null;
            QuickPlay.OnLobbyFound -= QuickPlayFoundLobby;
            // Join lobby
            Melon<Mod>.Logger.Msg("Joining lobby...");
            lobbyMan.ConnectLobbyViaCode(lobby.Settings.Settings["code"]);
            lobbyMan.StartStateUpdating();
            menuStateMachine.SwitchState<PublicOrPrivateGameState>();
        }
    }

    [HarmonyPatch(typeof(Tool), "Start", new Type[] {  })]
    public class ToolListManAddPatch
    {
        private static void Prefix(Tool __instance)
        {
            var env = Environment.instance;
            if (env != null && !env.tools.Contains(__instance))
            {
                env.tools.Add(__instance);
            }
        }
    }
    [HarmonyPatch(typeof(PlayerMovementSync), "Awake", new Type[] { })]
    public class PlayerListManAddPatch
    {
        private static void Prefix(PlayerMovementSync __instance)
        {
            if (Environment.instance != null)
            {
                Environment.instance.players.Add(__instance.gameObject);
            }
        }
    }
    [HarmonyPatch(typeof(MainMenuStateMachine), "Start", new Type[] { })]
    public class MainMenuStateMachineSetPatch
    {
        private static void Prefix(MainMenuStateMachine __instance)
        {
            Environment.menuStateMachine = __instance;
        }
    }
    [HarmonyPatch(typeof(LobbyManager), "Awake", new Type[] { })]
    public class LobbyManSetPatch
    {
        private static void Prefix(LobbyManager __instance)
        {
            Environment.lobbyMan = __instance;
        }
    }
    [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.ConnectToLobbyGameRoom), new Type[] { typeof(Lobby), typeof(Action) })]
    public class LobbySetPatch
    {
        private static void Prefix(Lobby lobby, Action success)
        {
            Environment.lobby = lobby;
        }
    }
}