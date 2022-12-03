using System;
using System.Collections.Generic;
using MelonLoader;
using HarmonyLib;
using UnityEngine;
using Donteco;
using GhostAdvancers;

namespace GhostAdvancers
{
    class Environment
    {
        public string levelName;
        static public Lobby? lobby;
        public List<Donteco.Tool> tools;

        static public Environment? instance;

        public Environment(string _levelName)
        {
            levelName = _levelName;
            tools = new List<Donteco.Tool>();
            instance = this;
        }
        
        public static void window(int windowID)
        {
            GUILayout.Label($"Current map: {instance!.levelName}");
            GUILayout.Label($"Player ID: {GameData.Id}");
            if (lobby != null)
            {
                GUILayout.Label($"Lobby ID: {lobby.Id}");
                if (lobby.Room != null)
                {
                    GUILayout.Label($"Room ID: {lobby.Room.GameRoomId}");
                }
            }
            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
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
    [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.ConnectToLobbyGameRoom), new Type[] { typeof(Lobby), typeof(Action) })]
    public class LobbyManSetPatch
    {
        private static void Prefix(Lobby lobby, Action success)
        {
            Environment.lobby = lobby;
        }
    }
}