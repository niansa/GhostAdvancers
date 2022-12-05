using System;
using System.Collections.Generic;
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
        static private QuickPlay? quickPlay;

        public string levelName;
        static public LobbyManager? lobbyMan;
        static public Lobby? lobby;
        static public MainMenuStateMachine? menuStateMachine;
        public List<Tool> tools;
        public List<GameObject> players;

        static public Environment? instance;

        public Environment(string _levelName)
        {
            levelName = _levelName;
            tools = new List<Donteco.Tool>();
            players = new List<GameObject>();
            instance = this;
        }
        
        public static void window(int windowID)
        {
            if (instance != null)
            {
                // In-Game UI
                GUILayout.Label($"Current map: {instance!.levelName}");
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
            } else
            {
                // Out-of-game UI
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
            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }
        private static void QuickPlayFoundLobby(Lobby lobby)
        {
            // Stop searching
            quickPlay = null;
            QuickPlay.OnLobbyFound -= QuickPlayFoundLobby;
            // Join lobby
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