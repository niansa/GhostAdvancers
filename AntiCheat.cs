using System;
using MelonLoader;
using HarmonyLib;
using Donteco;
using UnityEngine;
using Lidgren.Network;
using System.Diagnostics;
using System.Collections.Generic;
using GhostAdvancers;

namespace GhostAdvancers
{
    class AntiCheat
    {
        public static double interval = 1;
        public static double overspeedTolerance = 1.1;
        private static Stopwatch timer;
        private static Dictionary<string, Vector3> lastKnownPositions;

        static AntiCheat() {
            timer = new Stopwatch();
            timer.Start();
            lastKnownPositions = new Dictionary<string, Vector3>();
        }
        public static void RunChecks()
        {
            if (Environment.instance == null)
            {
                if (lastKnownPositions.Count != 0) Melon<Mod>.Logger.Msg("Reset last known player positions");
                // Clean up last known positions if there is no instance
                lastKnownPositions.Clear();
                return;
            }
            // Check if it's time to detect speedhacks
            double delta = timer.Elapsed.Milliseconds/1000f;
            if (delta >= interval)
            {
                // Interate over all players
                foreach (var player in Environment.instance.players)
                {
                    var playerId = player.GetComponent<PlayerSetup>().SteamId.ToString();
                    // Add player to last known positionsif it doesn't exist already
                    if (!lastKnownPositions.ContainsKey(playerId))
                    {
                        lastKnownPositions[playerId] = player.transform.position;
                        Melon<Mod>.Logger.Msg($"Registered player {playerId}");
                    } else
                    {
                        // Get distance between last known and current player positions
                        var lastPos = lastKnownPositions[playerId];
                        var newPos = player.transform.position;
                        lastKnownPositions[playerId] = newPos;
                        var distance = Vector3.Distance(lastPos, newPos);
                        Melon<Mod>.Logger.Msg($"Distance travelled by {playerId} in {delta} seconds: {distance}");
                        // Get max distance possible for delta
                        var maxDistancePossible = PlayerMovementController.RunSpeed * delta;
                        if (distance > maxDistancePossible * overspeedTolerance)
                        {
                            Melon<Mod>.Logger.Msg($"Speedhack detected (Player: {playerId})");
                        }
                    }
                }
                // Reset timer
                timer.Restart();
            }
        }
    }
}