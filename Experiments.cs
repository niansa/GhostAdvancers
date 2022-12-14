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
#if DEBUG
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
#endif