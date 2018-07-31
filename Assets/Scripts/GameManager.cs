﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {

    public static GameManager instance;
    public Text messagText;
    public int minPlayers = 2;
    public int maxPlayers = 4;

    [SyncVar]
    public int playerCount = 0;


    public List<PlayerControl> allPlayers;
    public List<Text> nameText;
    public List<Text> playerScoreText;

    public Color[] playerColors;

    public int maxScore = 3;

    [SyncVar]
    public bool gameOver = false;

    private PlayerControl winner;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start () {
        StartCoroutine(GameLoopRoutine());
    }
	
	void Update () {
		
	}

    public IEnumerator GameLoopRoutine()
    {
        yield return StartCoroutine(EnterLoby());
        yield return StartCoroutine(PlayGame());
        yield return StartCoroutine(EndGame());
        StartCoroutine(GameLoopRoutine());
    }
    private IEnumerator EnterLoby()
    {
        while (playerCount < minPlayers)
        {
            UpdateMessage("Esperando jogadores...");
            DisablePlayers();
            yield return null;
        }
    }

    private IEnumerator PlayGame()
    {
        UpdateMessage("");
        EnablePlayers();
        UpdateScore();
        while (!gameOver)
        {
            yield return null;
        }
    }

    private IEnumerator EndGame()
    {
        DisablePlayers();
        UpdateMessage("Game Over \n" + winner.pSetup.playerNameText.text + " venceu!");
        Reset();
        yield return new WaitForSeconds(3f);
        UpdateMessage("Reiniciando...");
        yield return new WaitForSeconds(3f);
    }

    [ClientRpc]
    public void RpcUpdateMessage(string msg)
    {
        messagText.gameObject.SetActive(true);
        messagText.text = msg;
    } 

    public void UpdateMessage(string msg)
    {
        if (isServer)
        {
            RpcUpdateMessage(msg);
        }
    }

    [ClientRpc]
    private void RpcSetPlayerState(bool state)
    {
        PlayerControl[] allPlayers = FindObjectsOfType<PlayerControl>();
        foreach (PlayerControl p in allPlayers)
        {
            p.enabled = state;
        }
    }

    private void EnablePlayers()
    {
        if (isServer)
        {
            RpcSetPlayerState(true);
        }
        
    }
    private void DisablePlayers()
    {
        if (isServer)
        {
            RpcSetPlayerState(false);
        }
    }

    public void AddPlayer(PlayerSetup pS)
    {
        /*if(playerCount < maxPlayers)
        {
            allPlayers.Add(pS.GetComponent<PlayerControl>());
            pS.playerColor = playerColors[playerCount];
            pS.playerNum = playerCount + 1;
            playerCount++;
        }*/

        playerCount++;
    }

    [ClientRpc]
    public void RpcUpdateScore(int[] playerScores, string[] playerNames)
    {
        for(int i = 0; i < playerCount; i++)
        {
            playerScoreText[i].text = playerScores[i].ToString();
            nameText[i].text = playerNames[i];
        }
    }

    public void UpdateScore()
    {
        if (isServer)
        {
            winner = GetWinner();

            if(winner != null)
            {
                gameOver = true;
            }

            int[] scores = new int[playerCount];
            string[] names = new string[playerCount];

            for (int i = 0; i < playerCount; i++)
            {
                scores[i] = allPlayers[i].score;
                names[i] = allPlayers[i].GetComponent<PlayerSetup>().playerNameText.text;
            }
            RpcUpdateScore(scores, names);
        }
    }

    public PlayerControl GetWinner()
    {
        if (isServer)
        {
            for(int i = 0; i < playerCount; i++)
            {
                if(allPlayers[i].score >= maxScore)
                {
                    return allPlayers[i];
                }
            }
        }
        return null;
    }

    [ClientRpc]
    public void RpcReset()
    {
        PlayerControl[] players = FindObjectsOfType<PlayerControl>();
        foreach (PlayerControl player in players)
        {
            player.score = 0;
        }
    }

    public void Reset()
    {
        if (isServer)
        {
            RpcReset();
            UpdateScore();
            winner = null;
            gameOver = false;
        }
    }
}
