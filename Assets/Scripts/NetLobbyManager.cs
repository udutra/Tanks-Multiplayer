using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class NetLobbyManager : LobbyHook {

    public LobbyPlayer lPlayer;
    public PlayerSetup pSetup;

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        lPlayer = lobbyPlayer.GetComponent<LobbyPlayer>();
        pSetup = gamePlayer.GetComponent<PlayerSetup>();

        pSetup.baseName = lPlayer.playerName;
        pSetup.playerColor = lPlayer.playerColor;
    }
}