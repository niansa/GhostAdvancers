using System;
using System.Diagnostics;
using UnityEngine;
using Donteco;
using GhostAdvancers;
using System.Collections.Generic;
using System.Linq;
using static Donteco.GameData;
using MelonLoader;
using TriangleNet.Topology.DCEL;

namespace GhostAdvancers
{
    public class QuickPlay
    {
        public static double interval = 5.0;
        public static int lobbiesPerPage = 100;

        public static event Action<Lobby> OnLobbyFound;

        private int lastPage;
        private LobbyManager lobbyMan;
        private Stopwatch timer;
        private List<Lobby>? lastLobbyList;
        private List<Lobby> newLobbyList;

        static QuickPlay()
        {
            OnLobbyFound = delegate { };
        }
        public QuickPlay()
        {
            lobbyMan = new LobbyManager();
            LobbyManager.OnChangeLobbiesList += OnChangeLobbiesList;
            newLobbyList = new List<Lobby>();
            timer = new Stopwatch();
            timer.Start();
        }
        ~QuickPlay() {
            LobbyManager.OnChangeLobbiesList -= OnChangeLobbiesList;
        }

        private void OnChangeLobbiesList(Lobby[] lobbies) // TODO: I could check for new lobbies while fetching them instead
        {
            // Make sure the list received isn't null
            if (lobbies != null)
            {
                Melon<Mod>.Logger.Msg($"Received {lobbies.Length} lobbies from server");
                // Add lobbies to new lobby list
                newLobbyList.AddRange(lobbies);
                // Make sure all lobbies were fetched
                if (lobbies.Length == lobbiesPerPage)
                {
                    Melon<Mod>.Logger.Msg("Requesting next page...");
                    // Fetch the rest of the lobbies
                    lobbyMan.LobbiesList(++lastPage, lobbiesPerPage);
                    return;
                }
            }
            // Check for new lobbies
            if (lastLobbyList != null)
            {
                // Find lobbies that haven't already existed before
                // Note: I could've done this with lambdas and shit but this seem so much more readable to me... Don't know much about them anyways, I come from the C++ world where that lambda stuff seems so much easier
                foreach (var newLobby in newLobbyList)
                {
                    var newLobbyCode = newLobby.Settings.Settings["code"].ToString();
                    bool isNew = true;
                    foreach (var oldLobby in lastLobbyList)
                    {
                        var oldLobbyCode = oldLobby.Settings.Settings["code"].ToString();
                        if (oldLobbyCode == newLobbyCode)
                        {
                            isNew = false;
                        }
                    }
                    if (isNew)
                    {
                        Melon<Mod>.Logger.Msg($"Found new lobby \"{newLobby.Title}\" ({newLobbyCode})!");
                        OnLobbyFound.Invoke(newLobby);
                        break;
                    }
                }
            }
            // Clean up
            lastLobbyList = newLobbyList;
            newLobbyList = new List<Lobby>();
        }
        public void RunUpdate()
        {
            // Check if it's time to request a new list
            if (timer.Elapsed.TotalMilliseconds/1000.0 > interval) {
                // Fetch lobbies from the start
                lastPage = 0;
                lobbyMan.LobbiesList(0, lobbiesPerPage);
                // Reset timer
                timer.Restart();
            }
        }
    }
}