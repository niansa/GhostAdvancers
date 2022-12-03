using System;
using System.Collections.Generic;
using System.Diagnostics;
using MelonLoader;
using HarmonyLib;
using UnityEngine;
using Donteco;
using MhNetworking.Models;
using Lidgren.Network;
using Donteco.Menu;
using GhostAdvancers;

namespace GhostAdvancers
{
    namespace Logging
    {
        [HarmonyPatch(typeof(NetworkBehaviour), nameof(NetworkBehaviour.RpcAll), new Type[] { typeof(string), typeof(Action<NetBuffer>) })]
        public class RpcAllSenderFilterPatch
        {
            private static void Prefix(string name, Action<NetBuffer> fillArgs)
            {
                Melon<Mod>.Logger.Msg($"Sending RPC {name} to everyone");
            }
        }
        [HarmonyPatch(typeof(NetworkBehaviour), nameof(NetworkBehaviour.RpcOthers), new Type[] { typeof(string), typeof(Action<NetBuffer>) })]
        public class RpcOthersSenderFilterPatch
        {
            private static void Prefix(string name, Action<NetBuffer> fillArgs)
            {
                Melon<Mod>.Logger.Msg($"Sending RPC {name} to others");
            }
        }
        [HarmonyPatch(typeof(NetworkBehaviour), nameof(NetworkBehaviour.RpcTarget), new Type[] { typeof(string), typeof(int), typeof(Action<NetBuffer>) })]
        public class RpcTargetSenderFilterPatch
        {
            private static void Prefix(string name, int targetPlayerId, Action<NetBuffer> fillArgs)
            {
                Melon<Mod>.Logger.Msg($"Sending RPC {name} to {targetPlayerId}");
            }
        }
        [HarmonyPatch(typeof(NetworkManager), nameof(NetworkManager.SetServerIpPort), new Type[] { typeof(string), typeof(int) })]
        public class Experiment1Patch
        {
            private static void Prefix(string ip, int port)
            {
                Melon<Mod>.Logger.Msg($"Connecting to {ip}:{port}");
            }
        }
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.ConnectToLobbyGameRoom), new Type[] { typeof(Lobby), typeof(Action) })]
        public class LobbyDebugPatch
        {
            private static void Prefix(Lobby lobby, Action success)
            {
                Melon<Mod>.Logger.Msg($"Lobby ID/Game Room ID: {lobby.Id}:{lobby.Room.GameRoomId}");
            }
        }
    }
}